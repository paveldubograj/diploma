using System;
using FluentValidation;
using TournamentService.BusinessLogic.Models.ParticipantDtos;

namespace TournamentService.BusinessLogic.Validators;

public class ParticipantValidator : AbstractValidator<ParticipantDto>
{
    public ParticipantValidator(){
        RuleFor(participant => participant.Name)
            .NotEmpty().WithMessage("Participant name is required.");
    }
}
