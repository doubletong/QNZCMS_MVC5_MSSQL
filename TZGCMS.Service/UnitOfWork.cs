using System;
using System.Collections.Generic;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Entity.Logs;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Entity.Videos;

namespace TZGCMS.Service
{
    public class UnitOfWork : IDisposable
    {

        #region Fields
        public readonly TZGEntities Context = new TZGEntities();
        private BaseRepository<PageMeta> _pageMetaRepository;
        private BaseRepository<Page> _pageRepository;
        private BaseRepository<ArticleCategory> _articleCategoryRepository;
        private BaseRepository<Article> _articleRepository;
        private BaseRepository<Comment> _commentRepository;
        private BaseRepository<FilterTemplate> _filterTemplateRepository;

        private BaseRepository<VideoCategory> _videoCategoryRepository;
        private BaseRepository<Video> _videoRepository;
        private BaseRepository<Reservation> _reservationRepository;

        private BaseRepository<User> _userRepository;
        private BaseRepository<Role> _roleRepository;
        private BaseRepository<Menu> _menuRepository;
        private BaseRepository<MenuCategory> _menuCategoryRepository;

        private BaseRepository<Log> _logRepository;
        private BaseRepository<Email> _emailRepository;
        private BaseRepository<EmailAccount> _emailAccountRepository;
        private BaseRepository<EmailTemplate> _emailTemplateRepository;

        private BaseRepository<Position> _positionRepository;
        private BaseRepository<Carousel> _carouselRepository;

        private BaseRepository<ProductCategory> _productCategoryRepository;
        private BaseRepository<Product> _productRepository;
        #endregion

        #region Constructors and Destructors

        public BaseRepository<ProductCategory> ProductCategoryRepository
        {
            get
            {
                if (this._productCategoryRepository == null)
                    this._productCategoryRepository = new BaseRepository<ProductCategory>(Context);
                return _productCategoryRepository;
            }
        }
        public BaseRepository<Product> ProductRepository
        {
            get
            {
                if (this._productRepository == null)
                    this._productRepository = new BaseRepository<Product>(Context);
                return _productRepository;
            }
        }
        public BaseRepository<Position> PositionRepository
        {
            get
            {
                if (this._positionRepository == null)
                    this._positionRepository = new BaseRepository<Position>(Context);
                return _positionRepository;
            }
        }

        public BaseRepository<Carousel> CarouselRepository
        {
            get
            {
                if (this._carouselRepository == null)
                    this._carouselRepository = new BaseRepository<Carousel>(Context);
                return _carouselRepository;
            }
        }

        public BaseRepository<Email> EmailRepository
        {
            get
            {
                if (this._emailRepository == null)
                    this._emailRepository = new BaseRepository<Email>(Context);
                return _emailRepository;
            }
        }
        public BaseRepository<EmailAccount> EmailAccountRepository
        {
            get
            {
                if (this._emailAccountRepository == null)
                    this._emailAccountRepository = new BaseRepository<EmailAccount>(Context);
                return _emailAccountRepository;
            }
        }
        public BaseRepository<EmailTemplate> EmailTemplateRepository
        {
            get
            {
                if (this._emailTemplateRepository == null)
                    this._emailTemplateRepository = new BaseRepository<EmailTemplate>(Context);
                return _emailTemplateRepository;
            }
        }

        public BaseRepository<PageMeta> PageMetaRepository
        {
            get
            {
                if (this._pageMetaRepository == null)
                    this._pageMetaRepository = new BaseRepository<PageMeta>(Context);
                return _pageMetaRepository;
            }
        }
        public BaseRepository<Page> PageRepository
        {
            get
            {
                if (this._pageRepository == null)
                    this._pageRepository = new BaseRepository<Page>(Context);
                return _pageRepository;
            }
        }
        public BaseRepository<ArticleCategory> ArticleCategoryRepository
        {
            get
            {
                if (this._articleCategoryRepository == null)
                    this._articleCategoryRepository = new BaseRepository<ArticleCategory>(Context);
                return _articleCategoryRepository;
            }
        }
        public BaseRepository<Article> ArticleRepository
        {
            get
            {
                if (this._articleRepository == null)
                    this._articleRepository = new BaseRepository<Article>(Context);
                return _articleRepository;
            }          
        }
        public BaseRepository<Comment> CommentRepository
        {
            get
            {
                if (this._commentRepository == null)
                    this._commentRepository = new BaseRepository<Comment>(Context);
                return _commentRepository;
            }
        }
        public BaseRepository<FilterTemplate> FilterTemplateRepository
        {
            get
            {
                if (this._filterTemplateRepository == null)
                    this._filterTemplateRepository = new BaseRepository<FilterTemplate>(Context);
                return _filterTemplateRepository;
            }
        }

        public BaseRepository<VideoCategory> VideoCategoryRepository
        {
            get
            {
                if (this._videoCategoryRepository == null)
                    this._videoCategoryRepository = new BaseRepository<VideoCategory>(Context);
                return _videoCategoryRepository;
            }
        }
        public BaseRepository<Video> VideoRepository
        {
            get
            {
                if (this._videoRepository == null)
                    this._videoRepository = new BaseRepository<Video>(Context);
                return _videoRepository;
            }
        }
        public BaseRepository<Reservation> ReservationRepository
        {
            get
            {
                if (this._reservationRepository == null)
                    this._reservationRepository = new BaseRepository<Reservation>(Context);
                return _reservationRepository;
            }
        }

        public BaseRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                    this._userRepository = new BaseRepository<User>(Context);
                return _userRepository;
            }
        }

        public BaseRepository<Role> RoleRepository
        {
            get
            {
                if (this._roleRepository == null)
                    this._roleRepository = new BaseRepository<Role>(Context);
                return _roleRepository;
            }
        }
        public BaseRepository<Menu> MenuRepository
        {
            get
            {
                if (this._menuRepository == null)
                    this._menuRepository = new BaseRepository<Menu>(Context);
                return _menuRepository;
            }
        }

        public BaseRepository<MenuCategory> MenuCategoryRepository
        {
            get
            {
                if (this._menuCategoryRepository == null)
                    this._menuCategoryRepository = new BaseRepository<MenuCategory>(Context);
                return _menuCategoryRepository;
            }
        }

        public BaseRepository<Log> LogRepository
        {
            get
            {
                if (this._logRepository == null)
                    this._logRepository = new BaseRepository<Log>(Context);
                return _logRepository;
            }
        }

        #endregion

        #region SQL 
        public IEnumerable<T> ExecuteQuery<T>(string sqlQuery, params object[] parameters)
        {
            return (IEnumerable<T>)Context.Database.SqlQuery<T>(sqlQuery, parameters);
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }
        #endregion

        #region Public Methods and Operators
        public void Save()
        {
            Context.SaveChanges();
        }
        public void SaveAsync()
        {
            Context.SaveChangesAsync();
        }
        #endregion

        #region Disposed
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}