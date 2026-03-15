using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;

namespace InsureYouAI.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutHeadComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
