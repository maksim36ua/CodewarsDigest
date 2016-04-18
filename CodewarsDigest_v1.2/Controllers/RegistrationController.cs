using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using cw_itkpi.Models;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace cw_itkpi.Controllers
{
    public class RegistrationController : Controller
    {
        private UserContext _context;

        private string connectionString { get; set; }
        public IConfigurationRoot Configuration { get; set; }

        public RegistrationController(UserContext context)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets();
            Configuration = builder.Build();

            connectionString = Configuration["Data:DefaultConnection"];

            _context = context;
        }

        public IActionResult RegButton()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View("Registration");
        }

        [HttpPost]
        public IActionResult Registration(UserInfo user)
        {
            if (_context.Users.Any(userFromDb => userFromDb.username == user.username)) // Check if user already exists in the database
                return View(user);

            if (ModelState.IsValid && !string.IsNullOrEmpty(user.RetrieveValues()))
            {
                user.ClearVkLink();

                user.thisWeekHonor = user.honor;
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        
    }
}
