﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Model.Validation;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Identity
{
    public class ProfileIM
    {
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public Guid Id { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "UserName")]
        public string UserName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "RealName")]
        public string RealName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Birthday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddressInvalidFormat")]
        [Remote("IsEmailUniqueAtEdit", "User", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool IsActive { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Gender")]
        public Gender Gender { get; set; }

        [QQ]
        public string QQ { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Mobile")]
        [ChinaMobile]
        public string Mobile { get; set; }
    }
}
