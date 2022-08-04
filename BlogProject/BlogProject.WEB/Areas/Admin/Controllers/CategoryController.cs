using AutoMapper;
using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Areas.Admin.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Kullanıcı yetkilendirmesine göre ulaşılabilir bir controller haline dönüştürülmüştür. Yani kullanıcının Rolü Member ise ulaşabilir. Bu attribute kullanıcı rolü tabanlı bir yetkilendirmedir.
    public class CategoryController : Controller
    {
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

        // List
        public async Task<IActionResult> List()
        {

            List<Category> model = _categoryRepository.GetDefaults(a => a.Statu != Statu.Passive);

            return View(model);
        }

        // Update
        public IActionResult Update(int id)
        {
            Category category = _categoryRepository.GetDefault(a => a.ID == id);

            // View a gönderilecek olan DTO nun doldurulması Mapper ile yapılır.
            var updateCategory = _mapper.Map<UpdateCategoryDTO>(category);
            return View(updateCategory);
        }

        // Update
        [HttpPost]
        public IActionResult Update(UpdateCategoryDTO updateCategoryDTO)
        {
            if (ModelState.IsValid)
            {
                // View dan doldurularak gelen bilgiler ile güncellenecek olan category i mapper ile doldururuz.
                Category category = _mapper.Map<Category>(updateCategoryDTO);
                // Güncellenen nesne repository içerisindeki update metoduna gönderilerek güncelleme yapılır
                _categoryRepository.Update(category);
                return RedirectToAction("List");
            }
            return View(updateCategoryDTO);
        }

        // Checklist
        public IActionResult CheckList()
        {
            List<Category> categories = _categoryRepository.GetDefaults(a => a.Statu == Statu.Passive && a.AdminCheck == AdminCheck.Waiting);
            return View(categories);
        }

        // Approve
        public IActionResult Approve(int id)
        {
            Category category = _categoryRepository.GetDefault(a => a.ID == id);
            _categoryRepository.Approve(category);
            return RedirectToAction("Checklist");
        }

        // Reject
        public IActionResult Reject(int id)
        {
            Category category = _categoryRepository.GetDefault(a => a.ID == id);
            _categoryRepository.Reject(category);
            return RedirectToAction("Checklist");
        }

        // Delete
        public IActionResult Delete(int id)
        {
            Category category = _categoryRepository.GetDefault(a => a.ID == id);
            _categoryRepository.Delete(category);
            return RedirectToAction("List");
        }
    }
}
