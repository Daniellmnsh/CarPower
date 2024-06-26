using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ApiProjectWork.Entities;

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

                    string query = "SELECT [Id], [Address], [Longitude], [Latitude], [IsActive], [HasFastCharge], [KwPrice] FROM [dbo].[Controller]";
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ChargingStation>> GetChargingStationById(int id)
        {
            try
            {
                ChargingStation chargingStation = null;

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    string query = "SELECT [Id], [Address], [Longitude], [Latitude], [IsActive], [HasFastCharge], [KwPrice] FROM [dbo].[Controller] WHERE [Id] = @Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                chargingStation = new ChargingStation
                                {
                                    Id = reader.GetInt32(0),
                                    Address = reader.GetString(1),
                                    Longitude = Convert.ToDouble(reader.GetDecimal(2)),
                                    Latitude = Convert.ToDouble(reader.GetDecimal(3)),
                                    IsActive = reader.GetBoolean(4),
                                    HasFastCharge = reader.GetBoolean(5),
                                    KwPrice = reader.GetDecimal(6)
                                };
                            }
                        }
                    }
                }

                if (chargingStation == null)
                {
                    return NotFound();
                }

                return Ok(chargingStation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
