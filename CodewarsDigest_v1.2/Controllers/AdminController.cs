using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw_itkpi.Models;
using Microsoft.AspNet.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.OptionsModel;

namespace cw_itkpi.Controllers
{
    public class AdminController : Controller
    {
        private UserContext _context;

        private string password { get; set; }
        private string connectionString { get; set; }
        public IConfigurationRoot Configuration { get; set; }

        public AdminController(UserContext context)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets();
            Configuration = builder.Build();

            connectionString = Configuration["Data:DefaultConnection"];
            password = Configuration["Data:AdminPassword"];

            _context = context;
        }

        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserInfo userObject)
        {
            if (ModelState.IsValid)
            {
                userObject.RetrieveValues();
                userObject.ClearVkLink();
                _context.Users.Update(userObject);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(userObject);
        }

        [ActionName("Delete")]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(UserInfo userInfoToDelete)
        {
            _context.Users.Remove(userInfoToDelete);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Rating()
        {
            return View();
        }

        public IActionResult RatingWithVKpage()
        {
            return View(_context.Users
                .Where(user => user.lastWeekHonor != 0)
                .OrderByDescending(user => user.lastWeekHonor).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rating(AdminClass admin)
        {
            if (admin.Password == password)
            {
                switch (admin.actionToPerform)
                {
                    case "Generate":
                        {
                            GenerateWeeklyRating();
                            return RedirectToAction("RatingWithVKpage");
                        }
                    case "Update":
                        {
                            UpdateWeeklyRating();
                            break;
                        }
                    case "Delete":
                        {
                            DeleteWeeklyRating();
                            break;
                        }
                    default:
                        return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index", "Home");
            }
            else
                return View();
        }

        public void UpdateWeeklyRating()
        {
            _context.Users.ToList()
                .ForEach(user =>
                {
                    user.RetrieveValues();
                    _context.Entry(user).State = Microsoft.Data.Entity.EntityState.Modified;
                });

            _context.SaveChanges();
            
        }

        public void GenerateWeeklyRating()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    int weekNumber = -1; // Create table for new week rating
                    bool exists = false;
                    List<string[]> usersAndPoints = new List<string[]>();

                    do
                    {
                        weekNumber++;
                        command.CommandText = $"select case when exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Week{weekNumber}') then 1 else 0 end";
                        exists = (int)command.ExecuteScalar() == 1;
                        if (!exists)
                        {
                            command.CommandText = $"create table Week{weekNumber}( username nvarchar(450) primary key not null references UserInfo(username) on delete cascade, honor integer, thisWeekHonor integer);";
                            command.ExecuteNonQuery();
                        }
                        
                    } while (exists);

                    command.CommandText = "select * from UserInfo;"; // Extract users from UserInfo table
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                usersAndPoints.Add(new string[] { reader.GetString(0), reader.GetInt32(2).ToString(), reader.GetInt32(4).ToString() });
                            }
                        }
                    }

                    foreach (var user in usersAndPoints) // Insert user points into new rating table
                    {
                        command.CommandText = $"insert into Week{weekNumber}(username, honor, thisWeekHonor) values('{user[0]}', '{int.Parse(user[1])}', '{int.Parse(user[2])}');";
                        command.ExecuteNonQuery();
                    }

                    _context.Users.ToList() // Change this week`s points for main rating
                                    .ForEach(user =>
                                    {
                                        user.lastWeekHonor = user.thisWeekHonor;
                                        user.thisWeekHonor = 0;

                                        _context.Entry(user).State = Microsoft.Data.Entity.EntityState.Modified;
                                    });

                    _context.SaveChanges();
                }
            }

        }        

        public void DeleteWeeklyRating()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    int weekNumber = -1; // Check week number
                    bool exists = false;

                    do
                    {
                        weekNumber++;
                        command.CommandText = $"select case when exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Week{weekNumber}') then 1 else 0 end";
                        exists = (int)command.ExecuteScalar() == 1;
                        if (!exists)
                        {
                            command.CommandText = $"drop table Week{weekNumber - 1};";
                            command.ExecuteNonQuery();
                        }

                    } while (exists);
                }
            }

            UpdateWeeklyRating();

        }
    }
}
