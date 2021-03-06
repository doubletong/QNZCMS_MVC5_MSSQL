﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Identity
{
    public class RegisterIM
    {
        [Remote("IsUserNameUnique", "User", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "UserName")]
        public string UserName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "DisplayName")]
        public string DisplayName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceName = "StringLengthWithMiniLength", ErrorMessageResourceType = typeof(Validations), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Labels), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Labels), Name = "ConfirmPassword")]



        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "ComparePassword")]
        public string ConfirmPassword { get; set; }

        [Remote("IsEmailUnique", "User", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddressInvalidFormat")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Email")]
        public string Email { get; set; }
    }
}
