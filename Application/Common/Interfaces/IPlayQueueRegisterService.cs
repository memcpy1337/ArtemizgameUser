using Application.Common.DTOs.Match;
using Netjection;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Common.Interfaces;

[InjectAsScoped]
public interface IPlayQueueRegisterService
{
    public Task AddUserToQueue(PlayQueueRegisterDto request, CancellationToken cancellationToken);
}
