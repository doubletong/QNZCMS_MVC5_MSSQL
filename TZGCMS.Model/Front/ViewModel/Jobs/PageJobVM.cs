using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity;

namespace TZGCMS.Model.Front.ViewModel.Jobs
{
    public class PageJobVM
    {
        public Job Job { get; set; }
        public List<Job> Jobs { get; set; }
        public string SeoName { get; set; }
    }
}
