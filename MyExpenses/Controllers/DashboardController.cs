using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyExpenses.Data;
using MyExpenses.Models;

namespace MyExpenses.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public DashboardController(AppDbContext _PrAppDbContext, IConfiguration _PrIconfiguration)
        {
            _db = _PrAppDbContext;
            _configuration = _PrIconfiguration;
        }
        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("connString");
            List<DashboardExpenses> obj = new List<DashboardExpenses>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "EXEC usp_get_expense_history";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DashboardExpenses readObj = new DashboardExpenses
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    ExpId = Convert.ToInt32(reader["ExpId"]),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Money = Convert.ToInt32(reader["Money"])
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
    }
}
