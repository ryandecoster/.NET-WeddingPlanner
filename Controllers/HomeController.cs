using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WeddingPlanner.Controllers
{
    
    public class HomeController : Controller
    {
        private YourContext _context;
        public HomeController(YourContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            // if user is not logged in and tries to go to this route then redirect back to home
            if(HttpContext.Session.GetInt32("id") == null) {
                return RedirectToAction("Index", "User");
            }
            // include a view model that allows all the models to display on one page
            DashboardModel view = new DashboardModel()
            {
                Users = new User(),
                Weddings = new Wedding(),
                Invites = new Invite()
            };
            int? user_id = HttpContext.Session.GetInt32("id");
            User curruser = _context.Users.Where(u => u.Id == user_id).SingleOrDefault();
            List<Wedding> allWeddings = _context.Weddings
                                        .Include(w => w.Guests)
                                        .ThenInclude(g => g.InvitedUser)
                                        .ToList();
            List<Invite> allInvites = _context.Invites
                                    .Include(w => w.InvitedUser).ToList();
            // store lists and user in viewbag to display on page
            ViewBag.Invites = allInvites;
            ViewBag.UserId = user_id;
            ViewBag.User = curruser;
            ViewBag.Weddings = allWeddings;
            return View(view);
        }

        [HttpGet]
        [Route("NewWedding")]
        public IActionResult NewWedding()
        {
            if(HttpContext.Session.GetInt32("id") == null) {
                return RedirectToAction("Index", "User");
            }
            return View();
        }

        [HttpPost]
        [Route("CreateWedding")]
        public IActionResult CreateWedding(NewWedding wedding)
        {
            if(HttpContext.Session.GetInt32("id") == null) {
                return RedirectToAction("Index", "User");
            }
            int? user_id = HttpContext.Session.GetInt32("id");
            if(ModelState.IsValid) {
                Wedding newWedding = new Wedding
                {
                    Groom = wedding.Groom,
                    Bride = wedding.Bride,
                    Date = wedding.Date,
                    Address = wedding.Address,
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now,
                    UserId = (int) user_id       
                };
                _context.Add(newWedding);
                _context.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }
            return View("NewWedding"); 
        }

        [HttpGet]
        [Route("Wedding/{WeddingId}")]
        public IActionResult Wedding(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("id") == null) {
                return RedirectToAction("Index", "User");
            }
            // include guests lists and then inclide invited user within the guests list
            Wedding wedding = _context.Weddings
                            .Include(w => w.Guests)
                            .ThenInclude(g => g.InvitedUser)
                            .SingleOrDefault(w => w.Id == WeddingId);
            // store current wedding and all the guests in viewbag to display on page
            ViewBag.CurrentWedding = wedding;
            ViewBag.WeddingGuests = wedding.Guests;
            return View();
        }

        [HttpGet]
        [Route("Delete/{WeddingId}")]
        public IActionResult Delete(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("id") == null) {
                return RedirectToAction("Index", "User");
            }
            // delete wedding by id
            Wedding wedding = _context.Weddings
                            .Where(w => w.Id == WeddingId).SingleOrDefault();
            _context.Weddings.Remove(wedding);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("RSVP/{WeddingId}")]
        public IActionResult RSVP(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("id") == null) {
                return RedirectToAction("Index", "User");
            }
            int? user_id = HttpContext.Session.GetInt32("id");
            // grab user id from session and add it to the wedding guest list
            User curruser = _context.Users.Where(u => u.Id == user_id).SingleOrDefault();
            Wedding wedding = _context.Weddings
                            .Include(w => w.Guests)
                            .ThenInclude(g => g.InvitedUser)
                            .SingleOrDefault(w => w.Id == WeddingId);
            Invite newInvite = new Invite
            {
                UserId = curruser.Id,
                InvitedUser = curruser,
                WeddingId = wedding.Id,
                Wedding = wedding
            };
            wedding.Guests.Add(newInvite);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("Undo/{WeddingId}")]
        public IActionResult Undo(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("id") == null) {
                return RedirectToAction("Index", "User");
            }
            int? user_id = HttpContext.Session.GetInt32("id");
            // grab user id from session and remove it from guest list
            User curruser = _context.Users.Where(u => u.Id == user_id).SingleOrDefault();
            Wedding wedding = _context.Weddings
                            .Include(w => w.Guests)
                            .ThenInclude(g => g.InvitedUser)
                            .SingleOrDefault(w => w.Id == WeddingId);
            Invite invite = _context.Invites.Where(i => i.UserId == user_id).Where(i => i.WeddingId == WeddingId).SingleOrDefault();
            wedding.Guests.Remove(invite);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}

