namespace SneakerStore.DTOs.Sneaker;

public record SneakerSizeResponseDto(
    Guid Id,
    decimal Size,
    int RemainedInStock,
    Guid SneakerId);