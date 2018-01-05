using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Front;

namespace TZGCMS.Model.Front.ViewModel.Articles
{
    public class CommentFVM
    {
        public int Id { get; set; }
        
        public string Body { get; set; }
       
        public int ArticleId { get; set; }

        public DateTime Pubdate { get; set; }
       
        public string Name { get; set; }

    }
}
