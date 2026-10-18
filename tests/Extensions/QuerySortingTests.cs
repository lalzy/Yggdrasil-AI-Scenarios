// QuerySortingTests.cs

using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Yggdrasil.Extensions;
using Yggdrasil.Models.Enums;
using System.Reflection;

namespace Yggdrasil.Tests.Utils;

public class QuerySortingTests{
    private class TestEntity{
        public Guid ID { get; }
        public string Name { get; } = "";
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
    }

    private static readonly IQueryable<TestEntity> Source = Array.Empty<TestEntity>().AsQueryable();

    private static string ConstantToText(Expression e) => e switch{
        ConstantExpression c => $"{c.Value}",
        MemberExpression { Expression: ConstantExpression c, Member: FieldInfo f } => $"{f.GetValue(c.Value)}"
    };

    private static string SortKeyToText(Expression body) => body switch{
        MethodCallExpression { Method.Name: "Property" } p => ConstantToText(p.Arguments[1]),
        MethodCallExpression c => $"{c.Method.Name}({SortKeyToText(c.Arguments[1])},{ConstantToText(c.Arguments[2])})"
    };

    private static string[] SortCallsToText(Expression expression){
        var calls = new System.Collections.Generic.List<string>();
        while (expression is MethodCallExpression call){
            var lambda = (LambdaExpression)((UnaryExpression)call.Arguments[1]).Operand;
            calls.Insert(0, $"{call.Method.Name}({SortKeyToText(lambda.Body)})");
            expression = call.Arguments[0];
        }
        return calls.ToArray();
    }
    
    [Theory]
    [InlineData(SortOrder.IDAsc,        new[]{ "OrderBy(ID)" })]
    [InlineData(SortOrder.IDDesc,       new[]{ "OrderByDescending(ID)" })]
    [InlineData(SortOrder.NameAsc,      new[]{ "OrderBy(Collate(Name,NOCASE))", "ThenBy(ID)" })]
    [InlineData(SortOrder.NameDesc,     new[]{ "OrderByDescending(Collate(Name,NOCASE))", "ThenBy(ID)" })]
    [InlineData(SortOrder.CreatedAsc,   new[]{ "OrderBy(CreatedAt)", "ThenBy(ID)" })]
    [InlineData(SortOrder.CreatedDesc,  new[]{ "OrderByDescending(CreatedAt)", "ThenBy(ID)" })]
    [InlineData(SortOrder.ModifiedAsc,  new[]{ "OrderBy(UpdatedAt)", "ThenBy(ID)" })]
    [InlineData(SortOrder.ModifiedDesc, new[]{ "OrderByDescending(UpdatedAt)", "ThenBy(ID)" })]
    public void SortBy_BuildsExpectedExpression(SortOrder sortOrder, string[] expected){
        Assert.Equal(expected, SortCallsToText(Source.SortBy(sortOrder).Expression));
    }
    
    [Fact]
    public void SortBy_UnknownSortOrder_Throws(){
        Assert.Throws<ArgumentOutOfRangeException>(() => Source.SortBy((SortOrder)999));
    }
}
