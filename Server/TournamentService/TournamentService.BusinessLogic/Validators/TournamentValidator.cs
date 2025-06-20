using System;
using FluentValidation;
using TournamentService.BusinessLogic.Models.Tournament;

namespace TournamentService.BusinessLogic.Validators;

public class TournamentValidator : AbstractValidator<TournamentDto>
{
    public TournamentValidator(){
        RuleFor(tournament => tournament.Name)
            .NotEmpty().WithMessage("Tournament name is required.");
        
        RuleFor(tournament => tournament.DisciplineId)
            .NotEmpty().WithMessage("Category is required");
        
        RuleFor(tournament => tournament.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(tournament => tournament.EndDate)
            .NotEmpty().WithMessage("End date is required");
    }
}
