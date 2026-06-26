using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results.Errors;

namespace SneakerStore.Core.Interfaces.Repositories.Sneaker;

public interface ISneakerRepository
{
    Task<List<Models.Sneaker.Sneaker>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a Sneaker by an id without check of the existence.
    /// </summary>
    /// <param name="id">The id of the Sneaker, Guid.</param>
    /// <param name="cancellationToken">Cancellation token to cancel an asynchronous operation.</param>
    /// <returns>Sneaker with the given id or null if not found.</returns>
    Task<Core.Models.Sneaker.Sneaker?> GetById(Guid id, CancellationToken cancellationToken = default);

    
    /// <summary>
    /// Checks whether a Sneaker with the id exists.
    /// </summary>
    /// <param name="id">The id of the Sneaker to be checked, Guid.</param>
    /// <param name="cancellationToken">Cancellation token to cancel an asynchronous operation.</param>
    /// <returns>True if Sneaker with given id exists, otherwise false. </returns>
    Task<bool> SneakerExists(Guid id, CancellationToken cancellationToken = default);
    
    Task<Guid> Create(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default);

    Task UpdateName(Guid id, string newName,
        CancellationToken cancellationToken = default);

    Task UpdatePrice(Guid id, decimal newPrice,
        CancellationToken cancellationToken = default);

    Task UpdateDescription(Guid id, string newDescription,
        CancellationToken cancellationToken = default);

    Task UpdateImageUrl(Guid id, string newImageUrl,
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