using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Areas.Admin.Models.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Kullanıcı yetkilendirmesine göre ulaşılabilir bir controller haline dönüştürülmüştür. Yani kullanıcının Rolü Member ise ulaşabilir. Bu attribute kullanıcı rolü tabanlı bir yetkilendirmedir.
    public class ArticleController : Controller
    {

        private readonly IArticleRepository _articleRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppUserRepository _appUserRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;

        public ArticleController(IArticleRepository articleRepository, UserManager<IdentityUser> userManager, IAppUserRepository appUserRepository, ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment, ILikeRepository likeRepository, ICommentRepository commentRepository)
        {
            _articleRepository = articleRepository;
            _userManager = userManager;
            _appUserRepository = appUserRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment; // IWebHostEnvironment interface’i, Asp.NET Core mimarisi dahilinde gelen ve projenin bulunduğu sunucu hakkında bizlere gerekli ortamsal bilgileri getirecek alanları barındıran bir yapıya sahiptir.
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
        }
        // List
        public async Task<IActionResult> List()
        {

            // Birden fazla entity içindeki propertyler kullanılarak oluşturulan sınıflar için ViewModel yani VM oluşturulur. 
            // Areas => Models klasörü altına VMs klasörü altına GetArticleVM isimli VM sınıfı açılır.

            var articleList = _articleRepository.GetByDefaults
                (
                // Selector parametresi ile yeni bir GetArticleVM tipinde yeni bir nesne oluşturacağımız söylenir.
                selector: a => new GetArticleVM()
                {
                    ArticleID = a.ID,
                    CategoryName = a.Category.Name, // include dan gelecek olan veri
                    Title = a.Title,
                    Content = a.Content,
                    Image = a.Image,
                    UserFullName = a.AppUser.FullName // include dan gelecek olan veri
                },
                // Expression parametresi ile Statu su passive olmayan ve makalenin yazarı aktif olan yazar olsun koşulu söylenir
                expression: a => a.Statu != Statu.Passive,
                // Article tablosu ile AppUser ve Category tablosunun birleştirildiği söylenir.
                include: a => a.Include(a => a.AppUser).Include(a => a.Category),
                // Sıralarken son oluşturulma tarihinden itibaren sırala dedik
                orderby: a => a.OrderByDescending(a => a.CreateDate)
                );

            return View(articleList);
        }

        // Detail
        public async Task<IActionResult> Detail(int id)
        {
            // Article sayfasında da profil bilgileri gösterileceği için ArticleDetailVM isimli VM kullanılır
            ArticleDetailVM articleDetailVM = new ArticleDetailVM()
            {
                Article = _articleRepository.GetDefault(a => a.ID == id),

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

            // Vm in ActiveAppUserID propertsi doldurularak eğer aktif kullanıcının yorumu varsa düzenlenebilir seçeneği eklenir. Yani aktif kullanıcı ActiveAppUser nesnesi doldurulur.
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            articleDetailVM.ActiveAppUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            // Vm in ActiveArticleID propertysi doldurulur. Yorum oluşturulması sırasında kullanılacaktır.
            articleDetailVM.ActiveArticleID = id;

            // Makale başlığına her tıklandığında okuma sayısı 1 artacaktır.
            _articleRepository.Read(articleDetailVM.Article);

            return View(articleDetailVM);
        }

        // Delete
        public IActionResult Delete(int id)
        {
            Article article = _articleRepository.GetDefault(a => a.ID == id);
            _articleRepository.Delete(article);
            return RedirectToAction("Index", "AppUser");
        }

        // Like
        public async Task<IActionResult> Like(int id)
        {
            Article article = _articleRepository.GetDefault(a => a.ID == id);

            IdentityUser identityUser = await _userManager.GetUserAsync(User);

            AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);


            Like like = new Like()
            {
                AppUser = appUser,
                AppUserID = appUser.ID,
                Article = article,
                ArticleID = article.ID
            };

            _likeRepository.Create(like);

            return RedirectToAction("Detail", new
            {
                id = id
            });
        }

        // Unlike
        public async Task<IActionResult> Unlike(int id)
        {
            Article article = _articleRepository.GetDefault(a => a.ID == id);

            IdentityUser identityUser = await _userManager.GetUserAsync(User);

            AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            Like like = _likeRepository.GetDefault(a => a.ArticleID == article.ID && a.AppUserID == appUser.ID);

            _likeRepository.Delete(like);
            return RedirectToAction("Detail", new { id = id });
        }

        // ListArticle Of Category
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
                        ReadCounter = a.ReadCounter
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
                return RedirectToAction("Index", "AppUser");
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
                    ReadCounter = a.ReadCounter
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

        // Checklist
        public IActionResult Checklist()
        {
            var articleList = _articleRepository.GetByDefaults
                 (
                 // Selector parametresi ile yeni bir GetArticleVM tipinde yeni bir nesne oluşturacağımız söylenir.
                 selector: a => new GetArticleVM()
                 {
                     ArticleID = a.ID,
                     CategoryName = a.Category.Name, // include dan gelecek olan veri
                     Title = a.Title,
                     Content = a.Content,
                     Image = a.Image,
                     UserFullName = a.AppUser.FullName // include dan gelecek olan veri
                 },
                 // Expression parametresi ile Statu su passive olmayan ve makalenin yazarı aktif olan yazar olsun koşulu söylenir
                 expression: a => a.Statu == Statu.Passive && a.AdminCheck == AdminCheck.Waiting,
                 // Article tablosu ile AppUser ve Category tablosunun birleştirildiği söylenir.
                 include: a => a.Include(a => a.AppUser).Include(a => a.Category),
                 // Sıralarken son oluşturulma tarihinden itibaren sırala dedik
                 orderby: a => a.OrderByDescending(a => a.CreateDate)
                 );
            return View(articleList);
        }

        // Approve
        public IActionResult Approve(int id)
        {
            Article article = _articleRepository.GetDefault(a => a.ID == id);
            _articleRepository.Approve(article);
            return RedirectToAction("Checklist");
        }

        // Reject
        public IActionResult Reject(int id)
        {
            Article article = _articleRepository.GetDefault(a => a.ID == id);
            _articleRepository.Reject(article);
            return RedirectToAction("Checklist");
        }
    }
}
