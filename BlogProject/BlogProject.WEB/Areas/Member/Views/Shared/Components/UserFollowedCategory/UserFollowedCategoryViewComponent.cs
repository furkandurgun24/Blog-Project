using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.WEB.Areas.Member.Views.Shared.Components.UserFollowCategory
{
    [ViewComponent(Name =("UserFollowedCategory"))]
    public class UserFollowedCategoryViewComponent : ViewComponent
    {
        // Aktif olan kullanıcının takip ettiği kategoriler gösterilecektir.

        private readonly ICategoryRepository _categoryRepository;      

        public UserFollowedCategoryViewComponent(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IViewComponentResult Invoke(int id)
        {
            List<Category> followedCategories = _categoryRepository.GetCategoryWithUser(id);
            return View(followedCategories);
        }
    }
}
