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

namespace BlogProject.WEB.Areas.Member.Controllers
{
    [Area("Member")] // Member Area nın controlleri olduğu belirtilir yazılmadığı taktirde controller isimleri çakışırsa multiendpoint hatası verir.
    [Authorize(Roles ="Member")] // Kullanıcı yetkilendirmesine göre ulaşılabilir bir controller haline dönüştürülmüştür. Yani kullanıcının Rolü Member ise ulaşabilir. Bu attribute kullanıcı rolü tabanlı bir yetkilendirmedir.

    public class AppUserController : Controller
    {
        // Bu controllerde kullanılacak olan Repositoryler ctor un giriş parametresi içerisinde çağrılır. SOLID in D prensibi gereği Constructor Injection yapılır. Program.cs içerisinde AddScope<InterfaceName,ClassName> şeklinde eklenmesi unutulmamalıdır.

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AppUserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAppUserRepository appUserRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment, IArticleRepository articleRepository, ICategoryRepository categoryRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appUserRepository = appUserRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
        }

        // Login sonrası kayıtlı kullanıcının Index sayfası
        public async Task<IActionResult> Index()
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
                      expression: a => a.Statu != Statu.Passive,
                      // Article tablosu ile AppUser ve Category tablosunun birleştirildiği söylenir.
                      include: a => a.Include(a => a.AppUser).Include(a => a.Category),
                      // Sıralarken son oluşturulma tarihinden itibaren sırala dedik
                      orderby: a => a.OrderByDescending(a => a.CreateDate)
               );
            // Oluşturulan bu articles nesnesinin 10 adedini al. Take metodu kullanıldığı için tekrardan ToList() metodu kullanılmak zorundadır.
            ViewBag.AllCategory = _categoryRepository.GetDefaults(a => a.Statu != Statu.Passive);
            return View(articles.Take(10).ToList());
        }

        // Logout. Kullanıcı çıkış işlemi sonucu Area sız başlangıç sayfasına yani Home/Index sayfasına yönlendirilir.
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        // Update
        public async Task<IActionResult> Update()
        {
            // İçerideki online kullanıcıyı getiriyor
            IdentityUser identityUser = await _userManager.GetUserAsync(User);

            // Girişi onaylanan kullanıcının AppUser tablosundaki ID si bulunur.
            AppUser user = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            //View a gönderilecek olan UpdateUserDTO auto mapper ile doldurulur.
            UpdateAppUserDTO updateAppUserDTO = _mapper.Map<UpdateAppUserDTO>(user);

            // Mail adresi AppUser içerisinde kolon olarak tutulmadığı için autoMapper ile eşleştirilemez onun için manuel atama yapılır.

            updateAppUserDTO.Mail = identityUser.Email;
            updateAppUserDTO.oldImage = user.Image;
            updateAppUserDTO.oldPassword = user.Password;

            // Eski şifreler dto içerisine gönderilir. View içerisinde hidden input ile saklanır. Bu sayede HTTPPOST sırasında automapper ile AppUser oluşturulmasında eski şifreler null olarak gelmez.
            updateAppUserDTO.oldPassword1 = user.OldPassword1;
            updateAppUserDTO.oldPassword2 = user.OldPassword2;
            updateAppUserDTO.oldPassword3 = user.OldPassword3;

            return View(updateAppUserDTO);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateAppUserDTO dto)
        {
            if (ModelState.IsValid)
            {
                // ToDo : Class burada AppUser oldUser = _appUserRepository.GetDefault(a => a.ID == dto.ID); çağrıldığı zaman update yaparken The instance of entity type 'AppUser' cannot be tracked because another instance with the same key value for {'ID'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.' hatası vermektedir. Bunu aşmak için UpdateAppUserDTO nesnesine oldImage ve oldPassword propertyleri eklendi.

                string oldPassword = dto.oldPassword;
                string oldImage = dto.oldImage;

                AppUser appUser = _mapper.Map<AppUser>(dto);

                List<string> last3Passwords = new List<string> { appUser.OldPassword1, appUser.OldPassword2, appUser.OldPassword3 };

                bool passwordResult = last3Passwords.Exists(a => a == dto.Password);

                IdentityUser identityUser = await _userManager.FindByIdAsync(dto.IdentityID);

                if (identityUser != null)
                {
                    if (passwordResult)
                    {
                        // Kullanıcı adı veya mail adresi sistemde kayıtlıysa hata mesajı döner.
                        TempData["Message"] = "Girdiğiniz şifre önceki 3 şifrenizden farklı olmalıdır";
                        return View(dto);
                    }

                    identityUser.Email = dto.Mail;
                    identityUser.UserName = appUser.UserName;

                    await _userManager.ChangePasswordAsync(identityUser, oldPassword, appUser.Password);
                    IdentityResult result = await _userManager.UpdateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        if (appUser.ImagePath != null)
                        {
                            // Kullanıcı resim seçtiyse eski resim silinir.

                            string imageName = oldImage + ".jpg"; // Resmin adı bulunur

                            string deletedImage = Path.Combine(_webHostEnvironment.WebRootPath, "images", "users", $"{imageName}"); // Dosya yolu oluşturulur.

                            if (System.IO.File.Exists(deletedImage))
                            {
                                System.IO.File.Delete(deletedImage); // Eğer dosya varsa silinir.
                            }

                            // Using anahtar kelimesi ile tanımlanan değişkenler işleri bittikten sonra ramdan kalkarlar.
                            using var image = Image.Load(dto.ImagePath.OpenReadStream());
                            image.Mutate(a => a.Resize(1000, 1000)); // Şekillendirme ve boyutlandırma işlemleri

                            image.Save($"wwwroot/images/users/{appUser.UserName}.jpg");

                            //Veri tabanındaki AppUser tablosunun dosya yolu kolonuna da eklenen resmin dosya yolu kayıt edilir.
                            appUser.Image = ($"/images/users/{appUser.UserName}.jpg");

                        }
                        appUser.OldPassword3 = appUser.OldPassword2;
                        appUser.OldPassword2 = appUser.OldPassword1;
                        appUser.OldPassword1 = appUser.Password;

                        _appUserRepository.Update(appUser);
                    }
                }

                return RedirectToAction("Index");
            }
            return View(dto);
        }

        // Detail
        public async Task<IActionResult> Detail(int id)
        {

            AppUser appUser = _appUserRepository.GetDefault(a => a.ID == id);
            IdentityUser identityUser = await _userManager.FindByIdAsync(appUser.IdentityId);

            List<Article> articleList = _articleRepository.GetDefaults(a => a.Statu != Statu.Passive && a.AppUserID == appUser.ID);

            List<Category> userFollowedCategories = _categoryRepository.GetCategoryWithUser(appUser.ID);

            GetAppUserProfileVM getProfileVM = new GetAppUserProfileVM()
            {
                FullName = appUser.FullName,
                Image = appUser.Image,
                Mail = identityUser.Email,
                Articles = articleList,
                Categories = userFollowedCategories,
            };

            // Eager loading olduğu için GetProfileVM içerisindeki List<Articles> listesinin categort elemanları foreach ile doldurulur
            foreach (var item in getProfileVM.Articles)
            {
                item.Category = _categoryRepository.GetDefault(a => a.Statu != Statu.Passive && a.ID == item.CategoryID);
            }


            return View(getProfileVM);

        }

        // Delete
        public async Task<IActionResult> Delete()
        {
            // Update.cshmtl içerisinde linki vardır
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);
            _appUserRepository.Delete(appUser);
            return Redirect("~/");
        }

        // User Self Detail Page
        public async Task<IActionResult> ActivatedUserDetail()
        {
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);


            List<Article> articleList = _articleRepository.GetDefaults(a => a.Statu != Statu.Passive && a.AppUserID == appUser.ID);

            List<Category> userFollowedCategories = _categoryRepository.GetCategoryWithUser(appUser.ID);

            GetAppUserProfileVM getProfileVM = new GetAppUserProfileVM()
            {
                FullName = appUser.FullName,
                Image = appUser.Image,
                Mail = identityUser.Email,
                Articles = articleList,
                Categories = userFollowedCategories,
            };

            // Eager loading olduğu için GetProfileVM içerisindeki List<Articles> listesinin categort elemanları foreach ile doldurulur
            foreach (var item in getProfileVM.Articles)
            {
                item.Category = _categoryRepository.GetDefault(a => a.Statu != Statu.Passive && a.ID == item.CategoryID);
            }

            // ToDo : Viem içerisi doldurulmadı. Detal Vm in kodları yazılacak.
            return View(getProfileVM);
        }

        // About
        public IActionResult About()
        {
            return View();
        }
    }
}
