using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApplication1.Lib.Domain;
using WebApplication1.Lib.Domain.Impl;
using WebApplication1.Lib.Domain.Models;

namespace WebApplication1.Controllers
{
    //http://stackoverflow.com/questions/2465462/asp-net-mvc-post-action-method-with-additional-parameters-from-url
    public class HomeController : GeneralController
    {
        private readonly IDatabaseConnection dbConnection;
        private readonly List<string> tokenList; 
        public HomeController()
        {
            dbConnection = new FileBasedDatabaseConnector().Connect(@"C:\data", "bob", "password", "test");
            tokenList = new List<string>();
        }

        public ActionResult Login()
        {
            return CheckToken(Session["AuthToken"], () => Redirect("/"), () =>
                   {
                       ViewBag.Message = "You are not logged in. Please login to continue.";
                       ViewBag.LoginBad = (TempData["LoginBad"] != null);
                       return View();
                   });
        }

        [HttpPost]
        public ActionResult LoginPost(string username, string password, bool? remember)
        {
            var result = dbConnection.Get<User>(new Dictionary<string, string> { { "Username", username }, { "Password", password } });
            if (result.Length == 0)
            {
                TempData["LoginBad"] = true;
                return Redirect("/Home/Login");

            }
            TempData["ActiveUser"] = username;
            IssueToken();
            return Redirect("/");
        }

        public ActionResult Index()
        {
            return CheckToken(Session["AuthToken"], View, () => Redirect("/Home/Login"));
        }

        public ActionResult Register()
        {
            return CheckToken(Session["AuthToken"], () => Redirect("/"), () =>
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
            dbConnection.Apply(new User{ Username = username, Password = password });
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

            Session["AuthToken"] = "I am here";

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

        private void IssueToken()
        {
            var token = Guid.NewGuid().ToString();
            tokenList.Add(token);
            Session["AuthToken"] = token;
        }
    }
}