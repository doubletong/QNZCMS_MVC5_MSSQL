using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Articles
{

    public partial class FilterTemplate
    {
       
        public  int Id { get; set; }
        public  string Name { get; set; }
        public  string Title { get; set; }
        public  string Body { get; set; }
        public  string Description { get; set; }
        public  string Author { get; set; }
        public  string PublishDate { get; set; }
        public  string Keyword { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string UpdatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  bool Active { get; set; }
        public  string Source { get; set; }
        public  string LinksContainer { get; set; }
        public  int Importance { get; set; }
        public  string Links { get; set; }
        public  string KeywordSet { get; set; }
        public  string Encode { get; set; }
    }
}
