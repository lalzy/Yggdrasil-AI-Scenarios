// TestDBContext.cs

using System;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.TestUtil;

public class TestEntity{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}

public class TestEntityNoUpdateAt{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TestEntityOwner(){
    public Guid ID { get; set; }
    public List<TestEntityOwned>? Owned { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}

public class TestEntityOwned(){
    public Guid ID { get; set; }
    public String Name { get; set; }
    public TestEntityOwner? Owner { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}

public class TestDBContext : AppDbContext{
    public TestDBContext(DbContextOptions<AppDbContext> options) :base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<TestEntity>();
        modelBuilder.Entity<TestEntityNoUpdateAt>();
        modelBuilder.Entity<TestEntityOwned>();
        modelBuilder.Entity<TestEntityOwner>();
        base.OnModelCreating(modelBuilder);
    }
}
