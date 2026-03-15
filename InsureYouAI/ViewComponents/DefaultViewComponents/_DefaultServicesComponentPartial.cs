using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DefaultViewComponents
{
    public class _DefaultServicesComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultServicesComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var servicesList = _context.Services.ToList();
            return View(servicesList);
        }
    }
}
