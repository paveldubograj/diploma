using System;
using FluentValidation;
using MatchService.BusinessLogic.Models.Match;

namespace MatchService.BusinessLogic.Validators;

public class MatchUpdateDtoValidator : AbstractValidator<MatchUpdateDto>
{
    public MatchUpdateDtoValidator(){
        RuleFor(match => match.participant1Id)
            .NotEmpty().WithMessage("First participant is required");

        RuleFor(match => match.participant2Id)
            .NotEmpty().WithMessage("Second participant is required");

        RuleFor(match => match.matchOrder)
            .NotEmpty().WithMessage("Match order is required")
            .GreaterThan(0).WithMessage("Match order should be greater than 0");

        RuleFor(match => match.endTime)
            .NotEmpty().WithMessage("End time is required");

        RuleFor(match => match.startTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(match => match.winScore)
            .NotEmpty().WithMessage("Winner score is required")
            .GreaterThanOrEqualTo(0).WithMessage("Winner score should be greater or equal to 0");

        RuleFor(match => match.looseScore)
            .NotEmpty().WithMessage("Looser score is required")
            .GreaterThanOrEqualTo(0).WithMessage("Looser score should be greater or equal to 0");
    }
}
