using AutoMapper;
using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Models.DTOs;
using BlogProject.WEB.Models.VMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BlogProject.WEB.Controllers
{
    public class UserController : Controller
    {

        // Bu controllerde kullanılacak olan Repositoryler ctor un giriş parametresi içerisinde çağrılır. SOLID in D prensibi gereği Constructor Injection yapılır. Program.cs içerisinde AddScope<InterfaceName,ClassName> şeklinde eklenmesi unutulmamalıdır.

        private readonly IAppUserRepository _appUserRepository;
        private readonly UserManager<IdentityUser> _userManager; // Microsoft.AspNetCore.Identity kütüphanesinin kullanıcı işlemleri için kullanılan Repositoryleri içeren sınıftır.
        private readonly IMapper _mapper; // AutoMapper kütüphanesinin otomatik mapleme işlemleri için kullanılan Repositoryleri içeren sınıftır.
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UserController(IAppUserRepository appUserRepository, UserManager<IdentityUser> userManager, IMapper mapper, IArticleRepository articleRepository, ICategoryRepository categoryRepository)
        {
            _appUserRepository = appUserRepository; // Kendi AppUser işlemlerimiz için Repositoryleri tutan interfacedir.
            _userManager = userManager; // Microsoft.AspNetCore.Identity kütüphanesinin kullanıcı işlemleri için kullanılan Repositoryleri içeren sınıftır.
            _mapper = mapper;// AutoMapper kütüphanesinin otomatik mapleme işlemleri için kullanılan Repositoryleri içeren sınıftır.
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
        }

        // Create. Kullanıcı kayıt işlemi
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // metot içerisinde asenkron metot kullanıldığı için dönüş tipi Task<IActionResult> olarak döner ve async anahtar kelimesi kullanılır.
        public async Task<IActionResult> Create(CreateUserDTO createUserDTO)
        {
            // Önce kullanıcıdan alınan veriler ile Microsoft.AspNetCore.Identity  kütüphanesi sınıflarının metotları kullanılarak AspNetUsers tablosuna doğrulanmış kullanıcı eklenir. Eğer kullanıcı ekleme işlemi tamamnalnırsa, sonrasında kendi AppUsers tablosuna kullanıcı eklenir.
            if (ModelState.IsValid)
            {
                // Kullanıcı daha önce kayıt olmuş mu kontrol edilir.
                var checkUserName = _userManager.Users.Any(a => a.UserName == createUserDTO.UserName);
                var checkMail = _userManager.Users.Any(a => a.Email == createUserDTO.Mail);

                if (!checkMail && !checkUserName) // girdiği mail adresi ve username daha önce var mı ?
                {
                    // Yeni bir doğrulanmış kullanıcı oluşturulacağı için Microsoft.AspNetCore.Identity kütüphanesinin IdentityUser nesnesini oluşturarak yapacağız.
                    string newId = Guid.NewGuid().ToString();
                    IdentityUser identityUser = new IdentityUser()
                    {
                        Email = createUserDTO.Mail,
                        UserName = createUserDTO.UserName,
                        Id = newId
                    };

                    // Kullanıcı oluşturma işleminin baraşırı olması durumunu öğrenmek için IdentityResult nesnesi kullanılır. Ayrıca _userManager sınıfının Create işlemi için kullanılan CreateAstnc metodu asenkron olarak çalıştığı için await anahtar kelimesi kullanılır. Asenkron metotlar için Actionlar Task<IActionResult> tipinde ve async anahtar kelimesi ile yazılırlar.
                    IdentityResult result = await _userManager.CreateAsync(identityUser, createUserDTO.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(identityUser, "Member"); // Eğer kullanıcı kayıdı gerçekleşirse kullanıcının rolü Member olarak atanır.

                        // AutoMapper kütüphanesinin repoları kullanılarak automapper işlemi yapılacak. IMapper interfacei ctor un giriş parametresinde tanımlanır.
                        var user = _mapper.Map<AppUser>(createUserDTO);

                        // CreateUserDTO içerisinde IdentityId isimli bir property olmadığı için AppUsers tablosundaki Identity kolonunu doldurmak için manuel atama yapılır.
                        user.IdentityId = newId;

                        // Fotoğraf işlemleri için ImagePath boş değilse yani kullanıcı dosya yüklediyse fotoğraf uygulamaları yapılacaktır.

                        if (createUserDTO.ImagePath != null)
                        {
                            // Using anahtar kelimesi ile tanımlanan değişkenler işleri bittikten sonra ramdan kalkarlar.
                            using var image = Image.Load(createUserDTO.ImagePath.OpenReadStream());
                            image.Mutate(a => a.Resize(500, 500)); // Şekillendirme ve boyutlandırma işlemleri
                                                                   // Fotoğraf kayıdı için dosya yolu söylenir. Dosya adı olarak kullanıcıAdı.jpg olarak kayıt edecek
                            image.Save($"wwwroot/images/users/{user.UserName}.jpg");
                            // Veri tabanındaki AppUser tablosunun dosya yolu kolonuna da eklenen resmin dosya yolu kayıt edilir.
                            user.Image = ($"/images/users/{user.UserName}.jpg");

                            // Database e AppUsers tablosuna yeni kullanıcı kaydedilir.

                            _appUserRepository.Create(user);
                            return RedirectToAction("Login", "Home"); // İlk kez kayıt olan kullanıcıyı login sayfasına yönlendir.
                        }
                    }
                }
                else
                {
                    // Kullanıcı adı veya mail adresi sistemde kayıtlıysa hata mesajı döner.
                    TempData["Message"] = "Bu mail adresi veya kullanıcı adı daha önce alınmıştır.";
                }
            }
            return View(createUserDTO); // Kullanıcı kayıt olamazsa kayıt sayfasına tekrardan dönderilecektir.
        }

        // Detail ArticleList Of User
        public async Task<IActionResult> Detail(int id)
        {
            AppUser appUser = _appUserRepository.GetDefault(a => a.ID == id);
            IdentityUser identityUser = await _userManager.FindByIdAsync(appUser.IdentityId);

            List<Article> articleList = _articleRepository.GetDefaults(a => a.Statu != Statu.Passive && a.AppUserID == appUser.ID);

            List<Category> userFollowedCategories = _categoryRepository.GetCategoryWithUser(appUser.ID);

            GetProfileVM getProfileVM = new GetProfileVM()
            {
                FullName = appUser.FullName,
                Image = appUser.Image,
                Mail = identityUser.Email,
                Articles = articleList,
                Categories = userFollowedCategories,
            };

            // Eager loading olduğu için GetProfileVM içerisindeki List<Articles> listesinin category elemanları foreach ile doldurulur
            foreach (var item in getProfileVM.Articles)
            {
                item.Category = _categoryRepository.GetDefault(a => a.Statu != Statu.Passive && a.ID == item.CategoryID);
            }

            // ToDo : Viem içerisi doldurulmadı. Detal Vm in kodları yazılacak.
            return View(getProfileVM);
        }
    }
}
