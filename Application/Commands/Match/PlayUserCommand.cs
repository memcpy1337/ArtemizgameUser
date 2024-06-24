using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.DTOs.Match;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Wrappers;
using Domain.Entities;
using Domain.Exceptions;
using Forbids;

namespace Application.Commands.Match;

public record PlayUserCommand(PlayUserRequest PlayUserRequest) : IRequestWrapper<PlayResponse>, IAuthorizedUser
{
    public string? RequestedUserId { get; set; }
    public string? RequestedUserName { get; set; }
}

internal sealed class PlayUserCommandHandler : IHandlerWrapper<PlayUserCommand, PlayResponse>
{
    private readonly IGetUserMatchData _userMatchData;
    private readonly IForbid _forbid;
    private readonly IPlayQueueRegisterService _playQueueRegister;

    public PlayUserCommandHandler(IGetUserMatchData userMatchData, IForbid forbid, IPlayQueueRegisterService playQueueRegister)
    {
        _userMatchData = userMatchData;
        _forbid = forbid;
        _playQueueRegister = playQueueRegister;
    }

    public async Task<IResponse<PlayResponse>> Handle(PlayUserCommand request, CancellationToken cancellationToken)
    {
        var userData = await _userMatchData.GetUserDataAsync(request.RequestedUserId!, cancellationToken);
        _forbid.Null(userData, UserNotFoundException.Instance);



        _playQueueRegister.AddUserToQueue()
        return Response.Success(new PlayResponse() { Ticket = "das"});
    }
}