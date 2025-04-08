using System;
using FluentValidation;
using NewsService.BusinessLogic.Models.News;
using NewsService.DataAccess.Entities;

namespace NewsService.BusinessLogic.Validators;

public class NewsValidator : AbstractValidator<NewsDto>
{
    public NewsValidator(){
        RuleFor(News => News.Title)
            .NotEmpty().WithMessage("Title should not be empty!")
            .MaximumLength(60).WithMessage("Title should not be longer than 60 symbols!");
        
        RuleFor(News => News.Content)
            .NotEmpty().WithMessage("Content should not be empty!");

        RuleFor(News => News.CategoryId)
            .NotEmpty().WithMessage("Category should be choosed!");
    }
}
