// DatabaseSetup.cs

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.TestUtil;

public class DatabaseSetup : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly WebApplication App;
    protected readonly FakeTimeProvider _clock = new();

    protected DatabaseSetup(){
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<TimeProvider>(_clock);
        builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));
        App = builder.Build();
        using var scope = App.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }

    protected AppDbContext Db(){
        return App.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public void Dispose(){
        _connection.Dispose();
    }
}
