// Profile.cs

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System;
using System.Linq;

namespace Yggdrasil.Models.ValueObjects;

/// <summary>To gurantee order of the properties when building the prompts</summary>
[AttributeUsage(AttributeTargets.Property)]
public class OrderAttribute : Attribute{
    public int Value {get;}
    public OrderAttribute(int value) => Value = value;
}


[ComplexType]
public class Profile{
    [Order(1)] public required string FirstName {get; set;} = "";
    [Order(2)] public string? LastName {get; set;}
    [Order(3)] public required string Gender { get;set; }

    /// <summary>Pronouns combat gender confusion</summary>
    [Order(4)] public string? Pronouns { get; set; }
    [Order(5)] public string? Apperance { get; set; }
    [Order(6)] public string? Occupation { get; set; }
    [Order(7)] public string? Race { get; set; }
    [Order(8)] public string? Personality { get; set; }
    [Order(9)] public string? Background { get; set; }
}
