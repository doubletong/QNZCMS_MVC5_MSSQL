using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Enums;
using TZGCMS.Model.Admin.ViewModel;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    public class QNZBaseController : Controller
    {
        // GET: Admin/QNZBase
        public AjaxResultVM AR = new AjaxResultVM();
    
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }

        protected string GetModelErrorMessage()
        {
            string validationErrors = string.Join("；",
                    ModelState.Values.Where(E => E.Errors.Count > 0)
                    .SelectMany(E => E.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray());
            return validationErrors;
        }


        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public async Task SetPageMetaAsync(IQNZDbContext db, short modelType, string objectId, string objectTitle, string title, string keywords, string description)
        {
            var pageMeta = await db.PageMetas.FirstOrDefaultAsync(d=>d.ModelType == modelType && d.ObjectId == objectId);
            if (pageMeta != null)
            {

                if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(keywords) || !string.IsNullOrEmpty(description))
                {
                    pageMeta.ObjectId = objectId;
                    pageMeta.Title = string.IsNullOrEmpty(title) ? objectTitle : title;
                    pageMeta.Keyword = string.IsNullOrEmpty(keywords) ? objectTitle : keywords.Replace('，', ',');
                    pageMeta.Description = description;
                    pageMeta.ModelType = modelType;

                    db.Entry(pageMeta).State = EntityState.Modified;
                }
                else
                {
                    db.PageMetas.Remove(pageMeta);
                    
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(keywords) || !string.IsNullOrEmpty(description))
                {
                    pageMeta = new PageMeta()
                    {
                        ObjectId = objectId,
                        Title = string.IsNullOrEmpty(title) ? objectTitle : title,
                        Keyword = string.IsNullOrEmpty(keywords) ? objectTitle : keywords.Replace('，', ','),
                        Description = description,
                        ModelType = modelType
                    };
                    db.PageMetas.Add(pageMeta);
                }

            }
            await db.SaveChangesAsync();

        }


    }
}