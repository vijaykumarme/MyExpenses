using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyExpenses.Data;
using MyExpenses.Models;

namespace MyExpenses.Controllers
{
    public class HistoryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public HistoryController(AppDbContext _PrAppDbContext, IConfiguration _PrIconfiguration)
        {
            _db = _PrAppDbContext;
            _configuration = _PrIconfiguration;
        }
        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("connString");
            List<ExpensesHistory> obj = new List<ExpensesHistory>();

            using (SqlConnection  conn = new SqlConnection(connectionString)) 
            {
                try
                {
                    conn.Open();
                    string query = "EXEC usp_get_expense_history";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ExpensesHistory readObj = new ExpensesHistory
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    ExpId = Convert.ToInt32(reader["ExpId"]),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    Category = reader["CategoryName"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Money = Convert.ToInt32(reader["Money"]),
                                    Location = reader["Location"].ToString()
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

        public IActionResult Edit(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }
            string connectionString = _configuration.GetConnectionString("connString");
            Expenses obj = new Expenses();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "EXEC usp_get_expense @ExpId = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Expenses readObj = new Expenses
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Category = Convert.ToInt32(reader["Category"]),
                                    Name = reader["Name"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    Money = Convert.ToInt32(reader["Money"]),
                                    Date = Convert.ToDateTime(reader["Date"])
                                };
                                obj.Id = readObj.Id;
                                obj.Category = readObj.Category;
                                obj.Name = readObj.Name;
                                obj.Location = readObj.Location;
                                obj.Money = readObj.Money;
                                obj.Date = readObj.Date;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return NotFound();
                }          
            }

            _db.SaveChanges();

            return View(obj);
        }

        [HttpPost]
        public IActionResult Edit(Expenses obj)
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
                    string query = "EXEC usp_update_expense @ExpId = @ExpId, @Category = @Category, @Name = @Name, @Location = @Location, @Year = @Year, @Month = @Month, @Date = @Date, @Money = @Money;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExpId", obj.Id);
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
                            TempData["Success"] = "Expense Updated Successfully";
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


            return RedirectToAction("Index", "History");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            string connectionString = _configuration.GetConnectionString("connString");
            Expenses obj = new Expenses();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "EXEC usp_get_expense @ExpId = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Expenses readObj = new Expenses
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Category = Convert.ToInt32(reader["Category"]),
                                    Name = reader["Name"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    Money = Convert.ToInt32(reader["Money"]),
                                    Date = Convert.ToDateTime(reader["Date"])
                                };
                                obj.Id = readObj.Id;
                                obj.Category = readObj.Category;
                                obj.Name = readObj.Name;
                                obj.Location = readObj.Location;
                                obj.Money = readObj.Money;
                                obj.Date = readObj.Date;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }

            _db.SaveChanges();

            return View(obj);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            string connectionString = _configuration.GetConnectionString("connString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = "EXEC usp_delete_expense @ExpId = @ExpId";
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExpId", id);

                        int rowsEffected = cmd.ExecuteNonQuery();

                        if (rowsEffected < 1)
                        {
                            return NotFound();
                        }
                    }
                    conn.Close();
                    TempData["Success"] = "Expense Deleted Successfully";
                }
                catch
                {
                    return NotFound();
                }
            }

            _db.SaveChanges();

            return RedirectToAction("Index", "History");
        }
    }
}
