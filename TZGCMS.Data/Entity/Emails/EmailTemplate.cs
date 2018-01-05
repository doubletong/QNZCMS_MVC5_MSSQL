using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TZGCMS.Data.Entity.Emails
{
    /// <summary>
    /// A class which represents the EmailTemplateSet table.
    /// </summary>
    [Table("EmailTemplateSet")]
    public partial class EmailTemplate
    {        
        public  int Id { get; set; }
        public  string Subject { get; set; }
        public  string Body { get; set; }
        public  string TemplateNo { get; set; }
        public  int EmailAccountId { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string UpdatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  EmailAccount EmailAccount { get; set; }

        [NotMapped]     
        public string Email
        {
            get
            {
                if (this.EmailAccount != null)
                {
                    return this.EmailAccount.Email;
                }
                return string.Empty;
            }
        }
    }


}
