using System.ComponentModel.DataAnnotations;

namespace BlogProject.WEB.Areas.Admin.Models.DTOs
{
    public class UpdateCategoryDTO
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [MinLength(3, ErrorMessage = "En az 3 karakter giriniz"), MaxLength(50, ErrorMessage = "En fazla 50 karakter giriniz")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [MinLength(3, ErrorMessage = "En az 3 karakter giriniz"), MaxLength(250, ErrorMessage = "En fazla 250 karakter giriniz")]
        public string Description { get; set; }
    }
}
