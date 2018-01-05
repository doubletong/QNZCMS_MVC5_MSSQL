using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Model.Inputs.Articles
{
    public class ArticleIM
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
       // public virtual ArticleCategory ArticleCategory { get; set; }

    }
}
