using DataCaptureExpertsWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;


namespace DataCaptureExpertsWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly string _connectionString = "Server=TAVISH-LAPTOP;Database=DataCaptureExpertsDB;Trusted_Connection=True;";

        public HomeController() { }

        [HttpGet]
        public IEnumerable<Customer> GetAllCustomers()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (IEnumerable<Customer>)StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("INSERT INTO Customer (UserId, Username, Email, FirstName, LastName, CreatedOn, IsActive) VALUES " +
                    "(@UserId, @Username, @Email, @FirstName, @LastName, @CreatedOn, @IsActive); SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@UserId", Guid.NewGuid());
                    command.Parameters.AddWithValue("@Username", customer.Username);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    command.Parameters.AddWithValue("@LastName", customer.LastName);
                    command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    command.Parameters.AddWithValue("@IsActive", customer.IsActive);

                    connection.Open();

                    if (customer != null)
                    {
                        command.ExecuteScalar();
                        return Ok(customer);
                    }
                    else
                    {
                        return StatusCode(204, "No Content");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public IActionResult UpdateCustomer(Guid id, Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("UPDATE Customer SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, IsActive = @IsActive WHERE UserId = @UserId", connection))
            {
                command.Parameters.AddWithValue("@Username", customer.Username);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                command.Parameters.AddWithValue("@LastName", customer.LastName);
                command.Parameters.AddWithValue("@IsActive", customer.IsActive);
                command.Parameters.AddWithValue("@UserId", id);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }

            return Ok(customer);
        }

        [HttpDelete]
        public IActionResult DeleteCustomer(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("DELETE FROM Customer WHERE UserId = @UserId", connection))
            {
                command.Parameters.AddWithValue("@UserId", id);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }

            return Ok();
        }

    }
}
