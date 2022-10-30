using System.Web;
using System.Web.Optimization;

namespace Portal
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/Site.css", "~/Content/select2.min.css","~/Content/adminlte.css", "~/Content/icheck-bootstrap.min.css", "~/Content/sweetalert2.min.css",
                "~/Content/jquery.timepicker.min.css"));

            bundles.Add(new StyleBundle("~/Content/dataTables").Include("~/Content/datatables/dataTables.bootstrap4.min.css", "~/Content/datatables/responsive.bootstrap4.min.css",
                                         "~/Content/datatables/buttons.bootstrap4.min.css", "~/Content/datatables/fixedColumns.bootstrap4.min.css", "~/Content/datatables/select.bootstrap4.min.css"));


            bundles.Add(new StyleBundle("~/Content/icons").Include("~/Content/fontawesome/css/all.min.css", "~/Content/fontawesome/css/fontawesome.min.css")); 


            bundles.Add(new ScriptBundle("~/Scripts/libs").Include("~/Scripts/jquery.min.js", "~/Scripts/bootstrap.bundle.min.js", "~/Scripts/select2.min.js","~/Scripts/adminlte.min.js", "~/Scripts/sweetalert2.min.js", "~/Scripts/jquery.timepicker.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/dataTables").Include("~/Scripts/datatables/jquery.dataTables.min.js","~/Scripts/datatables/dataTables.bootstrap4.min.js",
                                    "~/Scripts/datatables/dataTables.responsive.min.js", "~/Scripts/datatables/responsive.bootstrap4.min.js",
                                    "~/Scripts/datatables/dataTables.buttons.min.js", "~/Scripts/datatables/dataTables.fixedColumns.min.js","~/Scripts/datatables/dataTables.select.min.js", "~/Scripts/datatables/jszip.min.js", "~/Scripts/datatables/buttons.html5.min.js", "~/Scripts/datatables/buttons.print.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
        }
    }
}
