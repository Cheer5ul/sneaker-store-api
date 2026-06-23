using SneakerStore.Core.Interfaces.Repositories.Sneaker;
using SneakerStore.Core.Models.Sneaker;

namespace SneakerStore.Application.Services;

public class SneakerService(ISneakerRepository sneakerRepository)
{
    public async Task<List<Sneaker>> GetAll(CancellationToken cancellationToken = default)
    {
        return await sneakerRepository.GetAll(cancellationToken);
    }

    // public async Task<Sneaker> Create()
    // {
    //     
    // }
}