using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, LogInViewModel model)
        {
            DeleteCookies();
            HttpContext.Session.Clear();
            if (ModelState.IsValid)
            {
                model.Password = HashPassword.HashPass(model.Password);
                var validate =  await _context.Users.Where(x => x.email == email).Select(x => x.Password).FirstOrDefaultAsync();
                if (validate == model.Password)
                {
                    //Notify("Success", "Welcome Back!, Log In Successfully!", notificationType: Notifications.NotificationType.success);
                    var id = await _context.Users.Where(x => x.email == email).Select(x => x.user_Id).FirstOrDefaultAsync();
                    var user = await _context.Users.Where(x => x.email == email).Select(x => x.user).FirstOrDefaultAsync();
                    HttpContext.Session.SetInt32("Id",id);                    
                    HttpContext.Session.SetString("UserName",user);                    
                    return RedirectToAction("index", "JournalsViewModel");
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
