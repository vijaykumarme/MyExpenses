using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.JSInterop.Implementation;
using MyExpenses.Data;
using MyExpenses.Models;

namespace MyExpenses.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public ExpensesController(AppDbContext _PrAppDbContext, IConfiguration _prIconfiguration)
        {
            _db = _PrAppDbContext;
            _configuration = _prIconfiguration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Expenses obj)
        {
            string objDateStr = obj.Date.ToShortDateString();
            int objDateInt = Convert.ToInt32(objDateStr.Substring(0, 2));
            string objMonthStr = obj.Date.ToShortDateString();
            int objMonthInt = Convert.ToInt32(objMonthStr.Substring(3, 2));
            string objYearStr = obj.Date.ToShortDateString();
            int objYearInt = Convert.ToInt32(objYearStr.Substring(6, 4));

            string connectionString = _configuration.GetConnectionString("connstring");
            
            List<Expenses> list = new List<Expenses>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "EXEC usp_add_expenses @Category = @Category, @Name = @Name, @Location = @Location, @Year = @Year, @Month = @Month, @Date = @Date, @Money = @Money";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Category", obj.Category);
                        cmd.Parameters.AddWithValue("@Name", obj.Name);
                        cmd.Parameters.AddWithValue("@Location", obj.Location);
                        cmd.Parameters.AddWithValue("@Year", objYearInt);
                        cmd.Parameters.AddWithValue("@Month", objMonthInt);
                        cmd.Parameters.AddWithValue("@Date", obj.Date);
                        cmd.Parameters.AddWithValue("@Money", obj.Money);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            conn.Close();
                            TempData["Success"] = "Expenses Created Successfully";
                        }
                        else
                        {
                            conn.Close();
                            return NotFound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }


            return RedirectToAction("Index","Home");
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
