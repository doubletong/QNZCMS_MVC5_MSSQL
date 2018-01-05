using System;

namespace TZGCMS.Model.Admin.ViewModel.Emails
{
    public class EmailAccountVM
    {
        public int Id { get; set; }
        public string Email { get; set; }     
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
