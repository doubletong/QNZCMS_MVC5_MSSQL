using System;

namespace TZGCMS.Model.Admin.ViewModel.Emails
{
    public class EmailTemplateVM
    {
        public int Id { get; set; }
        public string Subject { get; set; }     
        public string TemplateNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Email { get; set; }
    }
}
