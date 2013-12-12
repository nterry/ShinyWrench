using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Lib.Domain;
using WebApplication1.Lib.Domain.Impl;
using WebApplication1.Lib.Domain.Java.JavaModels;
using WebApplication1.Lib.Domain.Models;

namespace WebApplication1.Controllers
{
    //http://stackoverflow.com/questions/2465462/asp-net-mvc-post-action-method-with-additional-parameters-from-url
    public class HomeController : GeneralController
    {
        private static readonly IDatabaseConnection userDbConnection;
        private static readonly IDatabaseConnection dataDbConnection;
        private static readonly List<string> tokenList;
 
        //Needs to be static as a new controller is created every request and these need to be persisted.
        static HomeController()
        {
            userDbConnection = new FileBasedDatabaseConnector().Connect(@"C:\data", "", "", "user");
            dataDbConnection = new FileBasedDatabaseConnector().Connect(@"C:\data", "", "", "data");
            tokenList = new List<string>();
        }

        //Called every request
        public HomeController()
        {
            ViewBag.LoggedIn = false;   //Assume logged out and check for login
        }

        public ActionResult Login()
        {
            return CheckToken(GetRequestTokenOrDefault(), () =>
                   {
                       TempData["LoggedIn"] = true;
                       TempData["LoggedInMessage"] = "Please log out to re-login.";
                       return Redirect("/");
                   }, () =>
                   {
                       ViewBag.Message = "You are not logged in. Please login to continue.";
                       ViewBag.LoginBad = (TempData["LoginBad"] != null);
                       return View();
                   });
        }

        [HttpPost]
        public ActionResult LoginPost(string username, string password, bool? remember)
        {
            var result = userDbConnection.Get<User>(x => x.Username == username && x.Password == password);
            if (result.Length == 0)
            {
                Session["LoginBad"] = true;
                //TempData["LoginBad"] = true;
                return Redirect("/Home/Login");
            }
            //TempData["ActiveUser"] = username;
            Session["ActiveUser"] = username;
            IssueToken();
            return Redirect("/");
        }

        //TODO: Need to set @ViewBag.ActiveUser in onSuccess
        public ActionResult Index()
        {
            ViewBag.LoggedIn = ( TempData["LoggedIn"] != null ) || ( GetRequestTokenOrDefault() != null );
            ViewBag.LoggedInMessage = TempData["LoggedInMessage"] ?? "";
            return CheckToken(GetRequestTokenOrDefault(), View, () => Redirect("/Home/Login"));
        }

        public ActionResult Register()
        {
            return CheckToken(GetRequestTokenOrDefault(), () => 
                   {
                       TempData["LoggedIn"] = true;
                       TempData["LoggedInMessage"] = "Please log out to register.";
                       return Redirect("/");
                   }, () =>
                   {
                       ViewBag.Message = "Fill out the form to register.";
                       ViewBag.LoginBad = (TempData["RegisterBad"] != null);
                       ViewBag.LoginBadMessage = TempData["RegisterBadMessage"] ?? "";
                       return View();
                   });
        }

        [HttpPost]
        public ActionResult RegisterPost(string username, string password, string passwordConfirm)
        {
            if (password != passwordConfirm)
            {
                TempData["RegisterBad"] = true;
                TempData["RegisterBadMessage"] = "Password fields must match!";
            }
            userDbConnection.Apply(new User{ Username = username, Password = password });
            IssueToken();
            return Redirect("/");
        }

        public ActionResult Logout()
        {
            Session["AuthToken"] = null;
            return Redirect("/");   //TODO: Need to make logout landing page.
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Quizform()
        {
            ViewBag.LoggedIn = (GetRequestTokenOrDefault() != null);
            //ViewBag.Class = dataDbConnection.Get<JavaClass>(new Dictionary<string, string> { { "ClassName", "Puppy" }, { "", "" } });
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        private ActionResult CheckToken(object token, Func<ActionResult> onSuccess, Func<ActionResult> onFailure)
        {
            if (token == null || !tokenList.Contains(token.ToString()))
                return onFailure();
            return onSuccess();
        }

        private string GetRequestTokenOrDefault()
        {
            var httpCookie = Request.Cookies.Get("Authorization");
            return httpCookie != null ? httpCookie.Value : null;
        }

        private void IssueToken()
        {
            var token = Guid.NewGuid().ToString();
            var authCookie = new HttpCookie("Authorization") { Expires = DateTime.Now.AddDays(+1d), Value = token };
            Response.SetCookie(authCookie);
            tokenList.Add(token);
        }
    }
}