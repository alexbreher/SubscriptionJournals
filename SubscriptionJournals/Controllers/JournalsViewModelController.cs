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

namespace SubscriptionJournals.Controllers
{
    public class JournalsViewModelController : Controller
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
            var applicationDbContext = _context.Journals.Include(j => j.Users);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: JournalsViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: JournalsViewModels/Create
        public IActionResult Create()
        {
            ViewData["author"] = new SelectList(_context.Users, "user_Id", "name");
            return View();
        }

        // POST: JournalsViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("journal_Id,name,author,creationDate,path,pdf")] JournalsViewModel journalsViewModel)
        {
            if (ModelState.IsValid)
            {
                //string uniqueFileName = null;
                //string filePath = null;
                //if (journalsViewModel.pdf != null)
                //{
                //    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "journals/");
                //    uniqueFileName = Guid.NewGuid().ToString() + "_" + journalsViewModel.pdf.FileName;
                //    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //    journalsViewModel.pdf.CopyTo(new FileStream(filePath, FileMode.Create));
                //}                 
                if (journalsViewModel.pdf != null)
                {
                    string folder = "journals/";
                    journalsViewModel.path = await UploadPDF(folder, journalsViewModel.pdf);
                }

                journalsViewModel.creationDate = DateTime.Now;
                _context.Add(journalsViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["author"] = new SelectList(_context.Users, "user_Id", "name", journalsViewModel.author);
            return View(journalsViewModel);
        }

        // GET: JournalsViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalsViewModel = await _context.Journals.FindAsync(id);
            if (journalsViewModel == null)
            {
                return NotFound();
            }
            ViewData["author"] = new SelectList(_context.Users, "user_Id", "name", journalsViewModel.author);
            return View(journalsViewModel);
        }

        // POST: JournalsViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("journal_Id,name,author,creationDate,path")] JournalsViewModel journalsViewModel)
        {
            if (id != journalsViewModel.journal_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journalsViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalsViewModelExists(journalsViewModel.journal_Id))
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
            ViewData["author"] = new SelectList(_context.Users, "user_Id", "name", journalsViewModel.author);
            return View(journalsViewModel);
        }

        // GET: JournalsViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: JournalsViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalsViewModel = await _context.Journals.FindAsync(id);
            var filepath =  _context.Journals.Where(x => x.journal_Id == id).Select(x => x.path).FirstOrDefault();            
            if (filepath !=null)
            {
                string wwwpath = this._hostingEnvironment.WebRootPath;
                string serverFolder = wwwpath + filepath;
                System.IO.File.Delete(serverFolder);
            }                
            _context.Journals.Remove(journalsViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalsViewModelExists(int id)
        {
            return _context.Journals.Any(e => e.journal_Id == id);
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
