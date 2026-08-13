// Controllers.cs

using Yggdrasil.Data;
using Yggdrasil.Models;
using Yggdrasil.Tests.Factories;


namespace Yggdrasil.Tests.Util;

public static class ControllerUtil{
    /// <Summary>
    /// Create the constructor logic for controller tests
    /// </summary>
    /// <param name="factory">The mock web server context</param>
    /// <returns>Finnished configured mock context
    public static WebApplicationFactory<Program> Setup(WebApplicationFactory<Program> factory){
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
            });
        });
    }

public static (WebApplicationFactory<Program>, FakeHttpHandler) SetupWithHandler(WebApplicationFactory<Program> factory, Func<IServiceCollection, IServiceCollection> configureServices){
    var handler = new FakeHttpHandler();
    var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();
    var app = factory.WithWebHostBuilder(builder =>
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
            configureServices(services);
        }));
    return (app, handler);
}


    /// <summary>Database wrapper</summary>
    /// <param name="factory">The mock webserver context</param>
    /// <param name="action">The DB Factory call</param>
    /// <returns>The DB Factory return</returns>
    private static T WithDB<T>(WebApplicationFactory<Program> factory, Func<AppDbContext, T> action){
        using var scope = factory.Services.CreateScope();
        return action(scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }
    
    /// <summary>Create a procedural-filled world in the database</summary>
    /// <param name="factory">The mock webserver context</param>
    /// <returns>The world object</returns>
    public static World CreateWorld(WebApplicationFactory<Program> factory){
        return WithDB(factory, db => WorldFactory.Create(db));
    }

    /// <summary>Create a procedural-filled character in the database</summary>
    /// <param name="factory">The mock webserver context</param>
    /// <returns>The character object</returns>
    public static Character CreateCharacter(WebApplicationFactory<Program> factory, Guid? world_ID=null){
        return WithDB(factory, db => CharacterFactory.Create(db, world_ID));
    }

    /// <summary>Create a procedural-filled persona in the database</summary>
    /// <param name="factory">The mock webserver context</param>
    /// <returns>The persona object</returns>
    public static Persona CreatePersona(WebApplicationFactory<Program> factory){
        return WithDB(factory, db => PersonaFactory.Create(db));
    }

    /// <summary>Create a procedural-filled conversation in the database</summary>
    /// <param name="factory">The mock webserver context</param>
    /// <returns>The conversation object</returns>
    public static Conversation CreateConversation(WebApplicationFactory<Program> factory, Guid? world_ID=null){
        return WithDB(factory, db => ConversationFactory.Create(db, world_ID));
    }

    public static ChatMessage CreateChatMessage(WebApplicationFactory<Program> factory, Guid? conversation_ID=null){
        return WithDB(factory, db => ChatMessageFactory.Create(db, conversation_ID));
    }
    
    public static LLMConnection CreateConnection(WebApplicationFactory<Program> factory, SupportedProviders? provider=null){
        return WithDB(factory, db => LLMConnectionFactory.Create(db, provider));
    }
    
}
