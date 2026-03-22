using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace InsureYouAI.ViewComponents.BlogDetailViewComponents
{
    public class _BlogDetailAboutAuthorComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;
        public _BlogDetailAboutAuthorComponentPartial(InsureContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke(int id)
        {
            string appUserId = _context.Articles
                .Where(x => x.ArticleId == id)
                .Select(y => y.AppUserId)
                .FirstOrDefault();

            if (appUserId == null)
                return Content(string.Empty);

            var userValue = _context.Users
                .Where(x => x.Id == appUserId)
                .FirstOrDefault();

            if (userValue == null)
                return Content(string.Empty);

            return View(userValue);
        }
    }
}
