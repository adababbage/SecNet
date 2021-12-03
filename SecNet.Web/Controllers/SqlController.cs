﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SecNet.Web.ViewModels;
using System.Collections.Generic;

namespace SecNet.Web.Controllers
{
    public class SqlController : Controller
    {
        public IConfiguration Configuration { get; }

        public SqlController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetUserUsingCommand()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUserUsingCommand(string firstName)
        {
            //This line includes a sql injection point.
            var unsafeQuery = $"Select firstName, LastName from users where firstname = '{firstName}'";

            SqlConnection conn = new SqlConnection(Configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            SqlCommand command = new SqlCommand(unsafeQuery, conn);

            List<UserDetailViewModel> userResults = new List<UserDetailViewModel>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    userResults.Add(new UserDetailViewModel { FirstName = reader["firstName"].ToString(), LastName = reader["LastName"].ToString() });
                }
            }

            return View("GetUserUsingCommandResult", userResults);
        }
    }
}
