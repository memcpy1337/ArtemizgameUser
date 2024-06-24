using API.Routes;
using Application.Commands.Match;
using Application.Common.DTOs.Match;
using Application.Common.Models;
using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Endpoints.Match;

[Route(MatchRoutes.Play)]
public class Play : EndpointBaseAsync
    .WithRequest<PlayUserRequest>
    .WithActionResult<IResponse<string>>
{
    private readonly IMediator _mediator;

    public Play(IMediator mediator) => _mediator = mediator;

    [HttpPost,
     SwaggerOperation(Description = "Request to play",
         Summary = "Play request",
         OperationId = "User.Play",
         Tags = new[] { "Play" }),
     SwaggerResponse(200, "Reqest was proceed", typeof(IResponse<string>)),
     SwaggerResponse(400, "err", typeof(IResponse<string>)),
     Produces("application/json"), Consumes("application/json")]
    public override async Task<ActionResult<IResponse<string>>> HandleAsync(
        [SwaggerRequestBody("User play payload", Required = true)]
        PlayUserRequest playUserRequest,
        CancellationToken cancellationToken = new()) => Ok(await _mediator.Send(new PlayUserCommand(playUserRequest), cancellationToken));
}