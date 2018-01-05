using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Articles
{  
    public partial class Article: IAuditedEntity
    {      
        public  int Id { get; set; }
        public  string Title { get; set; }
        public  string Body { get; set; }
        public  string Summary { get; set; }
        public  int CategoryId { get; set; }
        public  DateTime Pubdate { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string UpdatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  int ViewCount { get; set; }
        public  string Thumbnail { get; set; }
        public  bool Recommend { get; set; }
        public  bool Active { get; set; }
        public  string FullImage { get; set; }
        public  string Source { get; set; }
        public string SourceLink { get; set; }
        public bool CanComment { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public ArticleCategory ArticleCategory{ get; set; }

        [NotMapped]       
        public string CategoryTitle
        {
            get { return ArticleCategory!=null ? ArticleCategory.Title: string.Empty; }
        }
    }

}
