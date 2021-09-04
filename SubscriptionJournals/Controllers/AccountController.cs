using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubscriptionJournals.Models;
using SubscriptionJournals.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionJournals.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {            
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = HashPassword.HashPass(model.Password);
                var validate =  _context.Users.Where(x => x.email == email).Select(x => x.Password).FirstOrDefault();
                if (validate == model.Password)
                {
                    Notify("Success", "Welcome Back!, Log In Successfully!", notificationType: Notifications.NotificationType.success);
                    return RedirectToAction("index", "home");
                }
                else
                {
                    Notify("Error", "User LogIn Failed, please verify your email / password", notificationType: Notifications.NotificationType.error);
                    return View(model);
                }
            }

            return View(model);
        }
    }
}
