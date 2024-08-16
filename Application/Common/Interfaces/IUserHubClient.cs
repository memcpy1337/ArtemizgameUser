using Contracts.Common.Models.Enums;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IUserHubClient
{
    Task MatchmakingStatusUpdate(QueueStatusEnum newStatus);
    Task MatchReady(string address, int port, string ticket);
    Task MatchCancel(MatchCancelEnum reason);
    Task QueueRemove(QueuePlayerRemoveEnum reason);
}
