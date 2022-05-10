using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SecNet.Web.ViewModels;
using System.Collections.Generic;
using System.Data;

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
            //This line includes a sql injection point
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

        [HttpGet]
        public IActionResult SqlWithDataAdapter()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SqlWithDataAdapter(string firstName, string lastName)
        {
            SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection"));

            string query = "SELECT * FROM Users where FirstName='" + firstName + "'and LastName='" + lastName + "' ";

            var adapter = new SqlDataAdapter(query, connection);

            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count == 0) return View();

            List<UserDetailViewModel> userResults = new List<UserDetailViewModel>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                userResults.Add(new UserDetailViewModel { FirstName = dataTable.Rows[i]["FirstName"].ToString(), LastName = dataTable.Rows[i]["LastName"].ToString() });
            }
            
            return View("SqlWithDataAdapterResult", userResults);
        }
    }
}
