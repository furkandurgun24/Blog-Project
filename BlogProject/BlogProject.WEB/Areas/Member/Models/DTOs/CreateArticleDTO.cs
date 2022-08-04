using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProject.WEB.Areas.Member.Models.DTOs
{
    public class CreateArticleDTO
    {
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [MinLength(3, ErrorMessage = "En az 3 karakter giriniz"), MaxLength(100, ErrorMessage = "En fazla 100 karakter giriniz")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [MinLength(3, ErrorMessage = "En az 3 karakter giriniz"), MaxLength(600, ErrorMessage = "En fazla 600 karakter giriniz")]
        public string Content { get; set; }

        public string? Image { get; set; }

        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [NotMapped]
        public IFormFile ImagePath { get; set; }

        [Required(ErrorMessage ="Bu alan boş bırakılamaz")]
        public int CategoryID { get; set; }

        // Mapping işleminde kullanmak için AppUserID propertysi eklenir.
        public int AppUserID { get; set; }

        // Kullaınıcı Makale oluştururken dropdown dan seçim yapması için liste gönderilecek. Ve kategorinin bazı proplarını göndereceğimiz için List<GetCategoryDTO> tipinin sınıfı ayrıca oluşturulur.
        public List<GetCategoryDTO>? Categories { get; set; }

    }
}
