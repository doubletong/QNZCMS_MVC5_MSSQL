using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TZGCMS.Data.Entity.Articles
{
    public class Comment
    {
        public int Id { get; set; }   
        public string Body { get; set; }
        public DateTime Pubdate { get; set; }
        public string Name { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public bool Active { get; set; }

        [NotMapped]
        public string ArticleTitle => Article != null ? Article.Title : string.Empty;
    }
}