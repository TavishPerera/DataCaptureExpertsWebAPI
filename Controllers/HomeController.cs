using DataCaptureExpertsWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataCaptureExpertsWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {

        private readonly DataCaptureExpertsDBContext _dbContext;
        private readonly string _connectionString = "Server=TAVISH-LAPTOP;Database=DataCaptureExpertsDB;Trusted_Connection=True;";

        public HomeController(DataCaptureExpertsDBContext dbContext)

        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM Customer", connection))
            {
                connection.Open();
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    var customer = new Customer
                    {
                        UserId = (Guid)reader["UserId"],
                        Username = (string)reader["Username"],
                        Email = (string)reader["Email"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        CreatedOn = (DateTime)reader["CreatedOn"],
                        IsActive = (bool)reader["IsActive"]
                    };
                    customers.Add(customer);
                }
            }
            return customers;
        }
    }
}
