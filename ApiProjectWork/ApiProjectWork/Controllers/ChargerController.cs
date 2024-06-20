using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ApiProjectWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChargingStationsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ChargingStationsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChargingStation>>> GetChargingStations()
        {
            try
            {
                var chargingStations = new List<ChargingStation>();

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    string query = "SELECT [Id], [Adress], [Longitude], [Latitude], [IsActive], [HasFastCharge], [KwPrice] FROM [dbo].[Controller]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var station = new ChargingStation
                                {
                                    Id = reader.GetInt32(0),
                                    Address = reader.GetString(1),
                                    Longitude = Convert.ToDouble(reader.GetDecimal(2)),
                                    Latitude = Convert.ToDouble(reader.GetDecimal(3)),
                                    IsActive = reader.GetBoolean(4),
                                    HasFastCharge = reader.GetBoolean(5),
                                    KwPrice = reader.GetDecimal(6)
                                };
                                chargingStations.Add(station);
                            }
                        }
                    }
                }

                return Ok(chargingStations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }

    public class ChargingStation
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool IsActive { get; set; }
        public bool HasFastCharge { get; set; }
        public decimal KwPrice { get; set; }
    }
}
