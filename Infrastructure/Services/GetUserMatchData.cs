using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class GetUserMatchData : IGetUserMatchData
{
    private readonly IApplicationDbContext _context;

    public GetUserMatchData(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserDataAsync(string userId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    }
}
