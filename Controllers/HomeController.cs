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
                    if (customers == null)
                    {
                        return (IEnumerable<Customer>)NotFound();
                    }
                    else
                    {
                        return customers;
                    }
                }
            }
            catch (Exception ex)
            {
                return (IEnumerable<Customer>)BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateCustomer(CreateCustomerViewModel customer)
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
                    command.Parameters.AddWithValue("@IsActive", true);

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
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        //[Route("{id:guid}")]
        public IActionResult UpdateCustomer(Guid id, UpdateCustomerViewModel customer)
        {
            try
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
                    else
                    {
                        return Ok(customer);
                    }
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        //[Route("{id:guid}")]
        public IActionResult DeleteCustomer(Guid id)
        {
            try
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
                    else
                    {
                        return Ok("Customer Deleted Succesfully");
                    }
                } 
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetActiveOrdersByCustomer([FromRoute] Guid id)
        {
            try
            {
                var orders = new List<Orders>();

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("GetActiveOrdersByCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
                        if (orders == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            return Ok(orders);
                        }
                    }
                }
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}