using SneakerStore.Core.Models.Sneaker;

namespace SneakerStore.Appication.DTOs.Sneaker;

public record CreateSneakerDto(
    string Name,
    decimal Price,
    string Description,
    IReadOnlyCollection<SneakerSizeDto> Sizes,
    string? ImageUrl = null);