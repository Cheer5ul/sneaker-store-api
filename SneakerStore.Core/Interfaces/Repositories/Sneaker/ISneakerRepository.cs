using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results.Errors;

namespace SneakerStore.Core.Interfaces.Repositories.Sneaker;

public interface ISneakerRepository
{
    Task<List<Models.Sneaker.Sneaker>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a Sneaker by an id.
    /// </summary>
    /// <remarks>
    /// The method does not check the validity and existence of the Sneaker with given id.
    /// </remarks>
    /// <param name="id">The id of the Sneaker, Guid.</param>
    /// <param name="includeSizes">Whether you need to include sizes or not, optional parameter, default - true.</param>
    /// <param name="cancellationToken">Cancellation token to cancel an asynchronous operation.</param>
    /// <returns>Sneaker with the given id or null if not found.</returns>
    Task<Core.Models.Sneaker.Sneaker?> GetById(Guid id, bool includeSizes = true,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks whether a Sneaker with the provided id exists.
    /// </summary>
    /// <param name="id">The id of the Sneaker to be checked, Guid.</param>
    /// <param name="cancellationToken">Cancellation token to cancel an asynchronous operation.</param>
    /// <returns>True if Sneaker with given id exists, otherwise false.</returns>
    Task<bool> SneakerExists(Guid id, CancellationToken cancellationToken = default);
    
    Task<Guid> Create(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of a <see cref="Sneaker"/> entry.
    /// </summary>
    /// <remarks>
    /// Use after getting the Sneaker and updating its property with a domain method. 
    /// This method does not validate whether the Sneaker with the provided ID exists in the database.
    /// Ensure that <paramref name="sneaker"/> refers to existing entity before calling this method.  
    /// </remarks>
    /// <param name="sneaker">The Sneaker which name is to be updated.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task representing an asynchronous operation.</returns>
    Task UpdateName(Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default);

    Task UpdatePrice(Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default);

    Task UpdateDescription(Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default);

    Task UpdateImageUrl(Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default);
    
    Task Delete(Guid sneakerId,
        CancellationToken cancellationToken = default);

    Task<List<SneakerSize>> GetAllSizes(Guid sneakerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a SneakerSize with the provided id exists.
    /// </summary>
    /// <param name="sneakerSizeId">The id of the SneakerSize to be checked, Guid.</param>
    /// <param name="cancellationToken">Cancellation token to cancel an asynchronous operation.</param>
    /// <returns>True if Sneaker with given id exists, otherwise false.</returns>
    Task<bool> SneakerSizeExists(Guid sneakerSizeId,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateSize(Guid sneakerId,
        SneakerSize sneakerSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the size of a <see cref="SneakerSize"/> entry./>
    /// </summary>
    /// <remarks>
    /// Use after getting the Sneaker and updating its property with a domain method. 
    /// This method does not validate whether the provided IDs exist in the database.
    /// Ensure that <param name="sneakerId"/> and <param name="sneakerSizeId"/>
    /// refer to existing entities before calling this method.
    /// </remarks>
    /// <param name="sneakerId">The identifier of the Sneaker that owns the size.</param>
    /// <param name="sneakerSizeId">The identifier of the size entry to update.</param>
    /// <param name="newSize">The new size value to apply.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task representing an asynchronous operation.</returns>
    Task UpdateSneakerSizeSize(Guid sneakerId, Guid sneakerSizeId,
        decimal newSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the remained in stock amount of a <see cref="SneakerSize"/> entry./>
    /// </summary>
    /// <remarks>
    /// Use after getting the Sneaker and updating its property with a domain method. 
    /// This method does not validate whether the provided IDs exist in the database.
    /// Ensure that <param name="sneakerId"/> and <param name="sneakerSizeId"/>
    /// refer to existing entities before calling this method.
    /// </remarks>
    /// <param name="sneakerId">The identifier of the Sneaker that owns the size.</param>
    /// <param name="sneakerSizeId">The identifier of the size entry to update.</param>
    /// <param name="newRemainedInStock">The new remained in stock value to apply.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task representing an asynchronous operation.</returns>
    Task UpdateSneakerSizeRemainedInStock(Guid sneakerId, Guid sneakerSizeId,
        int newRemainedInStock, CancellationToken cancellationToken = default);
    
    Task DeleteSize(Guid sneakerId,
        Guid sneakerSizeId, CancellationToken cancellationToken = default);
}