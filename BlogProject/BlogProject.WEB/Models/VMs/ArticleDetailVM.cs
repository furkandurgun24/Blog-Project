using BlogProject.Models.Entities.Concrrete;

namespace BlogProject.WEB.Models.VMs
{
    public class ArticleDetailVM
    {
        public int ArticleID { get; set; }
        public Article Article { get; set; }
        public string Mail { get; set; }
        public List<Category> userFollowedCategories { get; set; }
    

    }
}
