using AutoMapper;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Entity.Logs;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Model.Admin.InputModel.Ads;
using TZGCMS.Model.Admin.InputModel.Articles;
using TZGCMS.Model.Admin.InputModel.Emails;
using TZGCMS.Model.Admin.InputModel.Identity;
using TZGCMS.Model.Admin.InputModel.LuceneSearch;
using TZGCMS.Model.Admin.InputModel.Menus;
using TZGCMS.Model.Admin.InputModel.Pages;
using TZGCMS.Model.Admin.InputModel.Products;
using TZGCMS.Model.Admin.InputModel.Videos;
using TZGCMS.Model.Admin.ViewModel.Ads;
using TZGCMS.Model.Admin.ViewModel.Articles;
using TZGCMS.Model.Admin.ViewModel.Emails;
using TZGCMS.Model.Admin.ViewModel.Log;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;
using TZGCMS.Model.Admin.ViewModel.Videos;

namespace TZGCMS.SiteWeb.Mappings
{
    /// <summary>
    /// DTO模型映射
    /// </summary>
    public class BackDomainToViewModelMappingProfile : Profile
    {
        /// <summary>
        /// 构造
        /// </summary>
        public BackDomainToViewModelMappingProfile()
        {
            CreateMap<Log, LogVM>();
            CreateMap<LogVM, Log>();
            //CreateMap<Menu, MenuVM>();
            //CreateMap<MenuVM, Menu>();

            //CreateMap<Menu, BaseMenuVM>();
            //CreateMap<BaseMenuVM, Menu>();

            CreateMap<Menu, MenuIM>();
            CreateMap<MenuIM, Menu>();
            CreateMap<Menu, FrontMenuIM>();
            CreateMap<FrontMenuIM, Menu>();
            //CreateMap<Menu, UserMenuDTO>();

            //CreateMap<MenuCategory, MenuCategoryVM>();
            //CreateMap<MenuCategoryIM, SIG.DAL.Dapper.Model.MenuCategory>();
            //CreateMap<SIG.DAL.Dapper.Model.MenuCategory, MenuCategoryIM>();

            CreateMap<User, ProfileIM>();
            CreateMap<ProfileIM, User>();
            //CreateMap<SIG.DAL.Dapper.Model.User, ProfileIM>();
            //CreateMap<ProfileIM, SIG.DAL.Dapper.Model.User>();
            //CreateMap<User, UserVM>();
            //CreateMap<UserVM, User>();

            //Mapper.CreateMap<Diary, DiaryDTO>().ForMember(d=>d.ProjectTitle,opt=>opt.MapFrom(source=>source.Project.Title))
            //    .ForMember(d=>d.WorkTypeTitle, opt=>opt.MapFrom(source=>source.WorkType.Title));

            //Mapper.CreateMap<InAccount, InAccountDTO>().ForMember(d => d.ProjectTitle, opt => opt.MapFrom(source => source.Project.Title))
            //   .ForMember(d => d.InTypeTitle, opt => opt.MapFrom(source => source.InType.Title))
            //   .ForMember(d => d.CustomerName, opt => opt.MapFrom(source => source.Customer.CustomerName));

            CreateMap<ProductCategoryIM, ProductCategory>();
            CreateMap<ProductCategory, ProductCategoryIM>();

            CreateMap<Product, ProductIM>();
            CreateMap<ProductIM, Product>();


            //CreateMap<TechnologyIM, SIG.DAL.Dapper.Model.Technology>();
            //CreateMap<SIG.DAL.Dapper.Model.Technology, TechnologyIM>();

            //CreateMap<Product, SearchData>().ForMember(d => d.Name, opt => opt.MapFrom(source => source.ProductName));


            CreateMap<Article, ArticleVM>().ForMember(d => d.CategoryTitle, opt => opt.MapFrom(source => source.ArticleCategory.Title));
            CreateMap<Article, ArticleIM>();
            CreateMap<ArticleIM, Article>();
            CreateMap<ArticleCategoryVM, ArticleCategory>();
            CreateMap<ArticleCategory, ArticleCategoryVM>();
            CreateMap<ArticleCategoryIM, ArticleCategory>();
            CreateMap<ArticleCategory, ArticleCategoryIM>();

            CreateMap<VideoCategoryIM, VideoCategory>();
            CreateMap<VideoCategory, VideoCategoryIM>();

            CreateMap<Page, PageIM>();
            CreateMap<PageIM, Page>();

            //CreateMap<PageMeta, ArticleIM>().ForMember(x => x.Id, opt => opt.MapFrom(source => source.ObjectId));
            //CreateMap<ArticleIM, PageMeta>().ForMember(x => x.ObjectId, opt => opt.MapFrom(source => source.Id));

            //CreateMap<GoodsCategoryVM, GoodsCategory>();
            //CreateMap<GoodsCategory, GoodsCategoryVM>();
            //CreateMap<GoodsCategoryIM, GoodsCategory>();
            //CreateMap<GoodsCategory, GoodsCategoryIM>();

            //CreateMap<Goods, GoodsVM>().ForMember(d => d.CategoryTitle, opt => opt.MapFrom(source => source.GoodsCategory.Title));
            //CreateMap<Goods, GoodsIM>();

            //CreateMap<GoodsVM, Goods>();
            //CreateMap<GoodsIM, Goods>();

            //CreateMap<GoodsPhotoVM, GoodsPhoto>();
            //CreateMap<GoodsPhotoIM, GoodsPhoto>();
            //CreateMap<GoodsPhoto, GoodsPhotoVM>();
            //CreateMap<GoodsPhoto, GoodsPhotoIM>();

            CreateMap<Position, PositionIM>();
            CreateMap<PositionIM, Position>();
            CreateMap<Position, PositionVM>();
            CreateMap<PositionVM, Position>();
            CreateMap<Carousel, CarouselIM>();
            CreateMap<CarouselIM, Carousel>();
            CreateMap<Carousel, CarouselVM>();
            CreateMap<CarouselVM, Carousel>();


            //CreateMap<Question, QuestionVM>().ForMember(d => d.CategoryTitle, opt => opt.MapFrom(source => source.Category.Title));
            //CreateMap<Question, QuestionVM>();
            //CreateMap<QuestionIM, Question>();
            //CreateMap<Question, QuestionIM>();
            //CreateMap<QuestionCategoryVM, QuestionCategory>();
            //CreateMap<QuestionCategory, QuestionCategoryVM>();
            //CreateMap<QuestionCategoryIM, QuestionCategory>();
            //CreateMap<QuestionCategory, QuestionCategoryIM>();

            //CreateMap<LinkCategoryVM, LinkCategory>();
            //CreateMap<LinkCategory, LinkCategoryVM>();
            //CreateMap<LinkCategoryIM, SIG.DAL.Dapper.Model.LinkCategory>();
            //CreateMap<SIG.DAL.Dapper.Model.LinkCategory, LinkCategoryIM>();
            //CreateMap<LinkVM, Link>();
            //CreateMap<Link, LinkVM>();
            //CreateMap<LinkIM, SIG.DAL.Dapper.Model.Link>();
            //CreateMap<SIG.DAL.Dapper.Model.Link, LinkIM>();

            //CreateMap<AnnouncementVM, Announcement>();
            //CreateMap<Announcement, AnnouncementVM>();
            //CreateMap<AnnouncementIM, Announcement>();
            //CreateMap<Announcement, AnnouncementIM>();

            CreateMap<VideoVM, Video>();
            CreateMap<Video, VideoVM>();
            CreateMap<VideoIM, Video>();
            CreateMap<Video, VideoIM>();

            CreateMap<EmailVM, Email>();
            CreateMap<Email, EmailVM>();

            CreateMap<EmailAccountVM, EmailAccount>();
            CreateMap<EmailAccount, EmailAccountVM>();
            CreateMap<EmailAccountIM, EmailAccount>();
            CreateMap<EmailAccount, EmailAccountIM>();

            CreateMap<EmailTemplateVM, EmailTemplate>();
            CreateMap<EmailTemplate, EmailTemplateVM>();
            CreateMap<EmailTemplateIM, EmailTemplate>();
            CreateMap<EmailTemplate, EmailTemplateIM>();

            //CreateMap<ChronicleVM, Chronicle>();
            //CreateMap<Chronicle, ChronicleVM>();
            //CreateMap<ChronicleIM, Chronicle>();
            //CreateMap<Chronicle, ChronicleIM>();

            //CreateMap<AlbumVM, Album>();
            //CreateMap<Album, AlbumVM>();
            //CreateMap<AlbumIM, SIG.DAL.Dapper.Model.Album>();
            //CreateMap<SIG.DAL.Dapper.Model.Album, AlbumIM>();

            //CreateMap<PhotoVM, Photo>();
            //CreateMap<Photo, PhotoVM>();
            //CreateMap<PhotoIM, SIG.DAL.Dapper.Model.Photo>();
            //CreateMap<SIG.DAL.Dapper.Model.Photo, PhotoIM>();

            //CreateMap<PageVM, Page>();
            //CreateMap<Page, PageVM>();
            //CreateMap<PageIM, SIG.DAL.Dapper.Model.Page>();
            //CreateMap<SIG.DAL.Dapper.Model.Page, PageIM>();

            //CreateMap<JobVM, Job>();
            //CreateMap<Job, JobVM>();
            //CreateMap<JobIM, Job>();
            //CreateMap<Job, JobIM>();

            //CreateMap<TeamVM, Team>();
            //CreateMap<Team, TeamVM>();
            //CreateMap<TeamIM, SIG.DAL.Dapper.Model.Team>();
            //CreateMap<SIG.DAL.Dapper.Model.Team, TeamIM>();

            //CreateMap<ClientVM, Client>();
            //CreateMap<Client, ClientVM>();
            //CreateMap<ClientIM, SIG.DAL.Dapper.Model.Client>();
            //CreateMap<SIG.DAL.Dapper.Model.Client, ClientIM>();

            //CreateMap<SolutionVM, Solution>();
            //CreateMap<Solution, SolutionVM>();
            //CreateMap<SolutionIM, Solution>();
            //CreateMap<Solution, SolutionIM>();

            //CreateMap<WorkTypeVM, WorkType>();
            //CreateMap<WorkType, WorkTypeVM>();
            //CreateMap<WorkCategoryIM, SIG.DAL.Dapper.Model.WorkCategory>();
            //CreateMap<SIG.DAL.Dapper.Model.WorkCategory, WorkCategoryIM>();

            //CreateMap<WorkVM, Work>();
            //CreateMap<Work, WorkVM>();
            //CreateMap<WorkIM, SIG.DAL.Dapper.Model.Work>();
            //CreateMap<SIG.DAL.Dapper.Model.Work, WorkIM>();

            //CreateMap<PostCategoryVM, PostCategory>();
            //CreateMap<PostCategory, PostCategoryVM>();
            //CreateMap<PostCategoryIM,SIG.DAL.Dapper.Model.PostCategory>();
            //CreateMap<SIG.DAL.Dapper.Model.PostCategory, PostCategoryIM>();

            //CreateMap<PostVM, Post>();
            //CreateMap<Post, PostVM>();
            //CreateMap<PostIM, SIG.DAL.Dapper.Model.Post>();
            //CreateMap<SIG.DAL.Dapper.Model.Post, PostIM>();

            CreateMap<FilterTemplateVM, FilterTemplate>();
            CreateMap<FilterTemplate, FilterTemplateVM>();
            CreateMap<FilterTemplateIM, FilterTemplate>();
            CreateMap<FilterTemplate, FilterTemplateIM>();

            CreateMap<SearchData, SearchDataIM>();
            CreateMap<SearchDataIM, SearchData>();

            //Mapper.CreateMap<Goal, GoalListViewModel>().ForMember(x => x.SupportsCount, opt => opt.MapFrom(source => source.Supports.Count))
            //                                          .ForMember(x => x.UserName, opt => opt.MapFrom(source => source.User.UserName))
            //                                          .ForMember(x => x.StartDate, opt => opt.MapFrom(source => source.StartDate.ToString("dd MMM yyyy")))
            //                                          .ForMember(x => x.EndDate, opt => opt.MapFrom(source => source.EndDate.ToString("dd MMM yyyy")));
            //Mapper.CreateMap<Group, GroupsItemViewModel>().ForMember(x => x.CreatedDate, opt => opt.MapFrom(source => source.CreatedDate.ToString("dd MMM yyyy")));

            //Mapper.CreateMap<IPagedList<Group>, IPagedList<GroupsItemViewModel>>().ConvertUsing<PagedListConverter<Group, GroupsItemViewModel>>();


        }
    }
}