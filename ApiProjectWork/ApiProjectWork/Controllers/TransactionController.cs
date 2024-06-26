using ApiProjectWork.Entities;
using CLOD.ProjectWork.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CLOD.ProjectWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransactionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Invalid transaction object");
            }

            try
            {
                // Retrieve the KwPrice for the specified ControllerId
                decimal kwPrice = await GetControllerKwPrice(transaction.ControllerId);

                // Calculate total money based on KwUsage and KwPrice
                transaction.TotalMoney = transaction.KwUsage * kwPrice;

                // Deduct total money from the user's wallet
                bool success = await DeductMoneyFromWallet(transaction.UserId, transaction.TotalMoney);

                if (!success)
                {
                    return BadRequest("Insufficient funds in the wallet.");
                }

                // Record the transaction in the database
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    string insertQuery = @"INSERT INTO [dbo].[Transaction] ([ControllerId], [UserId], [KwUsage], [Date], [TotalMoney]) 
                                           VALUES (@ControllerId, @UserId, @KwUsage, @Date, @TotalMoney);";

                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@ControllerId", transaction.ControllerId);
                        insertCommand.Parameters.AddWithValue("@UserId", transaction.UserId);
                        insertCommand.Parameters.AddWithValue("@KwUsage", transaction.KwUsage);
                        insertCommand.Parameters.AddWithValue("@Date", DateTime.UtcNow);
                        insertCommand.Parameters.AddWithValue("@TotalMoney", transaction.TotalMoney);

                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }

                return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private async Task<decimal> GetControllerKwPrice(int controllerId)
        {
            try
            {
                decimal kwPrice = 0;

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    string query = "SELECT [KwPrice] FROM [dbo].[Controller] WHERE [Id] = @ControllerId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ControllerId", controllerId);

                        var result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            kwPrice = Convert.ToDecimal(result);
                        }
                        else
                        {
                            throw new Exception("KwPrice not found for the specified controller ID.");
                        }
                    }
                }

                return kwPrice;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving KwPrice from controller.", ex);
            }
        }

        private async Task<bool> DeductMoneyFromWallet(string userId, decimal amount)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // Lock the wallet row to prevent concurrent updates
                        string lockQuery = "SELECT [Money] FROM [dbo].[Wallet] WITH (UPDLOCK) WHERE [User] = @UserId";
                        using (SqlCommand lockCommand = new SqlCommand(lockQuery, connection, transaction))
                        {
                            lockCommand.Parameters.AddWithValue("@UserId", userId);

                            object currentMoneyObj = await lockCommand.ExecuteScalarAsync();
                            if (currentMoneyObj == null)
                            {
                                throw new Exception($"Wallet not found for user with ID: {userId}");
                            }

                            decimal currentMoney = Convert.ToDecimal(currentMoneyObj);

                            if (currentMoney < amount)
                            {
                                throw new Exception("Insufficient funds in the wallet.");
                            }

                            // Deduct the amount from the wallet
                            string updateQuery = "UPDATE [dbo].[Wallet] SET [Money] = [Money] - @Amount WHERE [User] = @UserId";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@UserId", userId);
                                updateCommand.Parameters.AddWithValue("@Amount", amount);

                                int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                                if (rowsAffected == 0)
                                {
                                    throw new Exception("Failed to update wallet.");
                                }
                            }

                            // Commit transaction if all updates succeed
                            transaction.Commit();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if any exception occurs
                        transaction.Rollback();
                        throw ex; // Re-throw the exception to propagate it up the call stack
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deducting money from wallet.", ex);
            }
        }
    }
}
