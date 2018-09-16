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
    
    public class UserController : Controller
    {
        private YourContext _context;
        public UserController(YourContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser(RegisterUser userReg)
        {
            // if email already exists in database then throw error
            if(_context.Users.Where(user => user.Email == userReg.Email).FirstOrDefault() != null)
            {
                // System.Console.Beep(1500, 30000);
                ModelState.AddModelError("Email", "Email already in use.");
            }
            // declare variable that stores the hashed password
            PasswordHasher<RegisterUser> hasher = new PasswordHasher<RegisterUser>();
            if(ModelState.IsValid)
            {
                User newUser = new User
                {
                    First_Name = userReg.First_Name,
                    Last_Name = userReg.Last_Name,
                    Email = userReg.Email,
                    // set password as the new hashed password in the database
                    Password = hasher.HashPassword(userReg, userReg.Password),
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now,          
                };
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("id", newUser.Id);
                int? user_id = newUser.Id;
                System.Console.WriteLine(user_id);
                return RedirectToAction ("Dashboard", "Home");
            };
            return View("Index");
        }
        [Route("login")]
         public IActionResult LoginUser(LoginUser userLog)
        {
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            User loginUser = _context.Users.Where(user => user.Email == userLog.LogEmail).SingleOrDefault();
            // if any field is left empty then throw error
            if(loginUser == null)
            {
                ModelState.AddModelError("LogEmail", "Invalid Email/Password.");
            }
            // if the hashed password in the database does not match the password entered then throw error
            else if(hasher.VerifyHashedPassword(userLog, loginUser.Password, userLog.LogPassword) == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid Email/Password.");
            }
            if(!ModelState.IsValid)
            {
                return View("Index");
            }
            HttpContext.Session.SetInt32("id", loginUser.Id);
            return RedirectToAction ("Dashboard", "Home");
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["success"] = "You have successfully logged out.";
            return Redirect("/");
        }
    }
}

