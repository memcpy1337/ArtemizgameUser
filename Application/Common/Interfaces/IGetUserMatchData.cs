using Domain.Entities;
using Netjection;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

[InjectAsScoped]
public interface IGetUserMatchData
{
    Task<User?> GetUserDataAsync(string userId, CancellationToken cancellationToken);
}
