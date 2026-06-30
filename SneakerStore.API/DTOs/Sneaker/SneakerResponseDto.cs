namespace SneakerStore.DTOs.Sneaker;

public record SneakerResponseDto(
    Guid Id,
    string Name,
    decimal Price,
    string Description,
    string? ImageUrl,
    List<SneakerSizeResponseDto> Sizes);