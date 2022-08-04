--------- BLOG PROJESI ----------

1 - Proje Blank Solution olarak a��l�r.

2 - Solution alt�na ilk olarak entitylerin tutulaca�� Class Library projesi SolutionName.Models ismiyle olu�turulur.

3 - Models katman� i�erisine Entities klas�r� onun alt�na da Abstract ve Concrete isimli klas�rlerimiz olu�turulur. Uygulamada kullan�lacak olan soyut ve temel entity classlar� olu�turulur.

4 - Models katman� alt�na Enums klas�r� a��l�r. Gerekli olacak Enumlar burada tutulur.

5 - Models katman� alt�na EntityTypeConfiguration isimli klas�r a��l�r. A��lan bu klas�r�n alt�na yine Abstract ve Concrete isimli klas�rlerde olu�turulur. Bu klas�rlerde soyut ve temel entityler i�in validation configuration (veritaban� ili�kileri / tablo - kolon �zellikleri) lar�n yaz�laca�� s�n�flar olu�turulur.

6 - Models => Entities => Abstract klas�r� alt�na BaseEntity isimli public Abstract t�r�nde class olu�turulur. Bu class�n i�erisinde propertyler tan�mlan�r.

NOT 1 =>  Database tablolar�nda bir tablonun kolonlar� sadece di�er tablolardan gelen Foreign Key lerden olu�uyorsa o tablo i�in ayr�yeten bir PRIMARY ID kolonuna ihtiya� yoktur. E�er o tablo kendine has en az 1 adet kolonu varsa o tablo i�in PRIMARY KEY kolonuna ihtiya� vard�r.

7 - Models => Enums alt�na Statu isimli public Enum tipinde Enum olu�turulur.

8 - Models => Entities => Concrete klas�r� alt�na AppUser isimli public class olu�turulur. BaseEntity class�ndan kal�t�m al�r. Burada dosya i�lemlerini kullanmak i�in NuGet Package Manager dan Microsoft.AspNetCore.Http.Features k�t�phanesi kurulmal�d�r.

9 - Models => Entities => Concrete klas�r� alt�na Article isimli public class olu�turulur. BaseEntity class�ndan kal�t�m al�r. Burada dosya i�lemlerini kullanmak i�in NuGet Package Manager dan Microsoft.AspNetCore.Http.Features k�t�phanesi kurulmal�d�r.

10 - Models => Entities => Concrete klas�r� alt�na Comment isimli public class olu�turulur. BaseEntity class�ndan kal�t�m al�r.

11 - Models => Entities => Concrete klas�r� alt�na Like isimli public class olu�turulur. BaseEntity class�ndan kal�t�m ALMAZ!!! ID almad��� i�in AppUserID ve ArticleID composite key olacakt�r. Yani tekrarlar yap�lamayacakt�r. Ayr�ca bu tablo sadece di�er tablolardan ald��� Foreing Key lerden ile olu�tu�u i�in yani kendine has bir kolonu olmad��� i�in Primary Key kolonuna ihtiya� duymaz.

12 - Models => Entities => Concrete klas�r� alt�na UserFollowedCategory isimli ara tablo olan public class olu�turulur. BaseEntity class�ndan kal�t�m ALMAZ!!! ID almad��� i�in AppUserID ve CategoryID composite key olacakt�r. Yani tekrarlar yap�lamayacakt�r. Ara tablolarda ayr�yeten bir PRIMARY KEY tan�mlanmas�na gerek yoktur. Ayr�ca bu tablo sadece di�er tablolardan ald��� Foreing Key lerden ile olu�tu�u i�in yani kendine has bir kolonu olmad��� i�in Primary Key kolonuna ihtiya� duymaz.

13 - Models => Entities => Concrete klas�r� alt�na Category isimli public class olu�turulur. BaseEntity class�ndan kal�t�m al�r. 

