using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.DefaultViewComponents
{
    public class _DefaultPricingPlanComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultPricingPlanComponentPartial(InsureContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var values = _context.PricingPlans
                .Include(x => x.PricingPlanItems)
                .Where(x => x.IsFeatured == true)
                .ToList(); 
            return View(values);
        }
    }
}
