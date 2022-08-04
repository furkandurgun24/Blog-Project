using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Areas.Member.Models.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.WEB.Areas.Member.Controllers
{
    [Area("Member")]
    [Authorize(Roles = "Member")] // Kullanıcı yetkilendirmesine göre ulaşılabilir bir controller haline dönüştürülmüştür. Yani kullanıcının Rolü Member ise ulaşabilir. Bu attribute kullanıcı rolü tabanlı bir yetkilendirmedir.
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IArticleRepository _articleRepository;

        public CommentController(ICommentRepository commentRepository, IArticleRepository articleRepository, UserManager<IdentityUser> userManager, IAppUserRepository appUserRepository)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
            _appUserRepository = appUserRepository;
            _articleRepository = articleRepository;
        }

        // Create Comment
        public async Task<IActionResult> Create(ArticleDetailVM articleDetailVM)
        {
            if (ModelState.IsValid)
            {
                // Yorumun yapıldığı makale bulunur
                Article article = _articleRepository.GetDefault(a => a.ID == articleDetailVM.ActiveArticleID);

                // Yorumu yapan kullanıcı bulunur.
                IdentityUser identityUser = await _userManager.GetUserAsync(User);
                AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

                // Vm içerisinden gerekli yerler alınarak Comment nesnesi oluşturulur.

                Comment comment = new Comment()
                {
                    AppUser = appUser,
                    AppUserID = appUser.ID,
                    Article = article,
                    ArticleID = article.ID,
                    Text = articleDetailVM.CommentText
                };

                _commentRepository.Create(comment);
                return RedirectToAction("Detail", "Article", new { id = article.ID });

            }
            TempData["Message"] = "Lüten Minimum karakter sayısından fazla yazınız";
            return RedirectToAction("Detail", "Article", new { id = articleDetailVM.ActiveArticleID });
        }

        // update
        public IActionResult Update(ArticleDetailVM articleDetailVM)
        {
            if (ModelState.IsValid)
            {
                Comment comment = _commentRepository.GetDefault(a => a.ID == articleDetailVM.CommentID);
                comment.Text = articleDetailVM.CommentText;
                _commentRepository.Update(comment);
                return RedirectToAction("Detail", "Article", new
                {
                    id = comment.ArticleID
                });
            }
            return RedirectToAction("Detail", "Article", new
            {
                id = articleDetailVM.ArticleID
            });
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
        
    }
}