14 - Models => EntityTypeConfiguration => Abstract klas�r� alt�na BaseMap isimli public Abstract t�r�nde class olu�turulur. Bu class�n i�erisinde propertyler i�in configurationlar tan�mlan�r. Di�er classlara kal�t�m verirken kullanmak i�in GenericType<T> tipinde ve IEntityTypeConfiguration<T> interface inden kal�t�m al�r. Bu interface i kullnamk i�in EF.Core ve EF.Core.SqlServer k�t�phaneleri NuGet Package Managerdan kurulur.

NOT 2 => Hernagi bir database nesnesi olu�turulurken di�er nesnelere olan ba��ml�l�klar g�z �n�ne al�narak olu�turulmal�d�r. Bu projede herhangi bir makale nesnesi olu�turabilmek i�in Kullan�c� ve kategori olmal�d�r. Bu durumdan dolay� di�er tablolara ba��ml� olmayan kategori yada kullan�c� nesneleri ilk olarak olu�turulabilir.

15 - Models => EntityTypeConfiguration => Concrete klas�r� alt�na Concrete entitiy lerin propertylerinin configurationlar�n� tan�mlamak i�in o EntityNameMap ad�nda public t�rde classlar a��l�r. A��lan bu classlar ayn� Entity k�sm�nda oldu�u gibi kal�t�m alanlar i�in BaseMap class�ndan kal�t�m al�rlar. BaseMapten kal�t�m almayanlar IEntityTypeConfiguration<EntityName> �eklinde kal�t�m al�rlar. 

NOT 3 => Primary key almadan olu�turulacak olan entity ler i�in tan�mlanacak olan EntityTypeConfiguration class lar�n�n i�inde builder.HasKey(a => new { a.ForeignKey, a.ForeignKey }); tan�mlanmas� gerekmektedir. Bu tan�mlanma yap�lmazsa migration i�leminde relationship hatas� al�nacakt�r.

NOT 4 => Navigation propertyler entity i�erisinde tan�mlan�rken ili�kili olan b�t�n propertylerde tan�mlanmas� gerekmektedir. Lakin bu navigation propertyler configuration s�n�f�nda a��klan�rken ili�ki tablolar�n sadece birinde a��klanmas� yeterlidir.

16 - Kullan�c� olu�tururken o kullan�c�n�n Rollerinin de atanabilmesi i�in Identity k�t�phanesi �ncesinde olu�trulmal�d�r. Bunun i�in database aya�a kalkerken Roller haz�r olarak gelmelidir. Models => EntityTypeConfiguration => Concrete alt�na IdentityRoleMap public class� olu�turulur. Bu class IEntityTypeConfiguration<IdentityRole> �eklinde kal�t�m al�r. Burada IdentityRole s�n�f� Microsoft.AspNetCore.Identity k�t�phanesinden gelen haz�r s�n�ft�r. Burada seed data olarak Rolleri haz�r olarak getirece�iz. Bunun i�in Roller, Configure metodu i�erisinde tan�mlan�r. Member ve Admin isimli sadece 2 adet Rol vard�r.

*************************************

17 - DataAccessLayer olarak adland�r�lan Repositoryler ve Context s�n�f�n� i�eren katman olu�turulur.

18 - DataAccess katman� olu�turmak i�in Solution alt�na Class Library projesi SolutionName.DAL ismiyle olu�turulur.

19 - SolutionName.DAL alt�na Repositories klas�r� olu�turulur. Olu�turulan bu klas�r alt�na Abstract, Concrete ve Interfaces isimli alt klas�rler olu�turulur. Abstract ve Concrete klas�rleri alt�nda ise bu metot imzalar�n�n g�vdeleri yaz�lacakt�r.

20 - Interfaces klas�r�n�n alt�na da Abstract ve Concrete diye 2 klas�r a��l�r. Interface klas�r�n�n alt klas�rleri i�erisinde metotlar�n imzalar�n� i�eren Interfaceler tan�mlanacakt�r

