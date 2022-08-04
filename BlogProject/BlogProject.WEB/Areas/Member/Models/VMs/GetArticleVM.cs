namespace BlogProject.WEB.Areas.Member.Models.VMs
{
    public class GetArticleVM
    {
        // Listeleme sayfasında Update - Delete işlemleri sırasında gerekli
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string UserFullName { get; set; }
        public string CategoryName { get; set; }
    }
}
