using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SubscriptionJournals;
using SubscriptionJournals.Models;
using SubscriptionJournals.Tools;

namespace SubscriptionJournals.Controllers
{
    public class UsersViewModelController : BaseController
    {
        //DBContext dependency Injection
        private readonly ApplicationDbContext _context;

        public UsersViewModelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UsersViewModel
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Users.ToListAsync());DeleteCookies();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // GET: JournalsViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {            
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            //if session variable is not equal to id
            if(id_user != 0)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var usersViewModel = await _context.Users.FirstOrDefaultAsync(m => m.user_Id == id);
                if (usersViewModel == null)
                {
                    return NotFound();
                }
                if (id_user == id)
                {
                    //current user is viewing his data
                    ViewBag.CurrentUser = "True";
                    return View(usersViewModel);
                }
                else
                {
                    //another user is viewing the data                
                    var verifysubscirption = await _context.Subscriptions.Where(x => x.subscriptor == id_user && x.subscribesTo == id).Select(x => x.subscribesTo).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(verifysubscirption.ToString()) || verifysubscirption != 0)
                    {
                        //is a follower of the current user
                        ViewBag.Follower = "True";
                        return View(usersViewModel);
                    }
                    return View(usersViewModel);
                }
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

        }
        public async Task<IActionResult> DetailsForUsers(int? id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            //if session variable is not equal to id
            if (id_user != 0)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var usersViewModel = await _context.Users.FirstOrDefaultAsync(m => m.user_Id == id);
                if (usersViewModel == null)
                {
                    return NotFound();
                }
                if (id_user == id)
                {
                    //current user is viewing his data
                    ViewBag.CurrentUser = "True";
                    return View(usersViewModel);
                }
                else
                {
                    //another user is viewing the data                
                    var verifysubscirption = await _context.Subscriptions.Where(x => x.subscriptor == id_user && x.subscribesTo == id).Select(x => x.subscribesTo).FirstOrDefaultAsync();
                    if (verifysubscirption != 0)
                    {
                        //is a follower of the current user
                        ViewBag.Follower = "True";
                        return View(usersViewModel);
                    }
                    return View(usersViewModel);
                }
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

        }

        // GET: UsersViewModel/Create
        public IActionResult Create()
        {
                return View();
        }

        // POST: UsersViewModel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string user,[Bind("user_Id,user,name,Password,lastName,email,token,creationDate,followers,following,journalsPublished")] UsersViewModel usersViewModel)
        {
                if (ModelState.IsValid)
                {
                    //(Initialize) add default values to this fields to prevent Bugs with data
                    usersViewModel.creationDate = DateTime.Now;
                    usersViewModel.followers = 0;
                    usersViewModel.following = 0;
                    usersViewModel.journalsPublished = 0;
                    //hashing the password provided by user
                    usersViewModel.Password = HashPassword.HashPass(usersViewModel.Password);
                    //generating token for recover password
                    var userTokenStr = GenerateUserToken.RandomString();
                    //hashing the token for security purposes
                    usersViewModel.token = HashPassword.HashPass(userTokenStr);
                    _context.Add(usersViewModel);
                    await _context.SaveChangesAsync();
                    Notify("Success", "User Created Successfully", notificationType: Notifications.NotificationType.success);
                    //get the id from current user registered
                    var id = await _context.Users.Where(x => x.user == user).Select(x => x.user_Id).FirstOrDefaultAsync();
                    //return RedirectToAction("Edit", new { id = id });
                return RedirectToAction("Login", "Account");
                }
                return View(usersViewModel);
            
        }

       

        // GET: UsersViewModel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var usersViewModel = await _context.Users.FindAsync(id);
                if (usersViewModel == null)
                {
                    return NotFound();
                }
                return View(usersViewModel);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: UsersViewModel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string token, DateTime creationDate, string Password, [Bind("user_Id,user,Password,name,lastName,email,token,creationDate,followers,following,journalsPublished,NewPassword")] UsersViewModel usersViewModel)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                if (id != usersViewModel.user_Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(usersViewModel.NewPassword))
                        {
                            usersViewModel.NewPassword = HashPassword.HashPass(usersViewModel.NewPassword);
                            if (Password != usersViewModel.NewPassword)
                            {
                                usersViewModel.Password = usersViewModel.NewPassword;
                                _context.Update(usersViewModel);
                                await _context.SaveChangesAsync();
                                Notify("Success", "User Update Success", notificationType: Notifications.NotificationType.success);
                                return RedirectToAction("Index", "JournalsViewModel");
                            }
                            else
                            {
                                if (Password == usersViewModel.Password)
                                {
                                    Notify("Error", "You can't use the same password, please try again with another one", notificationType: Notifications.NotificationType.error);
                                    return RedirectToAction("Edit");
                                }
                            }
                        }
                        else
                        {
                            usersViewModel.Password = Password;
                            _context.Update(usersViewModel);
                            await _context.SaveChangesAsync();
                            //Notify("Success", "User Update data Success", notificationType: Notifications.NotificationType.success);
                            return RedirectToAction("Index", "JournalsViewModel");
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsersViewModelExists(usersViewModel.user_Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(usersViewModel);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var users = await _context.Users
                    .FirstOrDefaultAsync(m => m.user_Id == id);
                if (users == null)
                {
                    return NotFound();
                }

                return View(users);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                var users = await _context.Users.FindAsync(id);
                _context.Users.Remove(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }

        private bool UsersViewModelExists(int id)
        {
            return _context.Users.Any(e => e.user_Id == id);
        }
    }
}
