using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class LuceneListVM
    {
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool SearchDefault { get; set; }
        public StaticPagedList<SearchData> SearchIndexData { get; set; }
        public IList<SelectedList> SearchFieldList { get; set; }
        public string SearchTerm { get; set; }
        public string SearchField { get; set; }
    }
    public class SelectedList
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class SearchData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class SearchDataIM
    {
        public string Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ImageURL")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string ImageUrl { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Url")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Url { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Description { get; set; }
    }

    public class SearchListVM
    {

        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        //   public bool SearchDefault { get; set; }
        public StaticPagedList<SearchData> SearchIndexData { get; set; }
        //   public IList<SelectedList> SearchFieldList { get; set; }
        public string SearchTerm { get; set; }
        //   public string SearchField { get; set; }
    }
}
