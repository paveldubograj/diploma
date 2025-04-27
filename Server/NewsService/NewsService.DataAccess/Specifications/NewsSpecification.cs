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
        Includes.Add(c => c.Tags);
    }
    public static NewsSpecification FilterNews(string? SearchString, string? CategoryId/*, List<Tag> tags*/){
        var predicate = PredicateBuilder.True<News>();
        if(!string.IsNullOrEmpty(SearchString)){
            predicate = predicate.And(news => news.Title.Contains(SearchString));
        }
        if(!string.IsNullOrEmpty(CategoryId)){
            predicate = predicate.And(news => news.CategoryId.Equals(CategoryId));
        }
        // if(tags.Count > 0){
        //     Console.WriteLine("Tags added");
        //     var f = new Func<News, bool>((news) => {
        //         foreach(var el in tags) {
        //             if(!news.Tags.Contains(el)) return false;
        //         }
        //         return true;
        //     });
        //     predicate.And(news => f(news));
        // }
        return new NewsSpecification(predicate);
    }

}