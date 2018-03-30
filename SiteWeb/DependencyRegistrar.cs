using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMapper;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Service.Ads;
using TZGCMS.Service.Articles;
using TZGCMS.Service.Chronicles;
using TZGCMS.Service.Doc;
using TZGCMS.Service.Emails;
using TZGCMS.Service.Identity;
using TZGCMS.Service.Jobs;
using TZGCMS.Service.Links;
using TZGCMS.Service.Outlets;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Pages;
using TZGCMS.Service.Products;
using TZGCMS.Service.Systems;
using TZGCMS.Service.Teams;
using TZGCMS.Service.Videos;
using TZGCMS.SiteWeb.Mappings;

namespace TZGCMS.SiteWeb
{
    public static class DependencyRegistrar
    {
        public static void Register()
        {
            var config = GlobalConfiguration.Configuration;
            var builder = new ContainerBuilder();
            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            //installation localization service            
            builder.RegisterType<LoggingService>().As<ILoggingService>().InstancePerLifetimeScope();
            builder.RegisterType<CacheService>().As<ICacheService>();
            builder.RegisterType<TZGCMS.Infrastructure.Email.SMTPService>().As<TZGCMS.Infrastructure.Email.IEmailService>();

            builder.RegisterType<UserServices>().As<IUserServices>().InstancePerLifetimeScope();
            builder.RegisterType<RoleServices>().As<IRoleServices>().InstancePerLifetimeScope();
            builder.RegisterType<MenuServices>().As<IMenuServices>().InstancePerLifetimeScope();
            builder.RegisterType<MenuCategoryServices>().As<IMenuCategoryServices>().InstancePerLifetimeScope();
            builder.RegisterType<PageMetaServices>().As<IPageMetaServices>().InstancePerLifetimeScope();
            builder.RegisterType<PageServices>().As<IPageServices>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleServices>().As<IArticleServices>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleCategoryServices>().As<IArticleCategoryServices>().InstancePerLifetimeScope();
            builder.RegisterType<CommentServices>().As<ICommentServices>().InstancePerLifetimeScope();
            
            builder.RegisterType<FilterTemplateServices>().As<IFilterTemplateServices>().InstancePerLifetimeScope();
            
            builder.RegisterType<EmailServices>().As<IEmailServices>().InstancePerLifetimeScope();
            builder.RegisterType<EmailAccountServices>().As<IEmailAccountServices>().InstancePerLifetimeScope();
            builder.RegisterType<EmailTemplateServices>().As<IEmailTemplateServices>().InstancePerLifetimeScope();

            builder.RegisterType<VideoCategoryServices>().As<IVideoCategoryServices>().InstancePerLifetimeScope();
            builder.RegisterType<VideoServices>().As<IVideoServices>().InstancePerLifetimeScope();
            builder.RegisterType<ReservationServices>().As<IReservationServices>().InstancePerLifetimeScope();
            

            builder.RegisterType<PositionServices>().As<IPositionServices>().InstancePerLifetimeScope();
            builder.RegisterType<CarouselServices>().As<ICarouselServices>().InstancePerLifetimeScope();

            builder.RegisterType<JobServices>().As<IJobServices>().InstancePerLifetimeScope();
            builder.RegisterType<TeamServices>().As<ITeamServices>().InstancePerLifetimeScope();

            builder.RegisterType<BackupServices>().As<IBackupServices>().InstancePerLifetimeScope();
            builder.RegisterType<LogServices>().As<ILogServices>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCategoryServices>().As<IProductCategoryServices>().InstancePerLifetimeScope();
            builder.RegisterType<ProductServices>().As<IProductServices>().InstancePerLifetimeScope();
            builder.RegisterType<DocumentCategoryServices>().As<IDocumentCategoryServices>().InstancePerLifetimeScope();
            builder.RegisterType<DocumentServices>().As<IDocumentServices>().InstancePerLifetimeScope();

            builder.RegisterType<LinkCategoryServices>().As<ILinkCategoryServices>().InstancePerLifetimeScope();
            builder.RegisterType<LinkServices>().As<ILinkServices>().InstancePerLifetimeScope();

            builder.RegisterType<ChronicleServices>().As<IChronicleServices>().InstancePerLifetimeScope();
            builder.RegisterType<OutletServices>().As<IOutletServices>().InstancePerLifetimeScope();
            // MVC - Set the dependency resolver to be Autofac.



            //automapper 注册
            var profiles = from t in typeof(BackDomainToViewModelMappingProfile).Assembly.GetTypes() where typeof(Profile).IsAssignableFrom(t) select (Profile)Activator.CreateInstance(t);
            var frontProfiles = from t in typeof(FrontDomainToViewModelMappingProfile).Assembly.GetTypes() where typeof(Profile).IsAssignableFrom(t) select (Profile)Activator.CreateInstance(t);
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            }));
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in frontProfiles)
                {
                    cfg.AddProfile(profile);
                }
            }));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
           // webapi autofac 注入
        

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);
            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();
            // Set the dependency resolver to be Autofac.          
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);




        }
    }
}