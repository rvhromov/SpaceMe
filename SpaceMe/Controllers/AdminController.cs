using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SpaceMe.Models;
using SpaceMe.Repository;

namespace SpaceMe.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IRepository repo;
        public AdminController(IRepository r)
        {
            repo = r;
        }

        //Property to interact with the user store
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        //Property to manage website login
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Check if user exists
                ApplicationUser user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    // Create claim
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    // Delete auth cookies
                    AuthenticationManager.SignOut();
                    // Create auth cookies
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
                    // Redirect to the admin page
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ModelState.AddModelError("LoginErr", "Wrong email or password");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            // Delete auth cookies
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            return View(repo.GetPostList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new Post
            {
                PostedOn = DateTime.Now
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Post post, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid && uploadImage != null)
            {
                // Read uploaded image into byte array
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                post.Image = imageData;
                post.Title = post.Title.ToLower();

                // Save post
                if (await repo.Create(post) > 0)
                    return RedirectToAction("Index", "Admin");
                else
                    ModelState.AddModelError("CreateErr", "Unabble to create new post");
            }

            return View(post);
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            // Find and return post by Id
            Post post = await repo.GetPost(id);
            if (post == null)
                return HttpNotFound();

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Post post, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                // Should current image be update to a new one
                bool updateImage = true;
                if (uploadImage != null)
                {
                    post.Image = new byte[uploadImage.ContentLength];
                    uploadImage.InputStream.Read(post.Image, 0, uploadImage.ContentLength);
                }
                else
                {
                    updateImage = false;
                }
                post.Title = post.Title.ToLower();

                // Save changes
                if (await repo.Update(post, updateImage) > 0)
                    return RedirectToAction("Index", "Admin");
                else
                    ModelState.AddModelError("UpdateErr", "Unabble to update post");
            }

            return View(post);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            // Find and return post by Id
            Post post = await repo.GetPost(id);
            if (post == null)
                return HttpNotFound();

            return View(post);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            // Delete post
            if (await repo.Delete(id))
                return RedirectToAction("Index", "Admin");

            return HttpNotFound();
        }

        public async Task<ActionResult> RetrieveImage(int id)
        {
            // Retrieve image from DB
            var post = await repo.GetPost(id);
            byte[] img = post.Image;

            if (img != null)
                return File(img, "image/jpg");
            else
                return null;
        }

        [HttpGet]
        public ActionResult NewAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewAdmin(string email)
        {
            // Create new admin
            string password = GeneratePassword(8);
            var admin = new ApplicationUser { UserName = email, Email = email };
            var result = UserManager.Create(admin, password);

            // Send login and password to the new admin
            if (result.Succeeded)
            {
                SendEmail(email, password);
                return View("Succeeded");
            }

            return View();
        }

        private static Random random = new Random((int)DateTime.Now.Ticks);

        private string GeneratePassword(int size)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var builder = new StringBuilder();

            for (var i = 0; i < size; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }

        private void SendEmail(string email, string password)
        {
            // Form and send an email
            string subject = "Your details for SpaceMe administration";
            var callbackUrl = Url.Action("Login", "Admin", null, Request.Url.Scheme);
            string msg = "Welcome to SpaceMe team.<br /><br /> Your details for admin authorization:<br />" +
                         "Login: " + email + "<br />" + "Password: " + password + "<br /><br />" +
                         "Use them to enter <a href=\"" + callbackUrl + "\">here</a>";

            MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["mailAccount"], email, subject, msg);
            mail.From = new MailAddress(ConfigurationManager.AppSettings["mailAccount"], "Space Me");
            mail.IsBodyHtml = true;

            NetworkCredential credential = new NetworkCredential
            (
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]
            );
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = credential;
            smtp.Send(mail);
        }
    }
}