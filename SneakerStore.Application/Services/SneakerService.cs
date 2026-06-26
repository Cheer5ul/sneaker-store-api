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

    public async Task<Result> UpdateName(Guid id, string newName,
        CancellationToken cancellationToken = default)
    {
        var sneaker = await sneakerRepository.GetById(id, cancellationToken);
        
        if (sneaker == null) return Result.Failure([SneakerErrors.NotFound(id)]);
        
        var updateResult = sneaker.UpdateName(newName);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Errors);
        
        await sneakerRepository.UpdateName(id, newName, cancellationToken);
        
        return Result.Success();
    }
    

    public async Task<Result> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        // Making sure the sneaker exists
        var sneakerExists = await sneakerRepository.SneakerExists(id, cancellationToken);
        if (!sneakerExists) return Result.Failure([SneakerErrors.NotFound(id)]);
        
        await sneakerRepository.Delete(id, cancellationToken);
        return Result.Success();
    }
}