20 - SolutionName.DAL alt�na Context klas�r� olu�turulur. Bu klas�r i�erisinde database ba�lant�s�n� sa�layacak olan Context s�n�f� olu�turulacakt�r. public tipinde ProjectContext isimli class olu�turulur. Bu s�n�f IdentityDbContext s�n�f�ndan kal�t�m al�r. Bu kal�t�m� verebilmek i�in Microsoft.AspNetCore.Identity.EntityFrameworkCore k�t�phanesi indirilmelidir.

21 - �htiya� duyulmas� halinde Models katman�ndan Reference al�n�r.

22 - DAL => Interfaces => Abstract klas�r� alt�na public tipte IBaseRepository isimli GenericType olacak �ekilde bir interface tan�mlan�r. Bu interface i�erisinde CRUD i�lemleri i�in metotlar�n imzalar� yaz�l�r.

23 - DAL => Interfaces => Concrete klas�r� alt�na public tipte her entity isminde interface olu�turulur. Bu interfaceler IBaseRepository<EntityName> interface inden GenericType olarak kal�t�m al�r. ILikeRepository ve IUserFollowedCategory, IBaseRepository den kal�t�m almaz. ILikeRepository ve IUserFollowedCategory interface in metot imzalar� ayr�ca yaz�l�r.

NOT 5 => Kullan�c� do�rulama ve yetki i�lemleri gibi metotlar i�in Identity k�t�phanesinin build in it metotlar� kullan�lacakt�r.

24 - DAL => Repositories alt�nda bulunan Abstract ve Concrete klas�rleri i�erisinde interfacelerde imzalar� at�lan metotlar�n g�vdeleri yaz�lacakt�r.

25 - DAL => Repositories => Abstract alt�nda BaseRepository isimli public abstract s�n�f tan�mlanarak IBaseRepository interface inde ki imzalanan metotlar�n g�vdeleri yaz�l�r. Bu noktada CRUD i�lemleri database e ba�lant� yap�larak yap�laca�� i�in CTOR i�erisinde context s�n�f� giri� parametresi olarak al�narak ba�lanto sa�lan�r.

NOT 6 => LINQ sorgular� yazarken i�lem s�ralamas� �nemlidir. Yani �nce tablolar birle�tirilir, Gruplan�r, sorgulan�r, s�ralan�r ve se�im yap�larak sonu� d�nd�r�l�r.

26 - DAL =>Repositories => Concrete alt�nda a��lan repository classlar� ihtiya� varsa BaseRepositoryden ve kendi repository interfaceinden kal�t�m al�r. BaseRepository ata s�n�f�n�n CTOR u ProjectContext s�n�f�n� parametre olarak aya�a kalkt��� i�in Context repository s�n�flar�n�n CTOR unun parametresinde ProjextContext s�n�f� yaz�l�r ve :base(context) anahtar kelimesiyle atas�na context g�nderilir. Construct Chain i�lemi denir.

27 - UserFollowedCategoryRepository s�n�f� yaz�lmak zorunda de�ildir. UserFollowedCategory ara tablo oldu�u i�in ihtiya� olmayabilir.

*************************************

28 - Pressentation olarak adland�r�lan kullan�c� ile ileti�ime ge�ilen katman olu�turulur.

29 - Presentation katman� olu�turmak i�in Solution alt�na Asp.Net Core WebApp(MVC) projesi SolutionName.WEB ismiyle olu�turulur.

30 - Migration i�lemleri burada yap�laca�� i�in Program.cs ve appsetting.js i�erisine database i�in ConnectionString ifadesi yaz�l�r. builder.services

31 - IOC pattern deseni CORE i�in kullan�l�r. AddScope denerek bu durum tan�mlan�r. Yani kodu yazarken interface olarak yazd���m�z halde kendisi repository s�n�f�nda g�vdeleri olan metotlar� tan�yacakt�r. builder.Services.AddScoped<InterfaceName, ClassName>();

