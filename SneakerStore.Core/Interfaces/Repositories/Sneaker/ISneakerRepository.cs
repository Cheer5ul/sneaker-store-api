using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results.Errors;

namespace SneakerStore.Core.Interfaces.Repositories.Sneaker;

public interface ISneakerRepository
{
    Task<List<Models.Sneaker.Sneaker>> GetAll(CancellationToken cancellationToken = default);
    
    Task<bool> SneakerExists(Guid id, CancellationToken cancellationToken = default);
    
    Task<Guid> Create(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default);

    Task<(Guid id, List<Error> errors)> UpdateName(Guid id, string newName,
        CancellationToken cancellationToken = default);

    Task<(Guid id, List<Error> errors)> UpdatePrice(Guid id, decimal newPrice,
        CancellationToken cancellationToken = default);

    Task<(Guid id, List<Error> errors)> UpdateDescription(Guid id, string newDescription,
        CancellationToken cancellationToken = default);

    Task<(Guid id, List<Error> errors)> UpdateImageUrl(Guid id, string newImageUrl,
        CancellationToken cancellationToken = default);
    
    Task Delete(Guid sneakerId,
        CancellationToken cancellationToken = default);

    Task<List<SneakerSize>> GetAllSizes(Guid sneakerId,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateSize(Guid sneakerId,
        SneakerSize sneakerSize, CancellationToken cancellationToken = default);

    Task DeleteSize(Guid sneakerId,
        Guid sneakerSizeId, CancellationToken cancellationToken = default);
}