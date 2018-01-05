using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Enums;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Menus
{
    public class FrontMenuIM
    {
        public FrontMenuIM()
        {
            this.ChildMenus = new HashSet<FrontMenuIM>();
            this.MenuType = MenuType.PAGE;
        }

        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "MenuName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Link")]
        public string Url { get; set; }

        [Display(Name = "类型", Prompt = "类型")]
        public MenuType MenuType { get; set; }

        [Display(Name = "图标")]
        public string Iconfont { get; set; }
        [Display(Name = "排序", Prompt = "排序")]
        [Required(ErrorMessage = "请输入排序")]
        public int Importance { get; set; }
        [Display(Name = "激活", Prompt = "激活")]
        public bool Active { get; set; }
        [Display(Name = "父级菜单")]
        public int? ParentId { get; set; }
        public int CategoryId { get; set; }
        public virtual ICollection<FrontMenuIM> ChildMenus { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }
    }
}
