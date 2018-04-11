using AutoMapper;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Model.Front.InputModel.Articles;
using TZGCMS.Model.Front.ViewModel.Ads;
using TZGCMS.Model.Front.ViewModel.Articles;
using TZGCMS.Model.Front.ViewModel.Outlets;
using TZGCMS.Model.Front.ViewModel.Pages;
using TZGCMS.Model.Front.ViewModel.Products;
using TZGCMS.Model.Front.ViewModel.Videos;

namespace TZGCMS.SiteWeb.Mappings
{
    /// <summary>
    /// 前台表现层模型映射
    /// </summary>
    public class FrontDomainToViewModelMappingProfile : Profile
    {
        /// <summary>
        /// 构造
        /// </summary>
        public FrontDomainToViewModelMappingProfile()
        {
            //CreateMap<Goods, GoodsVM>();
            //CreateMap<Goods, GoodsDetailVM>();

            //CreateMap<GoodsVM, Goods>();
            //CreateMap<GoodsCategory, GoodsCategoryVM>();
            //CreateMap<GoodsCategoryVM, GoodsCategory>();

            CreateMap<Carousel, CarouselFVM>();
            CreateMap<Article, ArticleFVM>();
            CreateMap<Article, ArticleDetailFVM>();
            CreateMap<VideoCategory, VideoCategoryFVM>();
            CreateMap<Video, VideoFVM>();
            CreateMap<CommentFIM, Comment>();
            CreateMap<Comment, CommentFVM>();
            CreateMap<Page, PageFVM>();
            CreateMap<Outlet, OutletFVM>()             
                .ForMember(d => d.callout, opt => opt.MapFrom(source => new Callout { content = source.Name + "\r\n地址：" + source.Address,padding = 8, borderRadius=6 }));

            //CreateMap<Announcement, AnnouncementVM>();
            //CreateMap<Client, ClientVM>();
            //CreateMap<Video, VideoVM>();
            //CreateMap<Post, PostVM>();
            //CreateMap<SIG.DAL.Dapper.Model.Post, PostDetailVM>();
            //CreateMap<Work, WorkVM>();
            //CreateMap<Solution, SolutionVM>();

            //CreateMap<Link, LinkVM>();
            //CreateMap<LinkCategory, LinkCategoryVM>();       
            //CreateMap<Photo, PhotoVM>();

            CreateMap<Product, ProductVM>();
            CreateMap<Product, ProductDetailVM>();
            CreateMap<ProductCategory, ProductCategoryVM>();
        }
       

    }
}