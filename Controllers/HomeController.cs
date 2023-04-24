using DataCaptureExpertsWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;


namespace DataCaptureExpertsWebAPI.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("DCEDB");
        }

        [HttpGet]
        [Route("api/GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new SqlCommand("SELECT * FROM Customer", connection);
                var reader = await command.ExecuteReaderAsync();

                var customers = new List<Customer>();

                while (await reader.ReadAsync())
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
                if (customers.Count == 0)
                {
                    return NotFound("No customers found.");
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving customers from database: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerViewModel customer)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO Customer (UserId, Username, Email, FirstName, LastName, CreatedOn, IsActive) VALUES " +
            "(@UserId, @Username, @Email, @FirstName, @LastName, @CreatedOn, @IsActive); SELECT SCOPE_IDENTITY();", connection);

                command.Parameters.AddWithValue("@UserId", Guid.NewGuid());
                command.Parameters.AddWithValue("@Username", customer.Username);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                command.Parameters.AddWithValue("@LastName", customer.LastName);
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@IsActive", true);
                
                await command.ExecuteScalarAsync();
                return Ok(customer);
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating customer in database: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("api/UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerViewModel customer)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("UPDATE Customer SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, IsActive = @IsActive WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@Username", customer.Username);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                command.Parameters.AddWithValue("@LastName", customer.LastName);
                command.Parameters.AddWithValue("@IsActive", customer.IsActive);
                command.Parameters.AddWithValue("@UserId", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(customer);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating customer in database: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("api/DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DELETE FROM Customer WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok("Customer Deleted Succesfully");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting customer from database: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/GetActiveOrdersByCustomer")]
        public async Task<IActionResult> GetActiveOrdersByCustomer(Guid id)
        {
            try
            {
                var orders = new List<Orders>();

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("GetActiveOrdersByCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", id);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var order = new Orders
                            {
                                OrderId = reader.GetGuid(reader.GetOrdinal("OrderId")),
                                ProductId = reader.GetGuid(reader.GetOrdinal("ProductId")),
                                OrderStatus = reader.GetInt32(reader.GetOrdinal("OrderStatus")),
                                OrderType = reader.GetInt32(reader.GetOrdinal("OrderType")),
                                OrderBy = reader.GetGuid(reader.GetOrdinal("OrderBy")),
                                OrderedOn = reader.GetDateTime(reader.GetOrdinal("OrderedOn")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                Product = new Product
                                {
                                    ProductId = reader.GetGuid(reader.GetOrdinal("ProductId")),
                                    ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                    SupplierId = reader.GetGuid(reader.GetOrdinal("SupplierId")),
                                    CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    Supplier = new Supplier
                                    {
                                        SupplierId = reader.GetGuid(reader.GetOrdinal("SupplierId")),
                                        SupplierName = reader.GetString(reader.GetOrdinal("SupplierName")),
                                        CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                    },
                                }
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("ShippedOn")))
                            {
                                order.ShippedOn = reader.GetDateTime(reader.GetOrdinal("ShippedOn"));
                            }
                            orders.Add(order);
                        }
                        if (orders == null || orders.Count == 0)
                        {
                            return NotFound();
                        }
                        else
                        {
                            return Ok(orders);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retriving data from database: {ex.Message}");
            }
        }
    }
}