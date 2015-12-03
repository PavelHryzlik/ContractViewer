using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ContractViewer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application start
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
