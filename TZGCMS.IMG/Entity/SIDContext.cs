namespace TZGCMS.IMG.Entity
{
    using System.Data.Entity;

    public partial class SIDContext : DbContext
    {
        public SIDContext()
            : base("name=SIDContext")
        {
        }

        public virtual DbSet<AlbumSet> AlbumSets { get; set; }
        public virtual DbSet<AnnouncementSet> AnnouncementSets { get; set; }
        public virtual DbSet<ArticleCategorySet> ArticleCategorySets { get; set; }
        public virtual DbSet<ArticleSet> ArticleSets { get; set; }
        public virtual DbSet<CarouselSet> CarouselSets { get; set; }
        public virtual DbSet<CartSet> CartSets { get; set; }
        public virtual DbSet<ChronicleSet> ChronicleSets { get; set; }
        public virtual DbSet<ClientSet> ClientSets { get; set; }
        public virtual DbSet<CommentSet> CommentSets { get; set; }
        public virtual DbSet<DocumentCategorySet> DocumentCategorySets { get; set; }
        public virtual DbSet<EmailAccountSet> EmailAccountSets { get; set; }
        public virtual DbSet<EmailSet> EmailSets { get; set; }
        public virtual DbSet<EmailTemplateSet> EmailTemplateSets { get; set; }
        public virtual DbSet<FilterTemplateSet> FilterTemplateSets { get; set; }
        public virtual DbSet<GoodsCategorySet> GoodsCategorySets { get; set; }
        public virtual DbSet<GoodsPhotoSet> GoodsPhotoSets { get; set; }
        public virtual DbSet<GoodsSet> GoodsSets { get; set; }
        public virtual DbSet<JobSet> JobSets { get; set; }
        public virtual DbSet<LinkCategorySet> LinkCategorySets { get; set; }
        public virtual DbSet<LinkSet> LinkSets { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<MenuCategorySet> MenuCategorySets { get; set; }
        public virtual DbSet<MenuSet> MenuSets { get; set; }
        public virtual DbSet<OrderDetailSet> OrderDetailSets { get; set; }
        public virtual DbSet<OrderSet> OrderSets { get; set; }
        public virtual DbSet<OutletSet> OutletSets { get; set; }
        public virtual DbSet<PageMetaSet> PageMetaSets { get; set; }
        public virtual DbSet<PageSet> PageSets { get; set; }
        public virtual DbSet<PhotoSet> PhotoSets { get; set; }
        public virtual DbSet<PositionSet> PositionSets { get; set; }
        public virtual DbSet<PostCategorySet> PostCategorySets { get; set; }
        public virtual DbSet<PostSet> PostSets { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductCategorySet> ProductCategorySets { get; set; }
        public virtual DbSet<ProductPhotoSet> ProductPhotoSets { get; set; }
        public virtual DbSet<ProductSet> ProductSets { get; set; }
        public virtual DbSet<QuestionCategorySet> QuestionCategorySets { get; set; }
        public virtual DbSet<QuestionSet> QuestionSets { get; set; }
        public virtual DbSet<RecipientInfoSet> RecipientInfoSets { get; set; }
        public virtual DbSet<ReservationSet> ReservationSets { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SolutionSet> SolutionSets { get; set; }
        public virtual DbSet<T_City> T_City { get; set; }
        public virtual DbSet<T_District> T_District { get; set; }
        public virtual DbSet<T_Province> T_Province { get; set; }
        public virtual DbSet<TeamSet> TeamSets { get; set; }
        public virtual DbSet<TechnologySet> TechnologySets { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSafetySet> UserSafetySets { get; set; }
        public virtual DbSet<VideoCategorySet> VideoCategorySets { get; set; }
        public virtual DbSet<VideoSet> VideoSets { get; set; }
        public virtual DbSet<WorkCategorySet> WorkCategorySets { get; set; }
        public virtual DbSet<WorkSet> WorkSets { get; set; }
        public virtual DbSet<DocumentSet> DocumentSets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlbumSet>()
                .HasMany(e => e.PhotoSets)
                .WithRequired(e => e.AlbumSet)
                .HasForeignKey(e => e.AlbumId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ArticleCategorySet>()
                .HasMany(e => e.ArticleSets)
                .WithRequired(e => e.ArticleCategorySet)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FilterTemplateSet>()
                .Property(e => e.Encode)
                .IsUnicode(false);

            modelBuilder.Entity<GoodsCategorySet>()
                .HasMany(e => e.GoodsSets)
                .WithRequired(e => e.GoodsCategorySet)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<GoodsSet>()
                .HasMany(e => e.CartSets)
                .WithRequired(e => e.GoodsSet)
                .HasForeignKey(e => e.GoodsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<GoodsSet>()
                .HasMany(e => e.GoodsPhotoSets)
                .WithRequired(e => e.GoodsSet)
                .HasForeignKey(e => e.GoodsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MenuCategorySet>()
                .HasMany(e => e.MenuSets)
                .WithRequired(e => e.MenuCategorySet)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<MenuSet>()
                .HasMany(e => e.MenuSet1)
                .WithOptional(e => e.MenuSet2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<MenuSet>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.MenuSets)
                .Map(m => m.ToTable("RoleMenus").MapLeftKey("MenuId").MapRightKey("RoleId"));

            modelBuilder.Entity<OrderSet>()
                .HasMany(e => e.OrderDetailSets)
                .WithRequired(e => e.OrderSet)
                .HasForeignKey(e => e.OrderId);

            modelBuilder.Entity<PositionSet>()
                .HasMany(e => e.CarouselSets)
                .WithRequired(e => e.PositionSet)
                .HasForeignKey(e => e.PositionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCategorySet>()
                .HasMany(e => e.ProductCategories)
                .WithRequired(e => e.ProductCategorySet)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<ProductCategorySet>()
                .HasMany(e => e.ProductCategorySet1)
                .WithOptional(e => e.ProductCategorySet2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<ProductSet>()
                .HasMany(e => e.ProductPhotoSets)
                .WithRequired(e => e.ProductSet)
                .HasForeignKey(e => e.ProductId);

            modelBuilder.Entity<ProductSet>()
                .HasMany(e => e.TechnologySets)
                .WithRequired(e => e.ProductSet)
                .HasForeignKey(e => e.ProductId);

            modelBuilder.Entity<QuestionCategorySet>()
                .HasMany(e => e.QuestionSets)
                .WithRequired(e => e.QuestionCategorySet)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RecipientInfoSet>()
                .Property(e => e.Phone)
                .IsFixedLength();

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Roles)
                .Map(m => m.ToTable("UserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<T_City>()
                .HasMany(e => e.RecipientInfoSets)
                .WithOptional(e => e.T_City)
                .HasForeignKey(e => e.CityId);

            modelBuilder.Entity<T_City>()
                .HasMany(e => e.T_District)
                .WithRequired(e => e.T_City)
                .HasForeignKey(e => e.CityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<T_District>()
                .HasMany(e => e.RecipientInfoSets)
                .WithOptional(e => e.T_District)
                .HasForeignKey(e => e.DistrictId);

            modelBuilder.Entity<T_Province>()
                .HasMany(e => e.RecipientInfoSets)
                .WithRequired(e => e.T_Province)
                .HasForeignKey(e => e.ProvinceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<T_Province>()
                .HasMany(e => e.T_City)
                .WithOptional(e => e.T_Province)
                .HasForeignKey(e => e.ProvinceId);

            modelBuilder.Entity<WorkCategorySet>()
                .HasMany(e => e.WorkSets)
                .WithMany(e => e.WorkCategorySets)
                .Map(m => m.ToTable("WorkWorkCategorySet").MapLeftKey("CategoryId").MapRightKey("WorkId"));
        }
    }
}
