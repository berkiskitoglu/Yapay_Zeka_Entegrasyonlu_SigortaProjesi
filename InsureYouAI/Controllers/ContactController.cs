using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class ContactController : Controller
    {
        private readonly InsureContext _context;

        public ContactController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult ContactList()
        {
            ViewBag.ControllerName = "İletişim Bilgileri";
            ViewBag.PageName = "Email - Telefon - Adres ve Açıklama Bilgisi";
            var contactList = _context.Contacts.ToList();
            return View(contactList);
        }
        [HttpGet]
        public IActionResult CreateContact()
        {
            ViewBag.ControllerName = "İletişim Sayfası";
            ViewBag.PageName = "Yeni İletişim Bilgisi Ekleme";
            return View();
        }
        [HttpPost]
        public IActionResult CreateContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return RedirectToAction("ContactList");
        }
        [HttpGet]
        public IActionResult UpdateContact(int id)
        {
            ViewBag.ControllerName = "İletişim Sayfası";
            ViewBag.PageName = "İletişim Bilgilerini Güncelleme Sayfası";
            var value = _context.Contacts.Find(id);
            return View(value);
        }
        [HttpPost]
        public IActionResult UpdateContact(Contact contact)
        {
            _context.Contacts.Update(contact);
            _context.SaveChanges();
            return RedirectToAction("ContactList");
        }
        [HttpGet]
        public IActionResult DeleteContact(int id)
        {
            var value = _context.Contacts.Find(id);
            _context.Contacts.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ContactList");
        }
    }
}
