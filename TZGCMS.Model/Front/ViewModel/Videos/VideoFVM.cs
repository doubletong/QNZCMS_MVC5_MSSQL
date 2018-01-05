using System;
using TZGCMS.Infrastructure.Configs;

namespace TZGCMS.Model.Front.ViewModel.Videos
{
    public class VideoFVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string VideoUrl { get; set; }
        public string Thumbnail { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartDateFormat => $"{StartDate:yyyy-MM-dd HH:mm}";

        public string EndDateFormat => $"{EndDate:yyyy-MM-dd HH:mm}";
        public bool? IsReservation { get; set; }
        public string Status
        {
            get
            {
                if (DateTime.Now < StartDate)
                    return "预告中";
                else if (DateTime.Now >= StartDate && DateTime.Now <= EndDate)
                {
                    return "直播中";
                }
                else
                {
                    return "直播结束";
                }

            }
        }

        public string ThumbnailFull => SettingsManager.Site.SiteDomainName + Thumbnail + "?width=360&height=240&mode=crop";
    }
}
