using AutoMapper;
using BlogProject.Models.Entities.Concrrete;
using BlogProject.WEB.Areas.Member.Models.DTOs;

namespace BlogProject.WEB.Areas.Member.Models.Mappers
{
    public class Mapping : Profile // AutoMapper kütüphanesinin Profile sınıfından kalıtım alarak otomatik map işlemi yapabilir.
    {
        // Constructor metot içerisinde Mapping atamaları yapılır. Bunun için CreateMap<TSource,TDestination> metodu kullanılır.
        public Mapping()
        {
            // ReverseMap() metodu kullanılarak işlemin her iki tarafa da yapılacağı söylenir. ReverseMap yazdığımız için Source ve Destination nesnelerinin yerleri değiştirilir. 
            // Auto mapping işlemi sırasında hariç tutulması istenen propertyler varsa DoNotValidate metodu ve lambda syntax kullanılarak bu işlem uygulanır

            // Category Controller için gerekli Mapping
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
        
            // Article Controller için gerekli Mapping
            CreateMap<Article, CreateArticleDTO>().ReverseMap();
            CreateMap<UpdateArticleDTO,Article >().ReverseMap();

            // AppUser Controller için gerekli Mapping
            CreateMap<UpdateAppUserDTO, AppUser>().ReverseMap();


        }
    }
}
