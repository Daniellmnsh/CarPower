﻿using CLOD.ProjectWork.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CLOD.ProjectWork.Controllers
{
    [Route("[controller]")]
    [ApiController]
    // [Authorize] // Uncomment if you want to enforce authentication
    public class WalletController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WalletController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<Wallet>> PostWallet([FromBody] Wallet wallet)
        {
            if (wallet == null)
            {
                return BadRequest("Invalid wallet object");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    // Check if a record already exists for the user
                    string selectQuery = "SELECT [Money] FROM [dbo].[Wallet] WHERE [User] = @UserId";
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@UserId", wallet.User);
                        object existingMoneyObj = await selectCommand.ExecuteScalarAsync();

                        if (existingMoneyObj != null)
                        {
                            // Add the new amount to the existing amount
                            decimal existingMoney = Convert.ToDecimal(existingMoneyObj);
                            decimal newTotalMoney = existingMoney + wallet.Money;

                            // Update the existing record
                            string updateQuery = "UPDATE [dbo].[Wallet] SET [Money] = @Money WHERE [User] = @UserId";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@UserId", wallet.User);
                                updateCommand.Parameters.AddWithValue("@Money", newTotalMoney);

                                updateCommand.ExecuteNonQuery();
                                wallet.Money = newTotalMoney; // Update wallet object with the new total
                            }
                        }
                        else
                        {
                            // Insert a new record if no existing record found
                            string insertQuery = "INSERT INTO [dbo].[Wallet] ([User], [Money]) VALUES (@UserId, @Money); SELECT SCOPE_IDENTITY();";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@UserId", wallet.User);
                                insertCommand.Parameters.AddWithValue("@Money", wallet.Money);

                                // ExecuteScalarAsync returns an object, which we convert to int
                                var result = await insertCommand.ExecuteScalarAsync();
                                int newWalletId = Convert.ToInt32(result);

                                wallet.Id = newWalletId;
                            }
                        }
                    }
                }

                return CreatedAtAction("GetWallet", new { id = wallet.Id }, wallet);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Wallet>> GetWalletByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                string query = "SELECT [Id], [User], [Money] FROM [dbo].[Wallet] WHERE [User] = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var wallet = new Wallet
                            {
                                Id = reader.GetInt32(0),
                                User = reader.GetString(1),
                                Money = reader.GetDecimal(2)
                            };
                            return Ok(wallet);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
        }
    }
}