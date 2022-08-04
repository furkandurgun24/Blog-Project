using BlogProject.Models.Entities.Abstract;

namespace BlogProject.Models.Entities.Concrrete
{
    public class Category : BaseEntity // Categorylerinde ID vb ortak özelliklerini alabilmek için BaseEntity den kalıtım alır.
    {
        // Class içerisindeki list yapıları kullanılabilmesi için List yapıları ctor içerisinde instance edilir.
        public Category()
        {
            Articles = new List<Article>();
            UserFollowedCategories = new List<UserFollowedCategory>();
        }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation Property

        // 1 kategorinin in çokça makalesi olabilir. 1 makalenin 1 kategorisi olabilir.
        public List<Article> Articles { get; set; }

        // 1 kategorinin nin çokça takip edeni olabilir. 1 kullanıcının çokça takip ettiği kategori olabilir. Ara tablo kullanılacaktır.
        public List<UserFollowedCategory> UserFollowedCategories { get; set; }
    }
}