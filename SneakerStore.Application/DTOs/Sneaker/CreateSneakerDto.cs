using System.Collections.Generic;
using SneakerStore.Appication.DTOs.Sneaker;
using SneakerStore.Core.Models.Sneaker;

namespace SneakerStore.Application.DTOs.Sneaker;

public record CreateSneakerDto(
    string Name,
    decimal Price,
    string Description,
    IReadOnlyCollection<SneakerSizeDto> Sizes,
    string? ImageUrl = null);