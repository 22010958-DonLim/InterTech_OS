using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using StrawberryHub.Models;
using StrawberryHub.Services;

namespace StrawberryHub.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        private const string REGISTER_VIEW = "Register";
        private const string REDIRECT_CNTR = "Users";
        private const string REDIRECT_ACTN = "Index";

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.User.Include(u => u.Rank);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Rank)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RankId"] = new SelectList(_context.Rank, "RankId", "RankId");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,FirstName,LastName,Email,Points,RankId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewData["RankId"] = new SelectList(_context.Rank, "RankId", "RankId", user.RankId);
            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null!)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(REGISTER_VIEW);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterUser user)
        {
            IFormCollection form = HttpContext.Request.Form;

            string thePassword = form["Password"].ToString().Trim();
            //string thePassword = user.UserPassword;

            // Assuming user.UserPassword is a byte array
            // byte[] passwordBytes = user.UserPassword;

            // Convert byte array to string using UTF-8 encoding
            //string thePassword = Encoding.UTF8.GetString(passwordBytes);


            byte[] passwordHash;
            using (var sha1 = SHA1.Create())
            {
                passwordHash =
                    sha1.ComputeHash(Encoding.UTF8.GetBytes(thePassword));
            }

            User newUser = new User();

            newUser.Password = passwordHash;
            newUser.Email = user.Email;
            newUser.FirstName = user.FirstName!;
            newUser.LastName = user.LastName;
            newUser.Username = user.Username;
            newUser.Points = 0;
            newUser.RankId = 1;
            //newUser.DateOfReg = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);

        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RankId"] = new SelectList(_context.Rank, "RankId", "RankId", user.RankId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,FirstName,LastName,Email,Points,RankId")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            ViewData["RankId"] = new SelectList(_context.Rank, "RankId", "RankId", user.RankId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Rank)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'AppDbContext.User'  is null.");
            }

            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);

                //Remove the UserID in Task as well (Task contain foregin Key of UserID)
                var tasksToRemove = _context.Task.Where(t => t.UserId == id);
                _context.Task.RemoveRange(tasksToRemove);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.User?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
