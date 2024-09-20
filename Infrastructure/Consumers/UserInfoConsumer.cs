using Contracts.Events.UserEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class UserInfoConsumer : IConsumer<UserInfoRequest>
{
    private readonly IApplicationDbContext _context;

    public UserInfoConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<UserInfoRequest> context)
    {
        var data = await _context.Users.SingleAsync(x => x.Id == context.Message.UserId);

        await context.RespondAsync<UserInfoResponse>(new UserInfoResponse
        {
            UserId = data.Id,
            NickName = data.UserName
        });
    }
}