31 - Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Tools Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.SqlServer k�t�phanelerine ihtiya� vard�r.

32 - migration ve update-database i�lemleri i�in NuGet Package Manager console daki default project : context s�n�f�n� i�eren katman olarak se�ilmelidir. Bu i�lem yap�l�rken WEB s�n�f�n� i�eren katmanda solution startup project olarak se�ilmelidir.

33 - Migration i�lemi i�in EfCore.Desing paketi kullan�l�r.

34 - add-migration migrationName �eklinde migration i�lemi yap�l�r. Sonras�nda ise update-database i�lemi ile entityler ve entityTypeConfiguration lara g�re database i�erisinde tablolar olu�turulur.

35 - WEB alt�na Areas klas�r� a��larak tan�mlanacak olan area lar bu klas�r alt�na tan�mlan�r.

36 - Program.cs i�erisine app.MapControllerRoute eklenerek arealar i�in endpointler yaz�l�r. 

37 - WEB => Areas klas�r�ne Member isimli area eklenir. MVC ana dizinindeki View klas�r� kopyalanarak Areas => Member klas�r�ne yap��t�r�l�r.

NOT 7 => Area lar i�in olu�turulan Contollerin �st�ne [Area('AreaName')] attribute eklenmelidir.

38 - Program.cs i�erisine Identity k�t�phanesi �zelliklerini kullanmak i�in builder.Services.AddIdentity komutu eklenir.

39 - Kay�ts�z kullan�c� i�lemleri i�in WEB katman� ana dizinindeki controller i�erisine UserController isimli controller eklenir.

40 - Tek bir entity nin belirli propertylerini kullanmak veya o propertylere validation eklenmesi s�ras�nda kullan�lan s�n�flara DataTransferObject (DTO) denir.

41 - Bir �ok s�n�f�n propertylerii kullanarak i�lem yap�l�rken kullan�lan s�n�fa ViewModel (VM) denir.

42 - WEB => Models alt�na DTOs isimli klas�r olu�turulur. Kullan�c� kay�t i�lemleri s�ras�nda kullan�c�ya sadece AppUser Entity s�n�f�n�n belirli propertylerini g�sterece�im i�in DTO olu�turulur. Olu�turulan DTOs klas�r� alt�na CreateUserDTO isimli s�n�f olu�turulur.

43 - CreateUserDTO s�n�f� i�erisine i�lem kullan�lmak istenen propertyler, Entity i�erisindeki ayn� isimlendirme ile eklenir ve istenirse validationlar yaz�l�r. Bu sayede hem developer rahat eder hemde otomatik map i�lemi i�in k�t�phane kullan�labilir. Propertyler i�in VALIDATION i�lemleri yaz�l�r.

44 - Create i�leminin [HTTPPPOST] k�sm�nda IActionResult metodunun giri� parametresi olarak CreateUserDTO olarak verilir. ��nk� kullan�c� sadece DTO i�erisindeki propertyleri �nce dolduracak ve bizde doldurulmu� olan DTO yu kullnarak �ye kayd� olu�turaca��z.

45 - Yeni kullan�c� olu�turma i�lemi Microsoft.AspNetCore.Identity k�t�phanesi ile yap�lacakt�r. Yeni kullan�c� do�rulanm�� bir kullan�c� oldu�u i�in Microsoft.AspNetCore.Identity k�t�phanesinin IdentityUser s�n�f�ndan bir nesne new lenerek olu�turulur. Bu olu�turulacak nesnesin propertylerine DTO dan al�nan veriler atanacakt�r. Bu i�lem i�in instance yap�l�rken parametreli constructor kullan�larak yap�labilir.

