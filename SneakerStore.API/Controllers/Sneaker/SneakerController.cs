using Microsoft.AspNetCore.Mvc;
using SneakerStore.Application.DTOs.Sneaker;
using SneakerStore.Application.Services;
using SneakerStore.Core.Interfaces.Repositories.Sneaker;
using SneakerStore.DTOs.Sneaker;

namespace SneakerStore.Controllers.Sneaker;

[ApiController]
[Route("[controller]")]
public class SneakerController : ControllerBase
{
    // TODO: Add Error Handler
    
    private readonly ISneakerService _sneakerService;

    public SneakerController(ISneakerService sneakerService)
    {
        _sneakerService = sneakerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SneakerResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await  _sneakerService.GetAll(cancellationToken);
        
        // validation

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
    
    // public async Task<ActionResult<Guid>> Create(CreateSneakerDto createSneakerDto,
    //     CancellationToken cancellationToken)
    // {
    //     var result = await _sneakerService.Create(createSneakerDto, cancellationToken);
    //
    //     if (result.IsFailure)
    //     {
    //         // return null;
    //     }
    //
    //     var response = result.Value!;
    //     
    //     return Ok(response);
    // }
}