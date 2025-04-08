using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Specifications.SpecSettings;

namespace NewsService.DataAccess.Specifications;

public class NewsSpecification : BaseSpecification<News>
{
    public NewsSpecification(Expression<Func<News, bool>> criteria) : base(criteria)
    {
        
    }
    public static NewsSpecification FilterNews(string? SearchString, string? CategoryId, List<Tag> tags){
        var predicate = PredicateBuilder.True<News>();
        if(!string.IsNullOrEmpty(SearchString)){
            predicate = predicate.And(news => news.Title.Contains(SearchString));
        }
        if(!string.IsNullOrEmpty(CategoryId)){
            predicate = predicate.And(news => news.CategoryId.Equals(CategoryId));
        }
        if(tags.Count > 0){
            Console.WriteLine("Tags added");
            predicate.And(news => new HashSet<Tag>(news.Tags).SetEquals(tags));
        }
        return new NewsSpecification(predicate);
    }
}