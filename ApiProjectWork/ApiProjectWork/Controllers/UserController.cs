using ApiProjectWork.Entities;
using CLOD.ProjectWork.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CLOD.ProjectWork.Controllers
{
    [Route("[controller]")]
    [ApiController]
    // [Au
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetChargingStations()
        {
            try
            {
                var users = new List<User>();

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    string query = "SELECT Id,[Name],[Surname],[Plate],[IsAdmin],[UserName],[Email],[PhoneNumber] FROM [dbo].[AspNetUsers]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var user = new User
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    Surname = reader.GetString(2),
                                    Plate = reader.GetString(3),
                                    IsAdmin = reader.GetBoolean(4),
                                    Username = reader.GetString(5),
                                    Email = reader.GetString(6),
                                    Phone = reader.GetString(7)
                                };

                                users.Add(user);
                            }
                        }
                    }
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}