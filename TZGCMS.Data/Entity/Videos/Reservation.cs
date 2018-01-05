using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Videos
{
    public class Reservation
    {

        public int VideoId { get; set; }
        public string OpenId { get; set; }
     
        public DateTime? NoticedDate { get; set; }
        public DateTime CreatedDate { get; set; }
       
        public virtual Video Video { get; set; }
        [NotMapped]
        public string VideoTitle => Video != null ? Video.Title : string.Empty;

        public string Status => NoticedDate == null ? "未通知" : "已通知";


    }
}
