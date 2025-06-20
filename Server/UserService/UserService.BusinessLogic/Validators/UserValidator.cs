using System;
using FluentValidation;
using UserService.BusinessLogic.Models.User;

namespace UserService.BusinessLogic.Validators;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(user => user.UserName)
            .NotEmpty().WithMessage("User name is required.");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Please provide a valid email address.");
        
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
