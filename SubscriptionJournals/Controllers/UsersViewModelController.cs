using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return View(await _context.Users.ToListAsync());
        }

        // GET: JournalsViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersViewModel = await _context.Users.FirstOrDefaultAsync();
            if (usersViewModel == null)
            {
                return NotFound();
            }

            return View(usersViewModel);
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
                var id = _context.Users.Where(x => x.user == user).Select(x=>x.user_Id).FirstOrDefault();
                return RedirectToAction("Edit", new { id= id });
            }
            return View(usersViewModel);
        }

       

        // GET: UsersViewModel/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: UsersViewModel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string token, DateTime creationDate, string Password, [Bind("user_Id,user,name,Password,lastName,email,token,creationDate,followers,following,journalsPublished")] UsersViewModel usersViewModel)
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
                            return RedirectToAction("Index", "Home");
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
                        return RedirectToAction("Index", "Home");
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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.FindAsync(id);
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersViewModelExists(int id)
        {
            return _context.Users.Any(e => e.user_Id == id);
        }
    }
}
