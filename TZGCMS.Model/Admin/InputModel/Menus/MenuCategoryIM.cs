﻿using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Menus
{
    public class MenuCategoryIM
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }
        
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

    }
}
