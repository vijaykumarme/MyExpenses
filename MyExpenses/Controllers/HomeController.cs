using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyExpenses.Data;
using MyExpenses.Models;
using System.Diagnostics;

namespace MyExpenses.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        private readonly IConfiguration _Iconfiguration;

        public HomeController(AppDbContext _PrAppDbContext, IConfiguration _PrIconfiguration)
        {
            _db = _PrAppDbContext;
            _Iconfiguration = _PrIconfiguration;
        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            DateTime dateTime = DateTime.Now;
            int currentMonth = Convert.ToInt32(dateTime.Month);

            string connectionString = _Iconfiguration.GetConnectionString("connString");

            List<ExpensesMonthWise> obj = new List<ExpensesMonthWise>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "EXEC usp_current_month_expenses @CurrentMonth = @CurrentMonth";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ExpensesMonthWise readObj = new ExpensesMonthWise
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Money = Convert.ToInt32(reader["Money"]),
                                    Month = Convert.ToInt32(reader["Month"])
                                };
                                obj.Add(readObj);
                            }
                        }
                    }
                    conn.Close();

                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            _db.SaveChanges();
            return View(obj);
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