NOT 8 => Yeni kullan�c�n�n yani AppUser tablosunun ID de�eri AspNetUsers tablosunun ID de�erine atanmaz. ��nk� AppUser i�in ID de�erini SQL kay�t yaparsa verecektir. Bu durumda kullan�c� �nce database e eklenip daha sonra AspNetUsers olu�turulmaya  �al��aca�� i�in hata verecektir. Bu iki tablo birbirini tan�yacak lakin ili�kisi olmayacakt�r. AppUser tablosuna yeni kullan�c� eklenip id de�eri al�n�nca ve bu id AspNetUsers tablosunun validation lar�ndna ge�mezse yeni kullan�c� AppUser tablosuna eklenir lakin AspNetUsers tablosuna eklenmez bu durumu engellemek i�in AppUser tablosunda ayr�ca bir IdentityID kolonu vard�r ve bu de�er GUID olarak atan�r.

46 - Otomatik Map i�lemi i�in AutoMapper k�t�phanesi WEB katman�na eklenir. Ayr�ca AutoMapper.Extensions.Microsoft.Depen.... k�t�phaneside eklenir.

47 - Map leme i�lemleri i�in WEB => Models alt�na Mappers klas�r� ve i�erisine Mapping isimli class olu�turulur. Bu class AutoMapper k�t�phanesinin Profile class�ndan kal�t�m al�r. Ayr�ca Program.cs i�erisinde de builder.services.AddAutoMapper() komutu eklenecektir.

48 - Create i�erisinde automapping i�lemi yapmak i�in AutoMapper k�t�phanesinin repolar�na ihtiya� vard�r. Bu repolar� kullanmak i�in Controller in ctor unda IMapper s�n�f� giri� parametresi olarak tan�mlan�r.

49 - Foto�raf i�lemleri SixLabors.ImageSharp ve SixLabors.ImageSharp.Web k�t�phaneleri kullan�lacakt�r.

50 - Foto�raf�n dosya yollar� sadece database e kaydedilecek olup dosyalar wwwroot alt�na olu�turulan images klas�r�ne kay�t edilecektir.

51 - UserController un Create actionuna Empty View eklenir.

52 - AdminLTE nin index3 i�erisinde yer alan Form/General Elements alt�ndaki Quick Example isimli k�sm� inspect k�sm�ndan kopyalat�p bo� Viewa yap��t�r�l�r. Burada post i�lemi kullan�laca�� i�in asp-action vb attribute ler eklenir.

53 - DTO lar� vb nesneleri View i�erisinde Model olarak kullanmak i�in nesne yollar�n� View => ControllerName alt�ndaki _ViewImports.cshtml dosyas�na eklenir.

54 - Olu�turulan bo� View i�erisine model olarak CreateUserDTO nesnesi eklenir. Bu sayede kullan�c�dan al�nan veriler CreateUserDTO nun propertylerine atanabilir.

55 - Login i�lemi Home Controller i�erisinde yap�l�r. Login i�lemi i�in Identity k�t�phanesi kullan�lacakt�r. Bunun i�in ctor i�erisinde k�t�phanenin UserManager ve SignInManager s�n�flar�n�n metotlar� kullan�lmak i�in tan�mlan�r.

56 - Kullan�c� giri�i i�in E-mail adresi ve �ifre kullan�lacakt�r. Bunun i�in LoginDTO s�n�f� olu�turulur. Bu s�n�f i�erisinde sadece mail ve �ifre propertyleri olur.

57 - DTO ile al�nan bilgiler ile �nce mail adresi var m� ? Varsa �ifresi do�ru mu �eklinde s�ras�yla kontrol edilir ve do�ruysa kullan�c�n�n rol�ne g�re area ya y�nlendirilir.

NOT 9 => Cookie ler i�in Program.cs dosyas�na gerekli kodlar�n eklenmesi gerekmektedir. Hem do�rulama hemde yetkilendirme kodlar� eklenir. builder.service ve app. olarak

