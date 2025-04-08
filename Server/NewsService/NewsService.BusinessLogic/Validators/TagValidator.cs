using System;
using FluentValidation;
using NewsService.BusinessLogic.Models.Tag;

namespace NewsService.BusinessLogic.Validators;

public class TagValidator : AbstractValidator<TagDto>
{
    public TagValidator(){
        RuleFor(Tag => Tag.Name)
            .NotEmpty().WithMessage("Tag name should not be empty!")
            .MaximumLength(60).WithMessage("Tag name should not be longer than 60 symbols!");
    }
}
