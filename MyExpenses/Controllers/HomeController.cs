using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyExpenses.Data;
using MyExpenses.Models;
using Npgsql;
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

            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        conn.Open();
            //        string query = "EXEC usp_current_month_expenses @CurrentMonth = @CurrentMonth";
            
            //        using (SqlCommand cmd = new SqlCommand(query, conn))
            //        {
            //            cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);

            //            using (SqlDataReader reader = cmd.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    ExpensesMonthWise readObj = new ExpensesMonthWise
            //                    {
            //                        Id = Convert.ToInt32(reader["Id"]),
            //                        CategoryName = reader["CategoryName"].ToString(),
            //                        Money = Convert.ToInt32(reader["Money"]),
            //                        Month = Convert.ToInt32(reader["Month"])
            //                    };
            //                    obj.Add(readObj);
            //                }
            //            }
            //        }
            //        conn.Close();

            //    }
            //    catch (Exception ex)
            //    {
            //        return NotFound();
            //    }
            //}

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {

                    conn.Open();
                    string curSchema = "vijay";
                    //string query1 = "SELECT * FROM vijay.\"Expenses\" WHERE \"Month\" = 11;";
                    string query = "SELECT * FROM fn_current_month_expenses(@SchemaName, @CurrentMonth::SMALLINT)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        //cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);
                        cmd.Parameters.AddWithValue("SchemaName", curSchema);
                        cmd.Parameters.AddWithValue("CurrentMonth", currentMonth);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ExpensesMonthWise readObj = new ExpensesMonthWise()
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    CategoryName = reader["categoryname"].ToString(),
                                    Money = Convert.ToInt32(reader["money"]),
                                    Month = Convert.ToInt32(reader["month"])
                                };
                                obj.Add(readObj);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    return NotFound();
                }
                conn.Close();
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