using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TZGCMS.Data.Entity.Emails
{
    /// <summary>
    /// A class which represents the EmailSet table.
    /// </summary>
    [Table("EmailSet")]
    public partial class Email
    {
       
        public  int Id { get; set; }
        public  string Subject { get; set; }
        public  string Body { get; set; }
        public  string MailTo { get; set; }
        public  string MailCc { get; set; }
        public  bool Readed { get; set; }
        public  bool Active { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  string UpdatedBy { get; set; }
    }
}
