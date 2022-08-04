using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Enums;
using BlogProject.WEB.Models.VMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.WEB.Views.Shared.Components.Articles
{
    [ViewComponent(Name = "Articles")] // Articles isminde bir ViewComponent olduğunu söyledik
    public class ArticlesViewComponent : ViewComponent // Bu sınıfın bir ViewComponent olduğunu ViewComponent sınıfından kalıtım alarak söyleriz.
    {
        // Oluşma tarihine göre güncel 10 makale gösterilecektir.
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ArticlesViewComponent(IArticleRepository articleRepository,ICategoryRepository categoryRepository)
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
        }
        public IViewComponentResult Invoke()
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
                        CategoryID=a.CategoryID,
                        ReadingTime=a.ReadingTime,
                        CreateDate=a.CreateDate,
                        ReadCounter=a.ReadCounter
                    },
                    // Expression parametresi ile Statu su passive olmayan  koşulu söylenir
                    expression: a => a.Statu !=  Statu.Passive,
                    // Article tablosu ile AppUser ve Category tablosunun birleştirildiği söylenir.
                    include: a=>a.Include(a=>a.AppUser).Include(a=>a.Category),
                    // Sıralarken son oluşturulma tarihinden itibaren sırala dedik
                    orderby: a=>a.OrderByDescending(a=>a.CreateDate)
                );
            // Oluşturulan bu articles nesnesinin 10 adedini al. Take metodu kullanıldığı için tekrardan ToList() metodu kullanılmak zorundadır.
            ViewBag.AllCategory = _categoryRepository.GetDefaults(a=>a.Statu!=Statu.Passive);
            return View(articles.Take(10).ToList());
        }
    }
}
