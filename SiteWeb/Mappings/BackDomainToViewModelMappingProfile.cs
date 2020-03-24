using AutoMapper;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Entity.Chronicles;
using TZGCMS.Data.Entity.Doc;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Entity.Links;
using TZGCMS.Data.Entity.Logs;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Model;
using TZGCMS.Model.Admin.InputModel.Ads;
using TZGCMS.Model.Admin.InputModel.Articles;
using TZGCMS.Model.Admin.InputModel.Chronicles;
using TZGCMS.Model.Admin.InputModel.Doc;
using TZGCMS.Model.Admin.InputModel.Emails;
using TZGCMS.Model.Admin.InputModel.Identity;
using TZGCMS.Model.Admin.InputModel.Links;
using TZGCMS.Model.Admin.InputModel.Menus;

using TZGCMS.Model.Admin.InputModel.Teams;
using TZGCMS.Model.Admin.InputModel.Videos;
using TZGCMS.Model.Admin.ViewModel.Ads;
using TZGCMS.Model.Admin.ViewModel.Articles;
using TZGCMS.Model.Admin.ViewModel.Chronicles;
using TZGCMS.Model.Admin.ViewModel.Emails;
using TZGCMS.Model.Admin.ViewModel.Log;
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
            CreateMap<QNZ.Data.Menu, FrontMenuIM>();
            CreateMap<FrontMenuIM, QNZ.Data.Menu>();

            CreateMap<MenuCategoryIM, MenuCategory>();
            CreateMap<MenuCategory, MenuCategoryIM>();
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

            CreateMap<QNZ.Data.Album, AlbumVM>();
            CreateMap<AlbumIM, QNZ.Data.Album>();
            CreateMap<QNZ.Data.Album, AlbumIM>();

            CreateMap<QNZ.Data.Photo, PhotoVM>().ForMember(d => d.AlbumName, opt => opt.MapFrom(source => source.Album.Title));
            CreateMap<PhotoIM, QNZ.Data.Photo>();
            CreateMap<QNZ.Data.Photo, PhotoIM>();

            CreateMap<QNZ.Data.Outlet, OutletVM>();
            CreateMap<OutletIM, QNZ.Data.Outlet>();
            CreateMap<QNZ.Data.Outlet, OutletIM>();

            CreateMap<QNZ.Data.Laboratory, LaboratoryVM>().ForMember(d => d.InstituteTitle, opt => opt.MapFrom(source => source.Institute.Title));
            CreateMap<LaboratoryIM, QNZ.Data.Laboratory>();
            CreateMap<QNZ.Data.Laboratory, LaboratoryIM>();

            CreateMap<QNZ.Data.Institute, InstituteVM>();
            CreateMap<InstituteIM, QNZ.Data.Institute>();
            CreateMap<QNZ.Data.Institute, InstituteIM>();

            CreateMap<QNZ.Data.AchievementCategory, AchievementCategoryVM>();
            CreateMap<AchievementCategoryIM, QNZ.Data.AchievementCategory>();
            CreateMap<QNZ.Data.AchievementCategory, AchievementCategoryIM>();

            CreateMap<QNZ.Data.Achievement,AchievementVM> ().ForMember(d => d.CategoryTitle, opt => opt.MapFrom(source => source.AchievementCategory.Title));
            CreateMap<AchievementIM, QNZ.Data.Achievement>();
            CreateMap<QNZ.Data.Achievement, AchievementIM>();

            CreateMap<QNZ.Data.ProductCategory, ProductCategoryVM>();
            CreateMap<ProductCategoryIM, QNZ.Data.ProductCategory>();
            CreateMap<QNZ.Data.ProductCategory, ProductCategoryIM>();

            CreateMap<QNZ.Data.Product, ProductVM>();
            CreateMap<QNZ.Data.Product, ProductIM>();
            CreateMap<ProductIM, QNZ.Data.Product>();


            //CreateMap<TechnologyIM, SIG.DAL.Dapper.Model.Technology>();
            //CreateMap<SIG.DAL.Dapper.Model.Technology, TechnologyIM>();

            //CreateMap<Product, SearchData>().ForMember(d => d.Name, opt => opt.MapFrom(source => source.ProductName));


            CreateMap<QNZ.Data.Article, ArticleVM>().ForMember(d => d.CategoryTitle, opt => opt.MapFrom(source => source.ArticleCategory.Title));
            CreateMap<QNZ.Data.Article, ArticleIM>();
            CreateMap<ArticleIM, QNZ.Data.Article>();
            CreateMap<ArticleCategoryVM, QNZ.Data.ArticleCategory >();
            CreateMap<QNZ.Data.ArticleCategory, ArticleCategoryVM>();
            CreateMap<ArticleCategoryIM, QNZ.Data.ArticleCategory>();
            CreateMap<QNZ.Data.ArticleCategory, ArticleCategoryIM>();

            CreateMap<VideoCategoryIM, VideoCategory>();
            CreateMap<VideoCategory, VideoCategoryIM>();

            CreateMap<QNZ.Data.Page, PageVM>();
            CreateMap<QNZ.Data.Page, PageIM>();
            CreateMap<PageIM, QNZ.Data.Page>();
            CreateMap<DocumentCategoryIM, DocumentCategory>();
            CreateMap<DocumentCategory, DocumentCategoryIM>();

            CreateMap<DocumentIM, Document>();
            CreateMap<Document, DocumentIM>();

            CreateMap<SimpleProductVM, SimpleProduct>();
            CreateMap<SimpleProduct, SimpleProductVM>();
            CreateMap<SimpleProductIM, SimpleProduct>();
            CreateMap<SimpleProduct, SimpleProductIM>();

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

            CreateMap<LinkCategoryIM, LinkCategory>();
            CreateMap<LinkCategory, LinkCategoryIM>();

            CreateMap<LinkIM, Link>();
            CreateMap<Link, LinkIM>();
         
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

            CreateMap<ChronicleVM, Chronicle>();
            CreateMap<Chronicle, ChronicleVM>();
            CreateMap<ChronicleIM, Chronicle>();
            CreateMap<Chronicle, ChronicleIM>();




          
            CreateMap<JobVM, QNZ.Data.Job>();
            //CreateMap<Job, JobVM>();
            CreateMap<JobIM, QNZ.Data.Job>();
            CreateMap<QNZ.Data.Job, JobIM>();

            //CreateMap<TeamVM, Team>();
            //CreateMap<Team, TeamVM>();
            CreateMap<TeamIM, Team>();
            CreateMap<Team, TeamIM>();

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
            CreateMap<CaseIM, Case>();
            CreateMap<Case, CaseIM>();

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