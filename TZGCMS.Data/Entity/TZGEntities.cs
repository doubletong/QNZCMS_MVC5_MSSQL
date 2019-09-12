using SIG.Model.Mapping;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Entity.Chronicles;
using TZGCMS.Data.Entity.Doc;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Entity.Links;
using TZGCMS.Data.Entity.Logs;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Data.Mapping;
using TZGCMS.Infrastructure.Helper;

namespace TZGCMS.Data.Entity
{
    public partial class TZGEntities : DbContext
    {
        public TZGEntities()
            : base("name=TZGEntities")
        {
        }

        public virtual DbSet<SimpleProduct> SimpleProducts { get; set; }
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<ArticleCategory> ArticleCategory { get; set; }
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<FilterTemplate> FilterTemplate { get; set; }
        public virtual DbSet<VideoCategory> VideoCategory { get; set; }
        public virtual DbSet<Video> Video { get; set; }
        public virtual DbSet<Reservation> Reservation { get; set; }

        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageMeta> PageMetas { get; set; }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuCategory> MenuCategory { get; set; }
        public virtual DbSet<Role> Role { get; set; }

        public virtual DbSet<Log> Log { get; set; }

        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailAccount> EmailAccounts { get; set; }

        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Carousel> Carousels { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<DocumentCategory> DocumentCategories { get; set; }
        public virtual DbSet<Document> Documents { get; set; }

        public virtual DbSet<LinkCategory> LinkCategories { get; set; }
        public virtual DbSet<Link> Links { get; set; }

        public virtual DbSet<Chronicle> Chronicles { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Outlet> Outlets { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new MenuMap());
            modelBuilder.Configurations.Add(new MenuCategoryMap());
            modelBuilder.Configurations.Add(new PageMap());
            modelBuilder.Configurations.Add(new PageMetaMap());

            modelBuilder.Configurations.Add(new LogMap());

            modelBuilder.Configurations.Add(new EmailMap());
            modelBuilder.Configurations.Add(new EmailAccountMap());
            modelBuilder.Configurations.Add(new EmailTemplateMap());
            modelBuilder.Configurations.Add(new ArticleCategoryMap());
            modelBuilder.Configurations.Add(new ArticleMap());
            modelBuilder.Configurations.Add(new CommentMap());
            modelBuilder.Configurations.Add(new FilterTemplateMap());

            modelBuilder.Configurations.Add(new VideoCategoryMap());
            modelBuilder.Configurations.Add(new VideoMap());
            modelBuilder.Configurations.Add(new ReservationMap());

            modelBuilder.Configurations.Add(new PositionMap());
            modelBuilder.Configurations.Add(new CarouselMap());

            modelBuilder.Configurations.Add(new ProductCategoryMap());
            modelBuilder.Configurations.Add(new ProductMap());

            modelBuilder.Configurations.Add(new DocumentMap());
            modelBuilder.Configurations.Add(new DocumentCategoryMap());

            modelBuilder.Configurations.Add(new LinkMap());
            modelBuilder.Configurations.Add(new LinkCategoryMap());

            modelBuilder.Configurations.Add(new ChronicleMap());
            modelBuilder.Configurations.Add(new JobMap());
            modelBuilder.Configurations.Add(new TeamMap());
            modelBuilder.Configurations.Add(new OutletMap());
            //throw new UnintentionalCodeFirstException();
        }

        public override int SaveChanges()
        {
            AddUpdateUserInfo();

            return base.SaveChanges();
        }

        private void AddUpdateUserInfo()
        {
            var addedAuditedEntities = ChangeTracker.Entries<IAuditedEntity>()
                          .Where(p => p.State == EntityState.Added)
                          .Select(p => p.Entity);

            var modifiedAuditedEntities = ChangeTracker.Entries<IAuditedEntity>()
              .Where(p => p.State == EntityState.Modified)
              .Select(p => p.Entity);

            var now = DateTime.UtcNow;

            foreach (var added in addedAuditedEntities)
            {
                added.CreatedDate = now;
                added.CreatedBy = Site.CurrentUserName;
            }

            foreach (var modified in modifiedAuditedEntities)
            {
                modified.UpdatedDate = now;
                modified.UpdatedBy = Site.CurrentUserName;

                base.Entry(modified).Property(x => x.CreatedBy).IsModified = false;
                base.Entry(modified).Property(x => x.CreatedDate).IsModified = false;

            }
        }

        public override Task<int> SaveChangesAsync()
        {
            AddUpdateUserInfo();

            return base.SaveChangesAsync();
        }
    }
}
