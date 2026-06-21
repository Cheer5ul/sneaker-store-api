using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results.Errors;

namespace SneakerStore.Core.Interfaces.Repositories.Sneaker;

public interface ISneakerRepository
{
    /// <summary>
    /// Retrieves all the existing Sneakers.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken to cancel an asynchronous operation (optional).</param>
    /// <returns>A list of all the existing Sneakers.</returns>
    Task<List<Core.Models.Sneaker.Sneaker>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sneaker"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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