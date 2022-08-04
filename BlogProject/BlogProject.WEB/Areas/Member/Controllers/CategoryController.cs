using AutoMapper;
using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Areas.Member.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.WEB.Areas.Member.Controllers
{
    [Area("Member")] // Member Area nın controlleri olduğu belirtilir yazılmadığı taktirde controller isimleri çakışırsa multiendpoint hatası verir.
    [Authorize(Roles = "Member")] // Kullanıcı yetkilendirmesine göre ulaşılabilir bir controller haline dönüştürülmüştür. Yani kullanıcının Rolü Member ise ulaşabilir. Bu attribute kullanıcı rolü tabanlı bir yetkilendirmedir.
    public class CategoryController : Controller
    {
        // Bu controllerde kullanılacak olan Repositoryler ctor un giriş parametresi içerisinde çağrılır. SOLID in D prensibi gereği Constructor Injection yapılır. Program.cs içerisinde AddScope<InterfaceName,ClassName> şeklinde eklenmesi unutulmamalıdır.

        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUserFollowedCategoryRepository _userFollowedCategoryRepository;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper, UserManager<IdentityUser> userManager, IAppUserRepository appUserRepository, IUserFollowedCategoryRepository userFollowedCategoryRepository)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _userManager = userManager;
            _appUserRepository = appUserRepository;
            _userFollowedCategoryRepository = userFollowedCategoryRepository;
        }

        // Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryDTO createCategoryDTO)
        {
            if (ModelState.IsValid)
            {
                Category category = _mapper.Map<Category>(createCategoryDTO);
                _categoryRepository.Create(category);
                return RedirectToAction("List");
            }
            return View(createCategoryDTO);
        }

        // List
        public async Task<IActionResult> List()
        {
            // Aktif olan kullanıcıyı Identity kütüphanesinin UserManager sınıfının GetUserAsync metodu ile bulunur.
            IdentityUser identityUser = await _userManager.GetUserAsync(User);

            // Hangi kullanıcı aktifse onun IdentityID si ile AppUser tablosundaki kullanıcıya ulaşırız.
            AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            // Aktif kullanıcının takip ettiği UserFollowedCategory nesneleri liste haline getirilir.
            List<UserFollowedCategory> userFollowedCategories = _userFollowedCategoryRepository.GetFollowedCategories(a => a.AppUserID == appUser.ID);

            // Oluşturulan UserFollowedCategory listesi ViewBag içine atılır.
            ViewBag.list = userFollowedCategories;

            List<Category> model = _categoryRepository.GetDefaults(a => a.Statu != Statu.Passive);

            return View(model);
        }
         
        // Follow
        public async Task<IActionResult> Follow(int id)
        {
            Category category = _categoryRepository.GetDefault(a => a.ID == id);

            // Aktif olan kullanıcıyı Identity kütüphanesinin UserManager sınıfının GetUserAsync metodu ile bulunur.
            IdentityUser identityUser = await _userManager.GetUserAsync(User);

            // Hangi kullanıcı aktifse onun IdentityID si ile AppUser tablosundaki kullanıcıya ulaşırız.
            AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            // Kullanıcıların hangi kategorileri takip ettiğini tutan ara tabloya ekleme yapılır.

            // Category nesnesi içerisinde List<UserFollowedCategory> listesine ekleme yapılır. Bu ekleme yapıldığı zaman UserFollowedCategory tablosuna otomatik ekleme işlem yapılır.

            category.UserFollowedCategories.Add(new UserFollowedCategory
            {
                Category = category,
                CategoryID = category.ID,
                AppUser = appUser,
                AppUserID = appUser.ID
            });

            _categoryRepository.Update(category);
            return RedirectToAction("List");
        }

        // UnFollow
        public async Task<IActionResult> UnFollow(int id)
        {
            // Aktif olan kullanıcıyı Identity kütüphanesinin UserManager sınıfının GetUserAsync metodu ile bulunur.
            IdentityUser identityUser = await _userManager.GetUserAsync(User);

            // Hangi kullanıcı aktifse onun IdentityID si ile AppUser tablosundaki kullanıcıya ulaşırız.
            AppUser appUser = _appUserRepository.GetDefault(a => a.IdentityId == identityUser.Id);

            //Hangi kategori takipten çıkılmak istenir bulunur.
            Category category = _categoryRepository.GetDefault(a => a.ID == id);

            UserFollowedCategory userFollowedCategory = _userFollowedCategoryRepository.GetDefault(a => a.CategoryID == category.ID && a.AppUserID == appUser.ID);

            _userFollowedCategoryRepository.Delete(userFollowedCategory);

            return RedirectToAction("List");
        }

    }
}
