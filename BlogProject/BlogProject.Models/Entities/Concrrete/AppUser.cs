using BlogProject.Models.Entities.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogProject.Models.Entities.Concrrete
{
    public class AppUser : BaseEntity // Kullanıcılarında ID vb ortak özelliklerini alabilmek için BaseEntity den kalıtım alır.
    {

        // Class içerisindeki list yapıları kullanılabilmesi için List yapıları ctor içerisinde instance edilir.
        public AppUser()
        {
            Articles = new List<Article>();
            Comments = new List<Comment>();
            Likes = new List<Like>();
            UserFollowedCategories = new List<UserFollowedCategory>();
        }

        // AppUser a ait propertyler tanımlanır.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        // Kullanıcı güncelleme işlemleri sırasında son 3 şifre benzersiz olmalıdır.

        private string _oldPassword1 = null;

        public string? OldPassword1
        {
            get { return _oldPassword1; }
            set { _oldPassword1 = value; }
        }

        private string? _oldPassword2 = null;

        public string? OldPassword2
        {
            get { return _oldPassword2; }
            set { _oldPassword2 = value; }
        }

        private string? _oldPassword3 = null;

        public string? OldPassword3
        {
            get { return _oldPassword3; }
            set { _oldPassword3 = value; }
        }



        // Identity tarafıyla kullanıcıyı eşleştirmek için AppUser içinde IdentityId isimli property tanımlanır. Identity tarafı Id değerini GUID string paylaştığı için bu propertyde string tipinde tanımlanır.
        public string IdentityId { get; set; }

        // FullName property tanımlanır.
        public string FullName => $"{FirstName} {LastName}";

        // Profil fotoğrafı dosya yolu tutulacağı için string tipinde tutulur.
        public string Image { get; set; }

        [NotMapped] // Bu sınıf configure edilirken NOTMAPPED denirse bu property sql içerisinde kolon olarak ayağa kalkmaz.
        public IFormFile ImagePath { get; set; } // Dosya gönderme ve yükleme işlemlerini yapmak için IFormFile tipi kullanılır. Microsoft.AspNetCore.Http.Features kütüphanesi NuGet Package Manager dan kurulur.

        // Navigation Property. Eager Loading kullanılacaktır. Sorgular yazılırken Include kullanılarak yazılacaktır. 1 yazarın çokça makalesi olabilir. Bu durumdan dolayı 1 yazarın birçok makalesini tutmak için List<Article> tipinde veri tutulur. 1 makalenin ise yalnızca 1 yazarı olur. One To Many ilişki
        public List<Article> Articles { get; set; }

        // Navigation Property. Eager Loading kullanılacaktır. Sorgular yazılırken Include kullanılarak yazılacaktır. 1 yazarın çokça yorumu olabilir. Bu durumdan dolayı 1 yazarın birçok yorumunu tutmak için List<Comment> tipinde veri tutulur. 1 yorumun ise yalnızca 1 yazarı olur. One To Many ilişki
        public List<Comment> Comments { get; set; }

        // Navigation Property. Eager Loading kullanılacaktır. Sorgular yazılırken Include kullanılarak yazılacaktır. 1 yazarın çokça like i olabilir. Bu durumdan dolayı 1 yazarın birçok like tutmak için List<Like> tipinde veri tutulur. 1 like in ise yalnızca 1 yazarı olur. One To Many ilişki
        public List<Like> Likes { get; set; }

        // Navigation Property. Eager Loading kullanılacaktır. Sorgular yazılırken Include kullanılarak yazılacaktır. 1 yazarın çokça takip ettiği kategori i olabilir. Bu durumdan dolayı 1 yazarın birçok kategoriyi tutmak için List<UserFollowedCategory> tipinde veri tutulur. 1 kategori ise birçok yazar tarafından takip edilebilir. Many To Many ilişki. Çoka çok ilişkiden dolayı ara tablo oluşacaktır.
        public List<UserFollowedCategory> UserFollowedCategories { get; set; }
    }
}
