using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/animals")]
public class AnimalController : ControllerBase
{
    private IConfiguration _configuration;
    

    public AnimalController(IConfiguration configuration)
    {
        _configuration = configuration;
        
    }

    [HttpGet]
    public IActionResult GetAllAnimals([FromQuery] string orderBy = "Name")
    {
        var response = new List<GetAnimalsResponse>();
        var connectionString = _configuration.GetConnectionString("Default");
        try
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"SELECT * FROM Animal ORDER BY {orderBy}", sqlConnection);

                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response.Add(new GetAnimalsResponse(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4))
                        );
                    }
                }
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            // Obsługa błędu
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimalRequest animalRequest)
    {

        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "INSERT INTO ANIMAL(Name, Description,Category,Area) values (@1,@2,@3,@4); SELECT CAST(SCOPE_Identity() as int)",sqlConnection
            );

            sqlCommand.Parameters.AddWithValue("@1", animalRequest.Name);
            sqlCommand.Parameters.AddWithValue("@2", animalRequest.Description);
            sqlCommand.Parameters.AddWithValue("@3", animalRequest.Category);
            sqlCommand.Parameters.AddWithValue("@4", animalRequest.Area);
            sqlConnection.Open();

            var id = sqlCommand.ExecuteScalar();

            return Created($"animals/{id}", new AddAnimalResponse((int)id, animalRequest));
        }
    }

    [HttpPut("{IdAnimal}")]
    public IActionResult UpdateAnimal(int IdAnimal,AddAnimalRequest animalRequest)
    {

        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "UPDATE Animal SET Name = @1, Description = @2, Category = @3, Area = @4 WHERE IdAnimal = @5",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", animalRequest.Name);
            sqlCommand.Parameters.AddWithValue("@2", animalRequest.Description);
            sqlCommand.Parameters.AddWithValue("@3", animalRequest.Category);
            sqlCommand.Parameters.AddWithValue("@4", animalRequest.Area);
            sqlCommand.Parameters.AddWithValue("@5", IdAnimal);
            sqlCommand.Connection.Open();

            var affectedRows = sqlCommand.ExecuteNonQuery();
            return affectedRows == 0 ? NotFound() : NoContent();

        }
    }

    [HttpDelete("{IdAnimal}")]
    public IActionResult DeleteAnimal(int IdAnimal)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "DELETE FROM Animal WHERE IdAnimal = @1",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", IdAnimal);
            sqlCommand.Connection.Open();
            
            var affectedRows = sqlCommand.ExecuteNonQuery();
            return affectedRows == 0 ? NotFound() : NoContent();

        }

    }
}