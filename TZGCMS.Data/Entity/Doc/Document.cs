using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Doc
{
    public class Document : IAuditedEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Decimal? FileSize { get; set; }
        public string Extension { get; set; }
        public string FilePath { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }
        public bool IsVIP { get; set; }
        public bool IsLink { get; set; }
        public int DownloadCount { get; set; }
        public int CategoryId { get; set; }
        public string ProductIds { get; set; }
        public string Password { get; set; }
        public virtual DocumentCategory Category { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string CategoryTitle
        {
            get
            {
                if (this.Category != null)
                {
                    return this.Category.Title;
                }
                return string.Empty;
            }
        }


    }
}
