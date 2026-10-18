// QuerySorting.cs

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Models.Enums;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Yggdrasil.Extensions;

public static class QuerySorting{

    /// <summary>Builds a key selector for the named property.</summary>
    private static Expression<Func<T, TKey>> BuildKeySelector<T, TKey>(string property) where T : class
        => x => EF.Property<TKey>(x, property);

    /// <summary>Builds a case-insensitive key selector for the named string property.</summary>
    private static Expression<Func<T, string>> NoStringCase<T>(string property) where T : class
        => x => EF.Functions.Collate(EF.Property<string>(x, property), "NOCASE");
    
    /// <summary>Sorts a query by the given sort order.Ties are broken by ID.</summary>
    /// <typeparam name="T">Entity type with ID, Name, CreatedAt and UpdatedAt properties.</typeparam>
    /// <param name="request">The query to sort.</param>
    /// <param name="sortOrder">The sort order to apply.</param>
    /// <returns>The sorted query.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="sortOrder"/> is not a known value.</exception>
    public static IQueryable<T> SortBy<T>(this IQueryable<T> request, SortOrder sortOrder) where T : class{
        var id = BuildKeySelector<T, Guid>("ID");

        return sortOrder switch{
            SortOrder.IDAsc => request.OrderBy(id),
            SortOrder.IDDesc => request.OrderByDescending(id),
            SortOrder.NameAsc => request.OrderBy(NoStringCase<T>("Name")).ThenBy(id),
            SortOrder.NameDesc => request.OrderByDescending(NoStringCase<T>("Name")).ThenBy(id),
            SortOrder.CreatedAsc => request.OrderBy(BuildKeySelector<T, DateTime>("CreatedAt")).ThenBy(id),
            SortOrder.CreatedDesc => request.OrderByDescending(BuildKeySelector<T, DateTime>("CreatedAt")).ThenBy(id),
            SortOrder.ModifiedAsc => request.OrderBy(BuildKeySelector<T, DateTime>("UpdatedAt")).ThenBy(id),
            SortOrder.ModifiedDesc => request.OrderByDescending(BuildKeySelector<T, DateTime>("UpdatedAt")).ThenBy(id),
            _ => throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null)
        };
    }
}
