using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyExpenses.Data;
using MyExpenses.Models;
using Npgsql;

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

            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        conn.Open();
            //        string query = "EXEC usp_get_history_dashboard";

            //        using (SqlCommand cmd = new SqlCommand(query, conn))
            //        {
            //            using (SqlDataReader reader = cmd.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    DashboardExpenses readObj = new DashboardExpenses
            //                    {
            //                        Id = Convert.ToInt32(reader["Id"]),
            //                        CategoryName = reader["CategoryName"].ToString(),
            //                        Money = Convert.ToInt32(reader["Money"])
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
                    string currSchema = "vijay";
                    string query = "SELECT * FROM fn_get_history_dashboard(@schemaName);";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("schemaName", currSchema);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DashboardExpenses readObj = new DashboardExpenses
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    CategoryName = reader["categoryName"].ToString(),
                                    Money = Convert.ToInt32(reader["money"])
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
