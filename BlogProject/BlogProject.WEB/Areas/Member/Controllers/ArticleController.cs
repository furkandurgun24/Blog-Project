using AutoMapper;
using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Areas.Member.Models.DTOs;
using BlogProject.WEB.Areas.Member.Models.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;


namespace BlogProject.WEB.Areas.Member.Controllers
{
    [Area("Member")] // Member Area nın controlleri olduğu belirtilir yazılmadığı taktirde controller isimleri çakışırsa multiendpoint hatası verir.
    [Authorize(Roles = "Member")] // Kullanıcı yetkilendirmesine göre ulaşılabilir bir controller haline dönüştürülmüştür. Yani kullanıcının Rolü Member ise ulaşabilir. Bu attribute kullanıcı rolü tabanlı bir yetkilendirmedir.
    public class ArticleController : Controller
    {
        // Bu controllerde kullanılacak olan Repositoryler ctor un giriş parametresi içerisinde çağrılır. SOLID in D prensibi gereği Constructor Injection yapılır. Program.cs içerisinde AddScope<InterfaceName,ClassName> şeklinde eklenmesi unutulmamalıdır.

        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppUserRepository _appUserRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;

        public ArticleController(IArticleRepository articleRepository, IMapper mapper, UserManager<IdentityUser> userManager, IAppUserRepository appUserRepository, ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment, ILikeRepository likeRepository, ICommentRepository commentRepository)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _userManager = userManager;
            _appUserRepository = appUserRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment; // IWebHostEnvironment interface’i, Asp.NET Core mimarisi dahilinde gelen ve projenin bulunduğu sunucu hakkında bizlere gerekli ortamsal bilgileri getirecek alanları barındıran bir yapıya sahiptir.
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
        }


        // Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Article ın AppUserID kısmını doldurmak için aktif olan kullanıcı bulunur.
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            AppUser user = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            // Kullanıcı Article oluştururken Kategoriyi seçebilmek için CreateArticleDTO doldurulur.
            CreateArticleDTO dto = new CreateArticleDTO()
            {
                Categories = _categoryRepository.GetByDefaults
                (
                    selector: a => new GetCategoryDTO
                    {
                        ID = a.ID,
                        Name = a.Name
                    },
                     expression: a => a.Statu != Statu.Passive
                ),

                AppUserID = user.ID
            };

