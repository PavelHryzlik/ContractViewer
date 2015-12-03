using System.Web.Mvc;

namespace ContractViewer
{
    /// <summary>
    /// This class registers filter, in our case for handle exceptions
    /// </summary>
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
