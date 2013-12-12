namespace WebApplication1.Lib.Domain.Models
{
    public class User : IModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}