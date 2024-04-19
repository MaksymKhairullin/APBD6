using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public record AddAnimalRequest(
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Description,
    [Required] [MaxLength(200)] string Category,
    [Required] [MaxLength(200)] string Area
    );

public record AddAnimalResponse(int IdAnimal, string Name, string Description, string Category, String Area)
{
    public AddAnimalResponse(int IdAnimal, AddAnimalRequest animalRequest) :
        this(IdAnimal, animalRequest.Name, animalRequest.Description, animalRequest.Category, animalRequest.Area)
    { }
}