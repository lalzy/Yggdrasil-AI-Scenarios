# Yggdrasil

A scenario based web-app for running RP Sessions with an LLM, (contra companion or group-chatting, see [Terminology](#terminology) for the distinction).

**World:**
The world is where the RP takes place (can be based on reality, on fiction, whatever you think of). A world can contain 0 or more characters, and 0 or more lorebooks.

**Narrator**
The primary agent which handle the narration and orchestration of which characters are in a scene.

**Character:**
A primary (named, and fleshed out) character. Either separate Agent(s), or controlled by the narrator.

**Persona:**
The user "character". How you want the characters/narrator to 'see' or 'understand' what 'you' are.

### Current State:
Working towards MVP (Progress can be seen on the public [Trello](https://trello.com/b/1AmBLJMv)) but in essence, the MVP will have these features:
- Ability to create world, narrators, characters and personas.
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
6. If you want a different port. Edit the src/Properties/launchSettings.json

# Run The Service
Type ```dotnet run --project src/yggdrasil-ai-service.csproj``` in a terminal/command prompt (bash, powershell, cmd) pointed to the root (where yggdrasil-ai-service.sln is).

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
src/
  Controllers/ - API Endpoints
  Data/ - Database Definition
  Extensions - Extensions or object configurations(mapster)
  Models/
    /DTO - Data Transfer Objects
    /Entities - Database Table
	  /Enums - Shared enums.
  Pages/ - Razor Pages
  Properties/ - development settings
  Services/ - Business Logic
  wwwroot/ - static files
Migrations/ - EF Core Migrations. Migrations are auto-ran on dotnet run
Tests/ - Unit, Integration and E2E tests.
  /Controllers - Controller tests,
  /Data - Database Tests
  /E2E - E2E tests of frontend
  /Factories - Helpers to generate database data
  /Services - Service tests
  /TestUtil - testing utilities.
  
```

# API Documentation:
Run the service, then go to [http://localhost:5151/swagger](http://localhost:5151/swagger)
