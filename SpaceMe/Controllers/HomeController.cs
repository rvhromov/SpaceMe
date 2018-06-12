using SpaceMe.Models;
using SpaceMe.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpaceMe.Controllers
{
    public class HomeController : Controller
    {
        IRepository repo;
        public HomeController(IRepository r)
        {
            repo = r;
        }

        public ActionResult Index(int page = 0)
        {
            // Display all posts
            var posts = repo.GetPostList();

            // Pagination
            const int pageSize = 5;
            int count = posts.Count();
            IEnumerable<Post> postsPerPage = posts.OrderBy(p => p.Id).Skip(page * pageSize).Take(pageSize).ToList();
            ViewBag.MaxPage = (count / pageSize) - (count % pageSize == 0 ? 1 : 0);
            ViewBag.Page = page;

            return View(postsPerPage);
        }

        public async Task<ActionResult> Post(int id)
        {
            // Display particular post
            Post post = await repo.GetPost(id);
            if (post == null)
                HttpNotFound();

            return View(post);
        }

        public ActionResult Search(string searching)
        {
            // Search for posts by their title
            var posts = repo.GetPostList();
            return View("Index", posts.Where(p => p.Title.Contains(searching.ToLower())).ToList());
        }
    }
}