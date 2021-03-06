﻿using System;
using System.Collections.Generic;


namespace TZGCMS.Data.Entity.Articles
{

    public partial class ArticleCategory : IAuditedEntity
    {
      
        public  int Id { get; set; }
        public  string Title { get; set; }
        public  int Importance { get; set; }
        public  bool Active { get; set; }
     
        public  string SeoName { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  string UpdatedBy { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
    }
}
