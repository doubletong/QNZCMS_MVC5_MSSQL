using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Identity;

namespace TZGCMS.Service.Identity
{
    public interface IRoleServices
    {
        IEnumerable<Role> GetAll();
        Role GetById(int id);
        void SetRoleMenus(int RoleId, int[] menuId);
        bool Update(Role role);
        bool Create(Role role);
        bool Delete(Role role);
    }
    public class RoleServices : IRoleServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<Role> GetAll()
        {
            return _unitOfWork.RoleRepository.GetAll();
        }
        public Role GetById(int id)
        {
            return _unitOfWork.RoleRepository.GetById(id);
        }

        public void SetRoleMenus(int RoleId, int[] menuId)
        {          

            Role vRole = _unitOfWork.RoleRepository.GetById(RoleId);
            vRole.Menus.Clear();
            if(menuId!=null)
                vRole.Menus = _unitOfWork.MenuRepository.GetAll().Where(m => menuId.Contains(m.Id)).ToList();

            this.Update(vRole);

            //var key = $"{EntityNames.Menu}s";
            //_cacheService.Invalidate(key); //取消缓存

        }
        public bool Update(Role role)
        {
            return  _unitOfWork.RoleRepository.Update(role);
        }

        public bool Create(Role role)
        {
            return _unitOfWork.RoleRepository.Insert(role);
        }
        public bool Delete(Role role)
        {
            return _unitOfWork.RoleRepository.Delete(role);
        }
    }
}
