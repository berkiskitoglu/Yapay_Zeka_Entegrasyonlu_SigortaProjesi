using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAINew.ViewComponents.BlogViewComponents
{
    public class _BlogListCategoriesComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;
        public _BlogListCategoriesComponentPartial(InsureContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories
                 .Select(c => new CategoryArticleCountViewModel
                 {
                     CategoryId = c.CategoryId,
                     CategoryName = c.CategoryName,
                     ArticleCount = c.Articles.Count()
                 }).ToList();
            return View(categories);
        }
    }
}
