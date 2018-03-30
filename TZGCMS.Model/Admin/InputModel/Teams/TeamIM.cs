using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Teams
{
    public class TeamIM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Post")]
        public string Post { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Photo")]
        public string PhotoUrl { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string Description { get; set; }
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "ValidNumber")]
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        public bool Active { get; set; }
    }
}
