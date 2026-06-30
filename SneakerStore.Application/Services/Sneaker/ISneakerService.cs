using SneakerStore.Appication.DTOs.Sneaker;
using SneakerStore.Application.DTOs.Sneaker;
using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results;

namespace SneakerStore.Application.Services;

public interface ISneakerService
{
    Task<Result<List<Sneaker>>> GetAll(CancellationToken cancellationToken = default);

    Task<Result<Guid>> Create(CreateSneakerDto sneakerDto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateName(Guid id, string newName,
        CancellationToken cancellationToken = default);

    Task<Result> UpdatePrice(Guid id, decimal newPrice,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateDescription(Guid id, string newDescription,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateImageUrl(Guid id, string newImageUrl,
        CancellationToken cancellationToken = default);

    Task<Result> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<Result<List<SneakerSize>>> GetAllSizes(Guid sneakerId,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateSize(Guid sneakerId,
        SneakerSizeDto sneakerSizeDto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateSneakerSizeSize(Guid sneakerId, Guid sneakerSizeId,
        decimal newSize, CancellationToken cancellationToken = default);

    Task<Result> UpdateSneakerSizeRemainedInStock(Guid sneakerId, Guid sneakerSizeId,
        int newRemainedInStock, CancellationToken cancellationToken = default);

    Task<Result> DeleteSize(Guid sneakerId,
        Guid sneakerSizeId, CancellationToken cancellationToken = default);
}