
using Application.Common.Interfaces;
using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

internal sealed class RegisterUserService : IRegisterUserService
{
    private readonly IApplicationDbContext _context;

    public RegisterUserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegisterAsync(User user, CancellationToken cancellation)
    {
        await _context.Users.AddAsync(user, cancellation);
        await _context.SaveChangesAsync(cancellation);
    }
}
