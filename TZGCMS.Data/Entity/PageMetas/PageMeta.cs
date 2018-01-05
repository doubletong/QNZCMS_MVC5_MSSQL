using TZGCMS.Data.Enums;

namespace TZGCMS.Data.Entity.PageMetas
{
    public class PageMeta
    {
        public int Id { get; set; }
        public ModelType ModelType { get; set; }
        public string ObjectId { get; set; }
        public string Title { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
    }
}