58 - �yelerin kullanabilece�i �ye paneli olu�turulacakt�r. Bunun i�in �ncelikle layout haz�rlan�r. AdminLte 3 layoutu ve k�t�phaneleri kullan�lacakt�r. Area => Member _Layout g�ncellenecektir. Burada cs ve script dosya yollar� d�zenlenir.

59 - Areas => Member => Controller alt�na AppUserController isimli controller eklenir ve [Area("Member")] attribute namespace alt�na tan�mlan�r. Bu sayede ayn� controller name olmas�na ra�men hangi area i�in �al��aca��n� bilir.

60 - Bu controller alt�nda kay�tl� kullan�c� i�in yap�labilecek olan actionlar tan�mlan�r.

61 - Layoutta kullan�c� bilgilerinin ve resminin g�r�nece�i k�sma @RenderSection olu�turulur.

62 - AppUserController in Index actionu i�in View olu�turulur. Bu Viewda AppUser tipindeki s�n�f� model olarak kullan�l�r _ViewImports i�inde AppUser s�n�f�n�n yolu tan�mlan�r. Layout i�erisindeki RenderSection b�l�m� i�in section olu�turulur.

63 - Category olu�turmak i�in Areas => Member alt�nda CategoryController isimli controller olu�turulur ve i�erisine CRUD i�lemleri i�in actionlar olu�turulur.

64 - Create ve Update i�lemlerinde DTO lar kullan�l�r. Kategori DTO su Areas => Member => Models => DTOs klas�r� olu�turulur. Bu klas�r alt�na gerekli olan DTO lar tan�mlan�r.

65 - Auto mapper i�lemi yapmak i�in Areas => Member => Models => Mappers klas�r� olu�turulur. Bu klas�r alt�na Mapping s�n�f� olu�turulur ve gerekli mapping i�lemleri yaz�l�r.

66 - Update action yaz�l�rken UpdateCategoryDTO s�n�f� olu�turulur. Bu s�n�f Update in View � i�erisinde model olarak g�nderilip Update in [HTTPPOST] k�sm�nda giri� parametresi olarak kullan�lacakt�r.

67 - Update i�lemleri i�in mapper s�n�f�na ekleme yap�l�r.

68 - 63 - Article olu�turmak i�in Areas => Member alt�nda ArticleController isimli controller olu�turulur ve i�erisine CRUD i�lemleri i�in actionlar olu�turulur.

69 - Create ve Update i�lemlerinde DTO lar kullan�l�r. Kategori DTO su Areas => Member => Models => DTOs klas�r� olu�turulur. Bu klas�r alt�na gerekli olan DTO lar tan�mlan�r.

70 - Auto mapper i�lemi yapmak i�in Areas => Member => Models => Mappers klas�r� olu�turulur. Bu klas�r alt�na Mapping s�n�f� olu�turulur ve gerekli mapping i�lemleri yaz�l�r.

NOT 10 => Article List i�leminde, Article tablosunda sadece kullan�c�n�n ID de�eri tutulur. Makaleyi yazan kullan�c�n�n FirstName gibi AppUser tablosunda tutulan verilerine ula�mak i�in EAGER LOADING y�nteminde tablolar�n INCLUDE edilmesi gerekmektedir. 

NOT 11 => Birden fazla entity i�indeki propertyler kullan�larak olu�turulan s�n�flar i�in ViewModel yani VM olu�turulur. 

72 - Areas => Models klas�r� alt�na VMs klas�r� alt�na GetArticleVM isimli VM s�n�f� a��l�r.

73 - Update action yaz�l�rken UpdateArticleDTO s�n�f� olu�turulur. Bu s�n�f Update in View � i�erisinde model olarak g�nderilip Update in [HTTPPOST] k�sm�nda giri� parametresi olarak kullan�lacakt�r.

74 - Update i�lemleri i�in mapper s�n�f�na ekleme yap�l�r.

