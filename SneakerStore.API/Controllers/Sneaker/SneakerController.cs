using Microsoft.AspNetCore.Mvc;
using SneakerStore.Application.DTOs.Sneaker;
using SneakerStore.Application.Services;
using SneakerStore.DTOs.Sneaker;
using SneakerStore.FailureHandler;

namespace SneakerStore.Controllers.Sneaker;

[ApiController]
[Route("[controller]")]
public class SneakerController : ControllerBase
{
    // TODO: Add Error Handler
    
    private readonly ISneakerService _sneakerService;
    private readonly IFailureHandler _failureHandler;
    public SneakerController(ISneakerService sneakerService,
        IFailureHandler failureHandler)
    {
        _sneakerService = sneakerService;
        _failureHandler = failureHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<SneakerResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await  _sneakerService.GetAll(cancellationToken);

        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);

        var sneakerList = result.Value!;

        var sneakerResponses = sneakerList.Select(sneaker => new SneakerResponseDto(
                sneaker.Id,
                sneaker.Name,
                sneaker.Price,
                sneaker.Description,
                sneaker.ImageUrl,
                sneaker.Sizes.Select(size => new SneakerSizeResponseDto(
                    size.Id,
                    size.Size,
                    size.RemainedInStock,
                    size.SneakerId)).ToList()
                )
        ).ToList();
        
        return Ok(sneakerResponses);
    }
    
    [HttpPost("create-sneaker")]
    public async Task<ActionResult<Guid>> Create(CreateSneakerDto createSneakerDto,
        CancellationToken cancellationToken)
    {
        var result = await _sneakerService.Create(createSneakerDto, cancellationToken);
    
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
    
        var response = result.Value!;
        
        return Ok(response);
    }

    [HttpPatch("{id:guid}/name")]
    public async Task<ActionResult> UpdateName(Guid id, [FromBody] UpdateNameDto updateNameDto,
        CancellationToken cancellationToken)
    {
        var result = await _sneakerService.UpdateName(id, updateNameDto.Name, cancellationToken);

        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        
        return Ok();
    }
    
    [HttpPatch("{id:guid}/price")]
    public async Task<ActionResult> UpdatePrice(Guid id, [FromBody] UpdatePriceDto updatePriceDto,
        CancellationToken cancellationToken)
    {
        var result = await _sneakerService.UpdatePrice(id, updatePriceDto.Price, cancellationToken);
        
        if(result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }

    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sneakerService.Delete(id, cancellationToken);
        
        if (result.IsFailure) return _failureHandler.HandleFailure(result, HttpContext);
        
        return Ok();
    }
    
    
}