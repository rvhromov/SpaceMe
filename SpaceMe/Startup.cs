using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SpaceMe.Models;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(SpaceMe.Startup))]
namespace SpaceMe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createAdmin();
        }

        // Create default admin
        private void createAdmin()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = new ApplicationUser();
            user.UserName = ConfigurationManager.AppSettings["mailAccount"];
            user.Email = ConfigurationManager.AppSettings["mailAccount"];

            string userPWD = ConfigurationManager.AppSettings["mailPassword"];

            UserManager.Create(user, userPWD);
        }
    }
}
