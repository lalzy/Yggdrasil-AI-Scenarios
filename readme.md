# Yggdrasil AI Service
A scenario based Web-app for running RP Sessions with an LLM (contra companion or group-chatting, see [Terminology](#terminology) for the distinction).

# Requirements:
- [.net 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- EF Core Tools: ```dotnet tool install --global dotnet-ef```
- OpenAI-compatible LLM-Keys (either through something like openrouter, or running your own with llama.cpp)

# Initial Setup
1. clone project (see the shiny green button)
2. ```cd {intoFolder}```
3. ```dotnet restore```
4. Either [Publish](#publish) for a stand-alone executable or [Run](#run-the-service) from bash.
5. Go to [http://localhost:5151](http://localhost:5151) in a browser

# Run The Service
1. (optional) if you want a different port, edit src/properties/LaunchSettings.json
2. Type ```dotnet run --project src/yggdrasil-ai-service.csproj``` in a terminal/command prompt (bash, powershell, cmd) pointed to the root (where yggdrasil-ai-service.sln is).

# Publish
1. Type ```dotnet publish```
2. Go to ```bin/yggdrasil-ai-service/release/net10.0/win-x64/publish``` and copy all the files there to wherever you want the standalone binary to be.
3. Run the binary (yggdrasil-ai-service.exe on windows)
4. Go to [http://localhost:5000](http://localhost:5000) in a browser
5. If you want a different port, edit the appsettings.json

# Tests
1. Open the project root(where the yggdrasil-ai-service.sln is)
2. Run tests with ```dotnet test```

# Project structure
```
./
├── src/
│   ├── Controllers/       API endpoints
│   ├── Data/              Database context
│   ├── Extensions/        Extensions, wrappers, Mapster config
│   ├── Models/
│   │   ├── DTO/           Data transfer objects
│   │   ├── Entities/      Database tables
│   │   │   ├── Instances/ Instance entities
│   │   │   └── Templates/ Template entities
│   │   │   ------------------------------------------------
│   │   ├── Enums/         Enums and SmartEnums
│   │   └── ValueObjects/  Complex entity types
│   │   ----------------------------------------------------
│   ├── Pages/             Razor Pages
│   ├── Properties/        Development settings
│   ├── Services/          Business logic
│   │   ├── Instances/     Instance services
│   │   └── Templates/     Template services
│   │   ----------------------------------------------------
│   │── Utils/             Misc Utilities
│   └── wwwroot/           Static files
│   --------------------------------------------------------
├── Migrations/            EF Core migrations (run on start)
└── Tests/
    ├── Controllers/       Controller tests
    ├── Data/              Database tests
    ├── E2E/               Frontend E2E tests
    ├── Extensions/        Extension tests
    ├── Factories/         Test data generators
    ├── Services/          Service tests
    │   ├── Instances/     Instance service tests
    │   └── Templates/     Template service tests
    │   ----------------------------------------------------
    └── TestUtil/          Utilities for testing
```

### Current State:
Working towards MVP (Progress can be seen on the public [Trello](https://trello.com/b/1AmBLJMv)) but in essence, the MVP will have these features:
- Ability to create world, narrators, characters, and personas.
- Create an LLM Connection (Any LLM supported through setting up the endpoint and format whichever you use expects).
- Preset LLM Connection for LLamaCpp and OpenRouter (no API key included, obviously).
- Encrypted chats.

### Plan / Future Features:
- Multiple Agent system (separate character and narrator).
- Agent-based Memory (narrator has its memory, a character has its).
- Lorebooks. Universal but can be attached to any character/world separately. Narrator use World's lorebook(s).
- State-tracking of world, characters and players (such as location).
- SillyTavern character card import support (where you can have it be a world, or a character or both).
- Export world/character support.
- Plugin support
- TTS
- Image-generation (through other API), + image gallery, and export/import of gallery to a world/character.

### Terminology:
- RP - RolePlay / RolePlaying
- Companion Roleplay - 1:1 conversation with an LLM where the LLM has a 'role'.
- Group Chat - Same as companion Roleplay, but User takes turn between user, and the LLM-Companions inside the groupchat.
- Scenario - the LLM is delegated into being a "narrator" and instead "create" or "use" characters.
- Context - LLM's "short term" memory. Essentially your current chat-log.
- Context bleed - an LLM hallucinate things that occured previously when it shouldn't because it confuse past with present.
- Lorebook - Defines lore/information that will be injected to the chat when certain keywords are triggered (as defined in the lore entries).
- Memory - Longterm memory. Unlike context, it is stored in the Database and added "when relevant".
