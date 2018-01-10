using System.Web;
using System.Web.Optimization;

namespace TZGCMS.SiteWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/lib/jquery/plugins/jquery.validate.js",
                     "~/Scripts/lib/jquery/plugins/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalAjax").Include(
                 "~/Scripts/lib/jquery/plugins/jquery.unobtrusive-ajax.js",
               "~/Scripts/lib/jquery/plugins/jquery.validate.js",
                "~/Scripts/lib/jquery/plugins/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalWithBootstrap").Include(
     "~/Scripts/lib/jquery/plugins/jquery.unobtrusive-ajax.js",
      "~/Scripts/lib/jquery/plugins/jquery.validate.js",
       "~/Scripts/lib/jquery/plugins/jquery.validate.unobtrusive.js",
            "~/Scripts/lib/jquery/plugins/jquery.validate.unobtrusive.bootstrap.js"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //================== Admin path ===========================

            bundles.Add(new ScriptBundle("~/bundles/Adminjs").Include(
                      "~/Content/Admin/bootstrap/js/bootstrap.js",
                      "~/Plugins/bootbox/bootbox.js",
                       "~/Plugins/toastr/toastr.js",
                         "~/Content/Admin/js/App.js"
                        ));

            bundles.Add(new StyleBundle("~/Areas/Admin/Content/css/SigStyle").Include(
            "~/Areas/Admin/Content/css/font-awesome.css",
            "~/Areas/Admin/Content/css/style.css",
             "~/Plugins/toastr/toastr.css"
            ));
        }
    }
}
