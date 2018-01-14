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
    }
}
