using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Kullanıcı yetkilendirmesine göre ulaşılabilir bir controller haline dönüştürülmüştür. Yani kullanıcının Rolü Member ise ulaşabilir. Bu attribute kullanıcı rolü tabanlı bir yetkilendirmedir.
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IAppUserRepository _appUserRepository;

        public CommentController(ICommentRepository commentRepository, IArticleRepository articleRepository, IAppUserRepository appUserRepository)
        {
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _appUserRepository = appUserRepository;
        }

        // List
        public IActionResult List()
        {
            List<Comment> comments = _commentRepository.GetByDefaults
                (
                selector: a => new Comment()
                {
                    AppUser = a.AppUser,
                    Article = a.Article,
                    Text = a.Text,
                    ID = a.ID,
                },
                expression: a => a.Statu != Statu.Passive,
                include: a => a.Include(a => a.AppUser).Include(a => a.Article)
                );

            return View(comments);
        }

        // Delete
        public IActionResult Delete(int id)
        {
            Comment comment = _commentRepository.GetDefault(a => a.ID == id);
            _commentRepository.Delete(comment);

            return RedirectToAction("Detail", "Article", new
            {
                id = comment.ArticleID
            });

        }
 
        // Checklist
        public IActionResult CheckList()
        {
            List<Comment> comments = _commentRepository.GetByDefaults
               (
               selector: a => new Comment()
               {
                   AppUser = a.AppUser,
                   Article = a.Article,
                   Text = a.Text,
                   ID = a.ID,
               },
               expression: a => a.Statu == Statu.Passive && a.AdminCheck==AdminCheck.Waiting,
               include: a => a.Include(a => a.AppUser).Include(a => a.Article)
               );

            return View(comments);
        }

        // Approve
        public IActionResult Approve(int id)
        {
            Comment comment = _commentRepository.GetDefault(a => a.ID == id);
            _commentRepository.Approve(comment);
            return RedirectToAction("Checklist");
        }

        // Reject
        public IActionResult Reject(int id)
        {
            Comment comment = _commentRepository.GetDefault(a => a.ID == id);
            _commentRepository.Reject(comment);
            return RedirectToAction("Checklist");
        }
    }
}
