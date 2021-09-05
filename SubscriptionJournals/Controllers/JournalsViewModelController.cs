using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SubscriptionJournals;
using SubscriptionJournals.Models;
using SubscriptionJournals.Tools;

namespace SubscriptionJournals.Controllers
{
    public class JournalsViewModelController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public JournalsViewModelController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: JournalsViewModels
        public async Task<IActionResult> Index()
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                var Posts = await _context.Journals.Include(j => j.Users).ToListAsync();
                return View(Posts);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear(); 
                return RedirectToAction("Login", "Account");
            }
            
        }
        public async Task<IActionResult> IndexMyPosts()
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                var Posts = await _context.Journals.Where(x => x.author == id_user).Include(j => j.Users).ToListAsync();
                return View(Posts);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }
        public async Task<IActionResult> IndexFollowing()
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                
                var PostFollowing = await (from subs in _context.Subscriptions
                                           join Jour in _context.Journals
                                           on subs.subscribesTo equals Jour.author
                                           join User in _context.Users
                                           on Jour.author equals User.user_Id
                                           select new IndexFollowersViewModel
                                           {
                                               name = Jour.name,
                                               creationDate = Jour.creationDate,
                                               user = User.user,
                                               journal_Id = Jour.journal_Id,
                                               user_Id = User.user_Id,
                                               subscribesTo = subs.subscribesTo,
                                               subscriptor = subs.subscriptor
                                           }).Where(x => x.subscriptor == id_user).ToListAsync();

                return View(PostFollowing);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

        }
        //}
        // GET: JournalsViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var journalsViewModel = await _context.Journals
                    .Include(j => j.Users)
                    .FirstOrDefaultAsync(m => m.journal_Id == id);
                if (journalsViewModel == null)
                {
                    return NotFound();
                }

                return View(journalsViewModel);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: JournalsViewModels/Create
        public IActionResult Create()
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                return View();
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: JournalsViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("journal_Id,name,author,creationDate,path,pdf")] JournalsViewModel journalsViewModel)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                if (ModelState.IsValid)
                {
                    if (journalsViewModel.pdf != null)
                    {
                        string folder = "journals/";
                        journalsViewModel.path = await UploadPDF(folder, journalsViewModel.pdf);
                    }
                    journalsViewModel.author = id_user;
                    journalsViewModel.creationDate = DateTime.Now;
                    _context.Add(journalsViewModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(journalsViewModel);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

        }

        // GET: JournalsViewModels/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var journalsViewModel = await _context.Journals.FindAsync(id);
        //    if (journalsViewModel == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["author"] = new SelectList(_context.Users, "user_Id", "name", journalsViewModel.author);
        //    return View(journalsViewModel);
        //}

        //// POST: JournalsViewModels/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("journal_Id,name,author,creationDate,path")] JournalsViewModel journalsViewModel)
        //{
        //    if (id != journalsViewModel.journal_Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(journalsViewModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!JournalsViewModelExists(journalsViewModel.journal_Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["author"] = new SelectList(_context.Users, "user_Id", "name", journalsViewModel.author);
        //    return View(journalsViewModel);
        //}

        // GET: JournalsViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var journalsViewModel = await _context.Journals
                    .Include(j => j.Users)
                    .FirstOrDefaultAsync(m => m.journal_Id == id);
                if (journalsViewModel == null)
                {
                    return NotFound();
                }

                return View(journalsViewModel);
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: JournalsViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                var journalsViewModel = await _context.Journals.FindAsync(id);
                var filepath = _context.Journals.Where(x => x.journal_Id == id).Select(x => x.path).FirstOrDefault();
                if (filepath != null)
                {
                    string wwwpath = this._hostingEnvironment.WebRootPath;
                    string serverFolder = wwwpath + filepath;
                    System.IO.File.Delete(serverFolder);
                }
                _context.Journals.Remove(journalsViewModel);
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

        private bool JournalsViewModelExists(int id)
        {
            return _context.Journals.Any(e => e.journal_Id == id);
        }

        public async Task<IActionResult> Subscribe(int id, [Bind("subscription_Id,subscriptor,subscribesTo,subscriptionDate")] SubscriptionsViewModel subscriptionsViewModel)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                if (ModelState.IsValid)
                {
                    subscriptionsViewModel.subscriptor = id_user;
                    subscriptionsViewModel.subscribesTo = id;
                    subscriptionsViewModel.subscriptionDate = DateTime.Now;
                    _context.Add(subscriptionsViewModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DetailsForUsers", "UsersViewModel",new {id = id});
                }

                return RedirectToAction("DetailsForUsers", "UsersViewModel", new { id = id });
            }
            else
            {
                DeleteCookies();
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

        }
        public async Task<IActionResult> UnSuscribe(int id)
        {
            int id_user = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            if(id_user != 0)
            {
                var subscriptionsViewModel = await _context.Subscriptions.Where(x => x.subscriptor == id_user && x.subscribesTo == id).FirstOrDefaultAsync();
                _context.Subscriptions.Remove(subscriptionsViewModel);
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
        private async Task<string> UploadPDF(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostingEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }
    }
}
