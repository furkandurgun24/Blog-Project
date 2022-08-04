using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.Models.Entities.Concrrete;

namespace BlogProject.WEB.Areas.Member.Models.DTOs
{
    public class GetCategoryDTO
    {

        public int ID { get; set; }
        public string Name { get; set; }

    }
}