            return View(dto);
        }

        [HttpPost]
        public IActionResult Create(CreateArticleDTO createArticleDTO)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıdan alınan veriler AutoMapper kütüphanesi metodu kullanılarak mapping işlemi yapılır. Lakin. 
                Article article = _mapper.Map<Article>(createArticleDTO);

                //Mapping sınıfında İmagePath propertysi NotMapped olduğu için article in İmage propertysi manuel atanır.
                if (article.ImagePath != null)
                {
                    // Using anahtar kelimesi ile tanımlanan değişkenler işleri bittikten sonra ramdan kalkarlar.
                    using var image = Image.Load(createArticleDTO.ImagePath.OpenReadStream());
                    image.Mutate(a => a.Resize(1000, 1000)); // Şekillendirme ve boyutlandırma işlemleri

                    // Fotoğraf kayıdı için dosya yolu söylenir. Dosya adı olarak eşsiz bir GUID ID verilerek kayıt edecek
                    Guid guid = Guid.NewGuid();

                    image.Save($"wwwroot/images/articles/{guid}.jpg");

                    // Veri tabanındaki AppUser tablosunun dosya yolu kolonuna da eklenen resmin dosya yolu kayıt edilir.
                    article.Image = ($"/images/articles/{guid}.jpg");

                    // Database e Article tablosuna yeni kullanıcı kaydedilir.

                    _articleRepository.Create(article);
                }
                return RedirectToAction("List");
            }
            // ToDo : Eğer validation dan geçmezse createArticleDTO nesnesinin List<Category> Categories propertysi boş gideceği için View içerisindeki select kısmı doldurulamayacak ve hata verecek.

            createArticleDTO.Categories = _categoryRepository.GetByDefaults
                (
                    selector: a => new GetCategoryDTO
                    {
                        ID = a.ID,
                        Name = a.Name
                    },
                     expression: a => a.Statu != Statu.Passive
                );

            return View(createArticleDTO);
        }

        // List
        public async Task<IActionResult> List()
        {
            // Veri tabanında kullanıcının kendi oluşturduğu makaleleri görmek için öncelikle aktif olan kullanıcı bulunur
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            AppUser user = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            // Article tablosunda sadece kullanıcının ID değeri tutulur. Makaleyi yazan kullanıcının FirstName gibi AppUser tablosunda tutulan verilerine ulaşmak için EAGER LOADING yönteminde tabloların INCLUDE edilmesi gerekmektedir. 

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
                expression: a => a.Statu != Statu.Passive && a.AppUserID == user.ID,
                // Article tablosu ile AppUser ve Category tablosunun birleştirildiği söylenir.
                include: a => a.Include(a => a.AppUser).Include(a => a.Category),
                // Sıralarken son oluşturulma tarihinden itibaren sırala dedik
                orderby: a => a.OrderByDescending(a => a.CreateDate)
                );

            return View(articleList);
        }

        // Update
        public IActionResult Update(int id)
        {
            // Güncellenecek olan makale bulunur.
            Article article = _articleRepository.GetDefault(a => a.ID == id);

            // Update için oluşturulan UpdateArticleDTO sınıfı AutoMapper kütüphanesi ile mapping yapılır. Burada Mapping sınıfında bu işlem söylenmesi unutulmamalıdır.
            UpdateArticleDTO updateArticleDTO = _mapper.Map<UpdateArticleDTO>(article);

            // UpdateArticleDTO içerisindeki List<GetCategoryDTO> tipindeki Categories isimli property doldurulur.
            updateArticleDTO.Categories = _categoryRepository.GetByDefaults
                (
                    selector: a => new GetCategoryDTO
                    {
                        ID = a.ID,
                        Name = a.Name
                    },
                     expression: a => a.Statu != Statu.Passive
                );
            return View(updateArticleDTO);
        }

        [HttpPost]
        public IActionResult Update(UpdateArticleDTO dto)
        {
            if (ModelState.IsValid)
            {
                Article article = _mapper.Map<Article>(dto);


                if (article.ImagePath != null)
                {
                    // Kullanıcı resim seçtiyse eski resim silinir.

                    string imageName = dto.Image.Trim().Substring(17); // Resmin adı bulunur

                    string deletedImage = Path.Combine(_webHostEnvironment.WebRootPath, "images", "articles", $"{imageName}"); // Dosya yolu oluşturulur.

                    if (System.IO.File.Exists(deletedImage))
                    {
                        System.IO.File.Delete(deletedImage); // Eğer dosya varsa silinir.
                    }

                    // Using anahtar kelimesi ile tanımlanan değişkenler işleri bittikten sonra ramdan kalkarlar.
                    using var image = Image.Load(dto.ImagePath.OpenReadStream());
                    image.Mutate(a => a.Resize(1000, 1000)); // Şekillendirme ve boyutlandırma işlemleri

                    // Fotoğraf kayıdı için dosya yolu söylenir. Dosya adı olarak eşsiz bir GUID ID verilerek kayıt edecek
                    Guid guid = Guid.NewGuid();

                    image.Save($"wwwroot/images/articles/{guid}.jpg");

                    // Veri tabanındaki AppUser tablosunun dosya yolu kolonuna da eklenen resmin dosya yolu kayıt edilir.
                    article.Image = ($"/images/articles/{guid}.jpg");



                }

                // resim yolu hidden type input ile viewdan alınmazsa resim değiştirilmediği zaman article.Image = ($"/images/articles/c5c1a558-8c0f-4f9d-8d9c-6101d9d75e84.jpg") ataması yapılması gerekir.

                _articleRepository.Update(article);
                return RedirectToAction("List");
            }
            // Kullanıcı validasyondan geçemezse tekrar Viewa dönerken DTO içerisindeki List<GetCategoryDTO> tipindeki ve Categories isimli property boş dönmesin
            dto.Categories = _categoryRepository.GetByDefaults
                (
                    selector: a => new GetCategoryDTO
                    {
                        ID = a.ID,
                        Name = a.Name
                    },
                     expression: a => a.Statu != Statu.Passive
                );

            return View(dto);
        }

        // Delete
        public IActionResult Delete(int id)
        {
            Article article = _articleRepository.GetDefault(a => a.ID == id);
            _articleRepository.Delete(article);
            return RedirectToAction("List");
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
         
    }
}
