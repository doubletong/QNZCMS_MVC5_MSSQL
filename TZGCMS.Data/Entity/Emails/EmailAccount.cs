using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TZGCMS.Data.Entity.Emails
{
    /// <summary>
    /// A class which represents the EmailAccountSet table.
    /// </summary>
	[Table("EmailAccountSet")]
    public partial class EmailAccount
    {
        public EmailAccount()
        {          
            this.EmailTemplates = new HashSet<EmailTemplate>();
        }
        public  int Id { get; set; }
        public  string Email { get; set; }
        public  string Smtpserver { get; set; }
        public  string UserName { get; set; }
        public  string Password { get; set; }
        public  int Port { get; set; }
        public  bool EnableSsl { get; set; }
        public  bool IsDefault { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  string UpdatedBy { get; set; }
        public virtual ICollection<EmailTemplate> EmailTemplates { get; set; }
    }
}
