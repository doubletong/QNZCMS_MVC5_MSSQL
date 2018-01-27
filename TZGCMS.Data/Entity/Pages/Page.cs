using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Pages
{
    public class Page : IAuditedEntity
    {
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string SeoName { get; set; }
        public string TemplateName { get; set; }
        public string HeadCode { get; set; }
        public string FooterCode { get; set; }
        public int ViewCount { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
