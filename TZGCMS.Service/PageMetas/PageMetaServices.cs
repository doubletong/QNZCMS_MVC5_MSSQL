using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;

namespace TZGCMS.Service.PageMetas
{
    public interface IPageMetaServices
    {
        
        PageMeta GetPageMeta(ModelType modelType, string objectId);
        bool Update(PageMeta pageMeta);
        PageMeta Create(PageMeta pageMeta);
        bool Delete(PageMeta pageMeta);
        void SetPageMeta(ModelType modelType, string objectId, string objectTitle, string title,string keywords,string description);
    }
    public class PageMetaServices: IPageMetaServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public PageMeta GetPageMeta(ModelType modelType, string objectId)
        {
            return _unitOfWork.PageMetaRepository.GetMany(d => d.ModelType == modelType && d.ObjectId == objectId).FirstOrDefault();
        }
        public PageMeta Create(PageMeta pageMeta)
        {
            return _unitOfWork.PageMetaRepository.Insert(pageMeta);
        }


        public bool Update(PageMeta pageMeta)
        {
            return _unitOfWork.PageMetaRepository.Update(pageMeta);
        }
        public bool Delete(PageMeta pageMeta)
        {
            return _unitOfWork.PageMetaRepository.Delete(pageMeta);
        }
        public void SetPageMeta(ModelType modelType, string objectId, string objectTitle, string title, string keywords, string description)
        {
            var pageMeta = this.GetPageMeta(ModelType.ARTICLE, objectId);
            if (pageMeta != null)
            {               

                if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(keywords) || !string.IsNullOrEmpty(description))
                {
                    pageMeta.ObjectId = objectId;
                    pageMeta.Title = string.IsNullOrEmpty(title) ? objectTitle : title;
                    pageMeta.Keyword = string.IsNullOrEmpty(keywords) ? objectTitle : keywords.Replace('，', ',');
                    pageMeta.Description = description;
                    pageMeta.ModelType = ModelType.ARTICLE;

                    Update(pageMeta);
                }
                else
                {
                    this.Delete(pageMeta);
                }                   
               
            }
            else
            {
                if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(keywords) || !string.IsNullOrEmpty(description))
                {
                    pageMeta = new PageMeta()
                    {
                        ObjectId = objectId,
                        Title = string.IsNullOrEmpty(title) ? objectTitle : title,
                        Keyword = string.IsNullOrEmpty(keywords) ? objectTitle : keywords.Replace('，', ','),
                        Description = description,
                        ModelType = ModelType.ARTICLE
                    };
                    this.Create(pageMeta);
                }
               
            }
            
        }
    }
}
