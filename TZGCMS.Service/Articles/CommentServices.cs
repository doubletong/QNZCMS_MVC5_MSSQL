using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Infrastructure.Extensions;


namespace TZGCMS.Service.Articles
{
    public interface ICommentServices
    {
        #region Async
        Task<Comment> GetByIdWithCategoryAsync(int id);
        Task<bool> UpdateAsync(Comment article);
        #endregion
        IEnumerable<Comment> GetAll();
      
        List<Comment> GetPagedElements(int pageIndex, int pageSize, string keyword, int articleId, out int totalCount);
        List<Comment> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int articleId, out int totalCount);
        Comment GetById(int id);
      
        bool Update(Comment article);
        Comment Create(Comment article);
        bool Delete(Comment article);
        void Delete(int id);
        //bool CheckCode(string code);
    }

    public class CommentServices : ICommentServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Comment> GetByIdWithCategoryAsync(int id)
        {
            return await _unitOfWork.CommentRepository.GetFirstOrDefaultAsync(d => d.Id == id, default(CancellationToken), d => d.Article);
        }
        public async Task<bool> UpdateAsync(Comment article)
        {
            return await _unitOfWork.CommentRepository.UpdateAsync(article);
        }
        #endregion

        public IEnumerable<Comment> GetAll()
        {
            return _unitOfWork.CommentRepository.GetAll();
        }
       
        public List<Comment> GetPagedElements(int pageIndex, int pageSize, string keyword, int articleId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.CommentRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Body.Contains(keyword));
            if (articleId > 0)
                totalIQuery = totalIQuery.Where(g => g.ArticleId == articleId);

            totalCount = totalIQuery.Count();

            //get list
            List<Comment> comments;
            Expression<Func<Comment, bool>> filter = g => true;
            Expression<Func<Comment, bool>> filterByKeyword = g => g.Body.Contains(keyword);
            Expression<Func<Comment, bool>> filterByCategoryId = g => g.ArticleId == articleId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (articleId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            comments = _unitOfWork.CommentRepository.GetPagedElements(pageIndex, pageSize, (c => c.Pubdate), filter, false, (d=>d.Article)).ToList();

            return comments;
        }
        public List<Comment> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int articleId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.CommentRepository.Table.Where(d=>d.Active).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Body.Contains(keyword));
            if (articleId > 0)
                totalIQuery = totalIQuery.Where(g => g.ArticleId == articleId);

            totalCount = totalIQuery.Count();

            //get list
            List<Comment> comments;
            Expression<Func<Comment, bool>> filter = g => g.Active;
            Expression<Func<Comment, bool>> filterByKeyword = g => g.Body.Contains(keyword);
            Expression<Func<Comment, bool>> filterByCategoryId = g => g.ArticleId == articleId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (articleId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            comments = _unitOfWork.CommentRepository.GetPagedElements(pageIndex, pageSize, (c => c.Pubdate), filter, false, (d => d.Article)).ToList();

            return comments;
        }


        public Comment GetById(int id)
        {
            return _unitOfWork.CommentRepository.GetById(id);
        }
        

        public bool Update(Comment article)
        {
            return _unitOfWork.CommentRepository.Update(article);
        }

        public Comment Create(Comment article)
        {
            return _unitOfWork.CommentRepository.Insert(article);
        }
        public bool Delete(Comment article)
        {
            return _unitOfWork.CommentRepository.Delete(article);
        }

        public void Delete(int id)
        {
            var delobj = _unitOfWork.CommentRepository.Get(p => p.Id == id);
            _unitOfWork.CommentRepository.Delete(delobj);
        }

    }
}
