using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Model.Front.ViewModel.Outlets
{
    public class OutletFVM
    {
        public int id { get; set; }
        public string Coordinate { get; set; }
        public Callout callout { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string iconPath { get; set; }
    }
    public class Callout {
        public string content { get; set; }
        public int padding { get; set; }
        public int borderRadius { get; set; }
    }
}
