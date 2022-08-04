using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Models.VMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.WEB.Controllers
{
    public class ArticleController : Controller
    {

        private readonly IArticleRepository _articleRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ArticleController(IArticleRepository articleRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, IAppUserRepository appUserRepository, UserManager<IdentityUser> userManager, ICategoryRepository categoryRepository)
        {
            _articleRepository = articleRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _appUserRepository = appUserRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }

        // Detail 
        public IActionResult Detail(int id)
        {
            // Article sayfasında da profil bilgileri gösterileceği için ArticleDetailVM isimli VM kullanılır
            ArticleDetailVM articleDetailVM = new ArticleDetailVM()
            {
                Article = _articleRepository.GetDefault(a => a.ID == id)

            };

            // Eager loading olduğu için articleDetailVM in Article propertysinin category propertysi doldurulur
            articleDetailVM.Article.Category = _categoryRepository.GetDefault(a => a.ID == articleDetailVM.Article.CategoryID);

            // VM nun Article propertysinin AppUser propertysi doldurulur. Makaleyi yazan kişi
            articleDetailVM.Article.AppUser = _appUserRepository.GetDefault(a => a.ID == articleDetailVM.Article.AppUserID);

            // VM nun UserFollowedCategories listesi doldurulur. Yazarın takip listesi
            articleDetailVM.userFollowedCategories = _categoryRepository.GetCategoryWithUser(articleDetailVM.Article.AppUser.ID);

            // VM nun Yazarın Mail propertysi doldurulur. AppUser nesnesinde Mail kolonu olmadığı için UserManager tablosundan çekilir.
            articleDetailVM.Mail = _userManager.Users.FirstOrDefault(a => a.Id == articleDetailVM.Article.AppUser.IdentityId).Email;

            // VM in Article nesne propertysinin Likes propertysi doldurulur.
            articleDetailVM.Article.Likes = _likeRepository.GetLikes(a => a.ArticleID == articleDetailVM.Article.ID);

            // VM in Article nesne propertysinin comment liste propertysi doldurulur.
            articleDetailVM.Article.Comments = _commentRepository.GetDefaults(a => a.ArticleID == articleDetailVM.Article.ID && a.Statu != Statu.Passive);

            // VM in Article nesne propertsinin comment nesne propertysinin AppUser propertysi yani yorumu yapan kişiler doldurulur.
            foreach (var item in articleDetailVM.Article.Comments)
            {
                item.AppUser = _appUserRepository.GetDefault(a => a.ID == item.AppUserID);
            }

            // Makale başlığına her tıklandığında okuma sayısı 1 artacaktır.
            _articleRepository.Read(articleDetailVM.Article);

            return View(articleDetailVM);
        }

        public IActionResult ListWithFilter(int id)
        {
            List<GetArticleWithUserVM> articles = _articleRepository.GetByDefaults
                (
                    // Selector parametresi ile yeni bir GetArticleWithUserVM tipinde yeni bir nesne oluşturacağımız söylenir.
                    selector: a => new GetArticleWithUserVM()
                    {
                        Title = a.Title,
                        Content = a.Content,
                        UserId = a.AppUser.ID, // AppUser tablosu ile include edilip alınacak veri
                        UserFullName = a.AppUser.FullName, // AppUser tablosu ile include edilip alınacak veri
                        UserImage = a.AppUser.Image,
                        ArticleId = a.ID,
                        Image = a.Image,
                        CreatedDate = a.CreateDate,
                        CategoryName = a.Category.Name, // Category tablosu ile include edilip alınacak veri
                        CategoryID = a.CategoryID,
                        ReadingTime = a.ReadingTime,
                        CreateDate = a.CreateDate,
                        ReadCounter=a.ReadCounter
                    },
                    // Expression parametresi ile Statu su passive olmayan  koşulu söylenir
                    expression: a => a.Statu != Statu.Passive && a.CategoryID == id,
                    // Article tablosu ile AppUser ve Category tablosunun birleştirildiği söylenir.
                    include: a => a.Include(a => a.AppUser).Include(a => a.Category),
                    // Sıralarken son oluşturulma tarihinden itibaren sırala dedik
                    orderby: a => a.OrderByDescending(a => a.CreateDate)
                );
            // Oluşturulan bu articles nesnesinin hepsini alacak. Take metodu kullanılmayacaktır için tekrardan ToList() metodu kullanılmak zorundadır.
            ViewBag.AllCategory = _categoryRepository.GetDefaults(a => a.Statu != Statu.Passive);
            return View(articles);
        }

        // ListWithFilters
        public IActionResult ListWithFilters(List<int> categories)
        {
            if (categories.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GetArticleWithUserVM> articles = _articleRepository.GetByDefaults
                 (
                // Selector parametresi ile yeni bir GetArticleWithUserVM tipinde yeni bir nesne oluşturacağımız söylenir.
                selector: a => new GetArticleWithUserVM()
                {
                    Title = a.Title,
                    Content = a.Content,
                    UserId = a.AppUser.ID, // AppUser tablosu ile include edilip alınacak veri
                    UserFullName = a.AppUser.FullName, // AppUser tablosu ile include edilip alınacak veri
                    UserImage = a.AppUser.Image,
                    ArticleId = a.ID,
                    Image = a.Image,
                    CreatedDate = a.CreateDate,
                    CategoryName = a.Category.Name, // Category tablosu ile include edilip alınacak veri
                    CategoryID = a.CategoryID,
                    ReadingTime = a.ReadingTime,
                    CreateDate = a.CreateDate,
                    ReadCounter = a.ReadCounter,
                },
        // Expression parametresi ile Statu su passive olmayan  koşulu söylenir
        expression: a => a.Statu != Statu.Passive,
        // Article tablosu ile AppUser ve Category tablosunun birleştirildiği söylenir.
        include: a => a.Include(a => a.AppUser).Include(a => a.Category),
        // Sıralarken son oluşturulma tarihinden itibaren sırala dedik
        orderby: a => a.OrderByDescending(a => a.CreateDate)
                 );

            var newArticleList = articles.Where(article => categories.Contains(article.CategoryID)).ToList();

            // Yedek Code

            // List<GetArticleWithUserVM> filteredList = new List<GetArticleWithUserVM>();

            //foreach (var item in articles)
            //{
            //    if (categories.Contains(item.CategoryID))
            //    {
            //        filteredList.Add(item);
            //    }
            //}

            // Oluşturulan bu articles nesnesinin hepsini alacak. Take metodu kullanılmayacaktır için tekrardan ToList() metodu kullanılmak zorundadır.
            ViewBag.AllCategory = _categoryRepository.GetDefaults(a => a.Statu != Statu.Passive);

            return View(newArticleList);


        }
    }
}
