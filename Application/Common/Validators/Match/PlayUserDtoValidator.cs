using Application.Common.DTOs.Match;
using FluentValidation;

namespace Application.Common.Validators.Match;

public class PlayUserDtoValidator : AbstractValidator<PlayUserRequest>
{
    public PlayUserDtoValidator()
    {
        RuleFor(x => x.PlayerType).InclusiveBetween(0, 1).WithMessage("Incorrect player type");

        RuleFor(x => x.GameRegime).InclusiveBetween(0, 4).WithMessage("Incorrect game regime");
    }
}
