using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Model.Admin.ViewModel.Chronicles
{
   public  class ChronicleVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public short? Day { get; set; }
        public bool Active { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
