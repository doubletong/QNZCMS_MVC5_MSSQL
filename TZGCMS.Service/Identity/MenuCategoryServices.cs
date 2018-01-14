using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Identity;

namespace TZGCMS.Service.Identity
{
    public interface IMenuCategoryServices
    {
        MenuCategory GetById(int id);
        IEnumerable<MenuCategory> GetAll();
        bool Update(MenuCategory category);
        MenuCategory Create(MenuCategory category);
        bool Delete(MenuCategory category);
    }
    public class MenuCategoryServices: IMenuCategoryServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public MenuCategory GetById(int id)
        {
            return _unitOfWork.MenuCategoryRepository.GetById(id);
        }

        public IEnumerable<MenuCategory> GetAll()
        {
            return _unitOfWork.MenuCategoryRepository.GetAll();
        }

        public bool Update(MenuCategory category)
        {
            return _unitOfWork.MenuCategoryRepository.Update(category);
        }

        public MenuCategory Create(MenuCategory category)
        {
            return _unitOfWork.MenuCategoryRepository.Insert(category);
        }
        public bool Delete(MenuCategory category)
        {
            return _unitOfWork.MenuCategoryRepository.Delete(category);
        }
    }
}