NOT 12 => Birbirine kom�u olmayan tablolar� ba�lamak i�in ThenInclude(a=>a.Entity) metodu kullan�l�r. Bu metot en son yaz�lan Include() metodu baz al�narak kullan�l�r. Article tablosundan include(a=>a.Category).ThenInclude(a=>a.UserFollowedCategory) gibi

75 - Ortak yerlerde g�sterilecek Viewlar i�in Component olu�turulur. Bunun i�in Global alanda Views => Shared klas�r�n�n alt�na Components isimli klas�r olu�turulur ve Componentler bu klas�rler alt�nda kendine has klas�r isimleri ile birlikte olu�turulur.

76 - Component klas�r� alt�ndaki Articles klas�r� i�erisine ArticlesViewComponent isminde s�n�f eklenir. Bu s�n�f ViewComponent s�n�f�ndan kal�t�m al�r. Ayr�ca namespace alt�na  [ViewComponent(Name ="Articles")] attribute eklenir. Bu VM ile ana sayfadaki makalelerin Card lar� olu�turulacakt�r.

77 - ViewComponentleri tetiklemek i�in Invoke metodu kullan�l�r.

78 - ViewComponent i�erisinde farkl� s�n�flar�n propertyleri al�narak bir VM g�sterilece�i i�in Global alanda Models klas�r� alt�na VMs klas�r� a��l�r ve global alan VM leri burada tan�mlan�r.

79 - Componentlerin Viewlar� kendi Kendi klas�rleri alt�nda olu�turulmak zorundad�r. �lk olu�turuan Component View i nin ad� Default olmal�d�r. Bu View in model k�sm�na List<GetArticleWithUserVM> atan�r.

80 - Anasayfan�n View �na olu�turulan Default View g�nderilir. Bu i�lem i�in Anasayfa Viewinda @await Component.InvokeAsync("ViewComponentName") metodu �a�r�l�r.

81 - Kay�tl� kullan�c�da giri� yapt��� zaman Member area s�n�n ana sayfas�nda bu ViewComponent� g�rmesi i�in AppUser alt�ndaki Index View �na @await Component.InvokeAsync("ViewComponentName") eklenir.

82 - Areas i�erisinde de Member �zel ViewComponent yapmak i�in ayn� i�lemler izlenir.

NOT 13 => IWebHostEnvironment interface�i, Asp.NET Core mimarisi dahilinde gelen ve projenin bulundu�u sunucu hakk�nda bizlere gerekli ortamsal bilgileri getirecek alanlar� bar�nd�ran bir yap�ya sahiptir. Bu interfacei controller i�erisinde constructor injection yaparak kullan�l�r�z. Builder Design Pattern kullan�larak olu�turulmu� olan ve genellikle yeni Asp.net Core projesi olu�turdu�unuzda Program.cs yada Startup.cs i�erisinde WebHostBuilder s�n�f� kullan�larak projenin belli ba�l� �zellikleri ayarlanarak aya�a kald�r�l�r ve UseContentRoot ad�nda bir metodunun oldu�unu g�receksiniz bu metod ile ContentRootPath�deki yolu kendinize g�re ki�iselle�tirebilirsiniz.

NOT 14 => Member/AppUser/Update i�erisinde. Class burada AppUser oldUser = _appUserRepository.GetDefault(a => a.ID == dto.ID); �a�r�ld��� zaman update yaparken The instance of entity type 'AppUser' cannot be tracked because another instance with the same key value for {'ID'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.' hatas� vermektedir. Bunu a�mak i�in UpdateAppUserDTO nesnesine oldImage ve oldPassword propertyleri eklendi.

NOT 15 => cshtml i�erisinde herhangi bir inputun name attributune atanan isim controllerin actiona submit edilirse o actionda yaz�lan name de�eri input olarak al�nabilir. �rne�in checkbox �n name �zelli�ine example[] �eklinde atama yap�l�rsa ve ListWithFilters actionuna post edilirse actionun giri� parametresi olarak ListWithFilters(string[] example) �eklinde yakalanabilir.