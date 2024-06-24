using Application.Common.Interfaces;
using Contracts.Events.UserEvents;
using Domain.Entities;
using Mapster;
using MassTransit;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class UserRegisterConsumer : IConsumer<UserRegisterEvent>
{
    private readonly IRegisterUserService _registerService;

    public UserRegisterConsumer(IRegisterUserService registerService)
    {
        _registerService = registerService;
    }

    public async Task Consume(ConsumeContext<UserRegisterEvent> context)
    {
        var user = context.Message.Adapt<User>();

        await _registerService.RegisterAsync(user, context.CancellationToken);

    }
}
