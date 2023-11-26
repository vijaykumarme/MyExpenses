using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyExpenses.Data;
using MyExpenses.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;

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

            //using (SqlConnection  conn = new SqlConnection(connectionString)) 
            //{
            //    try
            //    {
            //        conn.Open();
            //        string query = "SELECT * FROM fn_get_expenses_history('vijay');";

            //        using (SqlCommand cmd = new SqlCommand(query, conn))
            //        {
            //            using(SqlDataReader reader = cmd.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    ExpensesHistory readObj = new ExpensesHistory
            //                    {
            //                        Id = Convert.ToInt32(reader["Id"]),
            //                        ExpId = Convert.ToInt32(reader["ExpId"]),
            //                        Date = Convert.ToDateTime(reader["Date"]),
            //                        Category = reader["CategoryName"].ToString(),
            //                        Name = reader["Name"].ToString(),
            //                        Money = Convert.ToInt32(reader["Money"]),
            //                        Location = reader["Location"].ToString()
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
                    string schemaName = "vijay";
                    string query = "SELECT * FROM fn_get_expenses_history(@schemaName);";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("schemaName", schemaName);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ExpensesHistory readObj = new ExpensesHistory
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    ExpId = Convert.ToInt32(reader["expid"]),
                                    Date = Convert.ToDateTime(reader["date"]),
                                    Category = reader["categoryname"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Money = Convert.ToInt32(reader["money"]),
                                    Location = reader["location"].ToString()
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

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string currSchema = "vijay";
                    string query = "SELECT * FROM fn_get_expense(@schemaName,@expenseId::Integer);";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("schemaName", currSchema);
                        cmd.Parameters.AddWithValue("expenseId", id);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Expenses readObj = new Expenses
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Category = Convert.ToInt32(reader["Category"]),
                                    Name = reader["Name"].ToString(),
                                    Money = Convert.ToInt32(reader["Money"]),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    Location = reader["Location"].ToString()
                                };
                                obj.Id = readObj.Id;
                                obj.Category = readObj.Category;
                                obj.Name = readObj.Name;
                                obj.Money = readObj.Money;
                                obj.Date = readObj.Date;
                                obj.Location = readObj.Location;

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

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string currSchema = "vijay";
                    string query = "SELECT fn_update_expense(@SchemaName, @ExpenseId, @NewCategory, @NewName, @NewLocation, @NewYear, @NewMonth, @NewDate, @NewMoney)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("SchemaName", currSchema).NpgsqlDbType = NpgsqlDbType.Text;
                        cmd.Parameters.AddWithValue("ExpenseId", obj.Id).NpgsqlDbType = NpgsqlDbType.Integer;
                        cmd.Parameters.AddWithValue("NewCategory", obj.Category).NpgsqlDbType = NpgsqlDbType.Integer;
                        cmd.Parameters.AddWithValue("NewName", obj.Name).NpgsqlDbType = NpgsqlDbType.Text;
                        cmd.Parameters.AddWithValue("NewLocation", obj.Location).NpgsqlDbType = NpgsqlDbType.Text;
                        cmd.Parameters.AddWithValue("NewYear", objYearInt).NpgsqlDbType = NpgsqlDbType.Smallint;
                        cmd.Parameters.AddWithValue("NewMonth", objMonthInt).NpgsqlDbType = NpgsqlDbType.Smallint;
                        cmd.Parameters.AddWithValue("NewDate", obj.Date).NpgsqlDbType = NpgsqlDbType.Timestamp;
                        cmd.Parameters.AddWithValue("NewMoney", obj.Money).NpgsqlDbType = NpgsqlDbType.Integer;

                        cmd.ExecuteNonQuery();

                        conn.Close();
                        TempData["Success"] = "Expenses Updated Successfully";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Success"] = "Expenses Update Failed";
                    return NotFound();
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
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

            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        conn.Open();
            //        string query = "EXEC usp_get_expense @ExpId = @id";

            //        using (SqlCommand cmd = new SqlCommand(query, conn))
            //        {
            //            cmd.Parameters.AddWithValue("@id", id);

            //            using (SqlDataReader reader = cmd.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    Expenses readObj = new Expenses
            //                    {
            //                        Id = Convert.ToInt32(reader["Id"]),
            //                        Category = Convert.ToInt32(reader["Category"]),
            //                        Name = reader["Name"].ToString(),
            //                        Location = reader["Location"].ToString(),
            //                        Money = Convert.ToInt32(reader["Money"]),
            //                        Date = Convert.ToDateTime(reader["Date"])
            //                    };
            //                    obj.Id = readObj.Id;
            //                    obj.Category = readObj.Category;
            //                    obj.Name = readObj.Name;
            //                    obj.Location = readObj.Location;
            //                    obj.Money = readObj.Money;
            //                    obj.Date = readObj.Date;
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        return NotFound();
            //    }
            //}

            string currSchema = "vijay";

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM fn_get_expense(@schemaName,@expenseId::Integer);";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("schemaName", currSchema);
                        cmd.Parameters.AddWithValue("expenseId", id);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Expenses readObj = new Expenses
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Category = Convert.ToInt32(reader["Category"]),
                                    Name = reader["Name"].ToString(),
                                    Money = Convert.ToInt32(reader["Money"]),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    Location = reader["Location"].ToString()
                                };
                                obj.Id = readObj.Id;
                                obj.Category = readObj.Category;
                                obj.Name = readObj.Name;
                                obj.Money = readObj.Money;
                                obj.Date = readObj.Date;
                                obj.Location = readObj.Location;

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

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string currSchema = "vijay";
                    string query = "SELECT fn_delete_expense(@schemaName,@expenseId::Integer);";
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("schemaName", currSchema);
                        cmd.Parameters.AddWithValue("expenseId", id);

                        int rowsEffected = cmd.ExecuteNonQuery();

                    }
                    conn.Close();
                    TempData["Success"] = "Expenses Deleted Successfully";
                }
                catch
                {
                    TempData["Success"] = "Expenses Dalete Failed";
                    return NotFound();
                }
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
