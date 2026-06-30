using SneakerStore.Appication.DTOs.Sneaker;
using SneakerStore.Application.DTOs.Sneaker;
using SneakerStore.Core.Interfaces.Repositories.Sneaker;
using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors.Sneaker;

namespace SneakerStore.Application.Services;

public class SneakerService(ISneakerRepository sneakerRepository) : ISneakerService
{
    // TODO: Handle race condition between Application existence check and Repository operations
    public async Task<Result<List<Sneaker>>> GetAll(CancellationToken cancellationToken = default)
    {
        var sneakersListResult = await sneakerRepository.GetAll(cancellationToken);
        return Result<List<Sneaker>>.Success(sneakersListResult);
    }

    public async Task<Result<Guid>> Create(CreateSneakerDto sneakerDto,
        CancellationToken cancellationToken = default)
    {
        // Passing Core validation
        var sneakerResult = Sneaker.Create(
            sneakerDto.Name,
            sneakerDto.Price,
            sneakerDto.Description,
            sneakerDto.Sizes
                .Select(ssDto => (ssDto.Size, ssDto.RemainedInStock)),
            sneakerDto.ImageUrl
        );

        if (sneakerResult.IsFailure)
        {
            return Result<Guid>.Failure(sneakerResult.Errors);
        }

        var sneakerGuid = await sneakerRepository.Create(
            sneakerResult.Value!, cancellationToken);
        
        return Result<Guid>.Success(sneakerGuid);
    }

    
    // TODO: Try to refactor methods to avoid boilerplate code (especially making sure the sneaker exists)
    public async Task<Result> UpdateName(Guid id, string newName,
        CancellationToken cancellationToken = default)
    {
        // Making sure the sneaker exists
        var sneaker = await sneakerRepository.GetById(id, false, cancellationToken);
        if (sneaker == null) return Result.Failure([SneakerErrors.NotFound(id)]);
        
        var updateResult = sneaker.UpdateName(newName); // domain validation 
        if (updateResult.IsFailure) return Result.Failure(updateResult.Errors);
        
        await sneakerRepository.UpdateName(sneaker, cancellationToken); 
        
        return Result.Success();
    }

    public async Task<Result> UpdatePrice(Guid id, decimal newPrice,
        CancellationToken cancellationToken = default)
    {
        var sneaker = await sneakerRepository.GetById(id, false, cancellationToken);
        if (sneaker == null) return Result.Failure([SneakerErrors.NotFound(id)]);
        
        var updateResult = sneaker.UpdatePrice(newPrice);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Errors);
        
        await sneakerRepository.UpdatePrice(sneaker, cancellationToken);
        
        return Result.Success();
    }

    public async Task<Result> UpdateDescription(Guid id, string newDescription,
        CancellationToken cancellationToken = default)
    {
        var sneaker = await sneakerRepository.GetById(id, false, cancellationToken);
        if (sneaker == null) return Result.Failure([SneakerErrors.NotFound(id)]);
        
        var updateResult = sneaker.UpdateDescription(newDescription);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Errors);
        
        await sneakerRepository.UpdateDescription(sneaker, cancellationToken);
        
        return Result.Success();
    }

    public async Task<Result> UpdateImageUrl(Guid id, string newImageUrl,
        CancellationToken cancellationToken = default)
    {
        var sneaker = await sneakerRepository.GetById(id, false, cancellationToken);
        if (sneaker == null) return Result.Failure([SneakerErrors.NotFound(id)]);
        
        var updateResult = sneaker.UpdateImageUrl(newImageUrl);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Errors);
        
        await sneakerRepository.UpdateImageUrl(sneaker, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var sneakerExists = await sneakerRepository.SneakerExists(id, cancellationToken);
        if (!sneakerExists) return Result.Failure([SneakerErrors.NotFound(id)]);
        
        await sneakerRepository.Delete(id, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<List<SneakerSize>>> GetAllSizes(Guid sneakerId,
        CancellationToken cancellationToken = default)
    {
        var sneaker = await sneakerRepository.GetById(sneakerId, false, cancellationToken);
        if (sneaker == null) return Result<List<SneakerSize>>.Failure([SneakerErrors.NotFound(sneakerId)]);
        
        var sizes = await sneakerRepository.GetAllSizes(sneakerId, cancellationToken);

        return Result<List<SneakerSize>>.Success(sizes);
    }

    public async Task<Result<Guid>> CreateSize(Guid sneakerId,
        SneakerSizeDto sneakerSizeDto,
        CancellationToken cancellationToken = default)
    {
        var sneakerExists = await sneakerRepository.SneakerExists(sneakerId, cancellationToken);
        if(!sneakerExists) return Result<Guid>.Failure([SneakerErrors.NotFound(sneakerId)]);
        
        // Passing Core validation
        var sneakerSize = SneakerSize.Create(
            sneakerSizeDto.Size,
            sneakerSizeDto.RemainedInStock,
            sneakerId);

        if (sneakerSize.IsFailure)
        {
            return Result<Guid>.Failure(sneakerSize.Errors);
        }
            
        var sneakerGuid = await sneakerRepository.CreateSize(sneakerId,
            sneakerSize.Value!, cancellationToken);
        
        return Result<Guid>.Success(sneakerGuid);
    }

    public async Task<Result> UpdateSneakerSizeSize(Guid sneakerId, Guid sneakerSizeId,
        decimal newSize, CancellationToken cancellationToken = default)
    {
        var sneakerSize = await sneakerRepository.FindSize(sneakerId, sneakerSizeId, cancellationToken);
        if (sneakerSize == null) return Result.Failure([SneakerSizeErrors.NotFound(sneakerSizeId)]);
        
        var updateResult = sneakerSize.UpdateSize(newSize);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Errors);
        
        await sneakerRepository.UpdateSneakerSizeSize(
            sneakerSize,
            cancellationToken);
        
        return Result.Success();
    }

    public async Task<Result> UpdateSneakerSizeRemainedInStock(Guid sneakerId, Guid sneakerSizeId,
        int newRemainedInStock, CancellationToken cancellationToken = default)
    {
        var sneakerSize = await sneakerRepository.FindSize(sneakerId, sneakerSizeId, cancellationToken);
        if (sneakerSize == null) return Result.Failure([SneakerSizeErrors.NotFound(sneakerSizeId)]);
        
        var updateResult = sneakerSize.UpdateRemainedInStock(newRemainedInStock);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Errors);

        await sneakerRepository.UpdateSneakerSizeRemainedInStock(
            sneakerSize, cancellationToken);
        
        return Result.Success();
    }

    public async Task<Result> DeleteSize(Guid sneakerId,
        Guid sneakerSizeId, CancellationToken cancellationToken = default)
    {
        var sneakerExists = await sneakerRepository.SneakerExists(sneakerSizeId, cancellationToken);
        if (!sneakerExists) return Result.Failure([SneakerErrors.NotFound(sneakerSizeId)]);
        
        var sneakerSizeExists = await sneakerRepository.SneakerSizeExists(sneakerSizeId, cancellationToken);
        if(!sneakerSizeExists) return Result.Failure([SneakerSizeErrors.NotFound(sneakerSizeId)]);
        
        await sneakerRepository.DeleteSize(sneakerId, sneakerSizeId, cancellationToken);
        return Result.Success();
    }
}