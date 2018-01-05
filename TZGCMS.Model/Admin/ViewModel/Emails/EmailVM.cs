using System;

namespace TZGCMS.Model.Admin.ViewModel.Emails
{
    public class EmailVM
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string MailTo { get; set; }
        public string MailCc { get; set; }
        public bool Readed { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
