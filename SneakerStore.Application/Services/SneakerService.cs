using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using SneakerStore.Application.DTOs.Sneaker;
using SneakerStore.Core.Interfaces.Repositories.Sneaker;
using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors.Sneaker;

namespace SneakerStore.Application.Services;

public class SneakerService(ISneakerRepository sneakerRepository)
{
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

    
    // TODO: Try to refactor methods to avoid boilerplate code
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
}