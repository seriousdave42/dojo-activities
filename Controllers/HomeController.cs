using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeltExam.Models;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    newUser.Password = hasher.HashPassword(newUser, newUser.Password);
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    User justMade = _context.Users.FirstOrDefault(u => u.Email == newUser.Email);
                    HttpContext.Session.SetInt32("LoggedId", justMade.UserId);
                    HttpContext.Session.SetString("LoggedName", justMade.FirstName);
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return View ("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == userLogin.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<UserLogin> hasher = new PasswordHasher<UserLogin>();
                    PasswordVerificationResult check = hasher.VerifyHashedPassword(userLogin, userInDb.Password, userLogin.Password);
                    if (check == 0)
                    {
                        ModelState.AddModelError("Email", "Invalid Email/Password");
                        return View("Index");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("LoggedId", userInDb.UserId);
                        HttpContext.Session.SetString("LoggedName", userInDb.FirstName);
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("LoggedId");
            string userName = HttpContext.Session.GetString("LoggedName");
            if (userId == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AllCampaigns = _context.Campaigns
                                              .Include(c => c.Attendees)
                                              .Include(c => c.Planner)
                                              .Where(c => c.Date > DateTime.Now)
                                              .OrderBy(c => c.Date)
                                              .ToList();
                User user = _context.Users
                                    .Include(u => u.RSVPs)
                                    .ThenInclude(p => p.RSVP)
                                    .Include(u => u.CampaignsPlanned)
                                    .FirstOrDefault(u => u.UserId == (int)userId);
                ViewBag.UserName = userName;
                return View(user);
            }
        }

        [HttpGet("new")]
        public IActionResult Campaign()
        {
            return View();
        }

        [HttpPost("new")]
        public IActionResult NewCampaign(Campaign newCampaign)
        {
            if (ModelState.IsValid)
            {
                int? userId = HttpContext.Session.GetInt32("LoggedId");
                newCampaign.UserId = (int)userId;
                _context.Campaigns.Add(newCampaign);
                _context.SaveChanges();
                Campaign createdCampaign = _context.Campaigns.FirstOrDefault(c => 
                                                        c.UserId == (int)userId &&
                                                        c.Title == newCampaign.Title &&
                                                        c.Date == newCampaign.Date &&
                                                        c.Description == newCampaign.Description);
                return RedirectToAction("SingleCampaign", new {campaignId = createdCampaign.CampaignId});
            }
            else
            {
                return View("Campaign");
            }
        }

        [HttpGet("activities/{campaignId}")]
        public IActionResult SingleCampaign(int campaignId)
        {
            ViewBag.UserId = (int)HttpContext.Session.GetInt32("LoggedId");
            Campaign campaign = _context.Campaigns
                                      .Include(c => c.Attendees)
                                      .ThenInclude(p => p.Attendee)
                                      .Include(c => c.Planner)
                                      .SingleOrDefault(c => c.CampaignId == campaignId);
            return View(campaign);
        }

        [HttpGet("rsvp/{userId}/{campaignId}/{source}")]
        public IActionResult RSVP(int userId, int campaignId, string source)
        {
            Particpant newParticpant = new Particpant()
            {
                UserId = userId,
                CampaignId = campaignId
            };
            _context.Particpants.Add(newParticpant);
            _context.SaveChanges();
            if (source == "camp")
            {
                return RedirectToAction("SingleCampaign", new {campaignId = campaignId});
            }
            else{
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("unrsvp/{userId}/{campaignId}/{source}")]
        public IActionResult UnRSVP(int userId, int campaignId, string source)
        {
            Particpant cancelParticipant = _context.Particpants.FirstOrDefault(p => p.UserId == userId && p.CampaignId == campaignId);
            _context.Particpants.Remove(cancelParticipant);
            _context.SaveChanges();
            if (source == "camp")
            {
                return RedirectToAction("SingleCampaign", new {campaignId = campaignId});
            }
            else{
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("cancel/{campaignId}")]
        public IActionResult Cancel(int campaignId)
        {
            Campaign cancelled = _context.Campaigns.FirstOrDefault(c => c.CampaignId == campaignId);
            _context.Campaigns.Remove(cancelled);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
