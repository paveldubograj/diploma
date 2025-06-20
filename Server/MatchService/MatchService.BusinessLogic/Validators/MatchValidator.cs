using System;
using FluentValidation;
using MatchService.BusinessLogic.Models.Match;

namespace MatchService.BusinessLogic.Validators;

public class MatchValidator : AbstractValidator<MatchDto>
{
    public MatchValidator(){
        RuleFor(match => match.participant1Id)
            .NotEmpty().WithMessage("First participant is required");
        
        RuleFor(match => match.participant2Id)
            .NotEmpty().WithMessage("Second participant is required");

        RuleFor(match => match.round)
            .NotEmpty().WithMessage("Round is required");

        RuleFor(match => match.endTime)
            .NotEmpty().WithMessage("End time is required");

        RuleFor(match => match.startTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(match => match.winScore)
            .GreaterThanOrEqualTo(0).WithMessage("Winner score should be greater or equal to 0");

        RuleFor(match => match.looseScore)
            .GreaterThanOrEqualTo(0).WithMessage("Looser score should be greater or equal to 0");
    }
}
