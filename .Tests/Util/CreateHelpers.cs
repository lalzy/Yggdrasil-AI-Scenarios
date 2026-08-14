// CreateHelpers.cs

using Yggdrasil.Models;

namespace Yggdrasil.Tests.Util;

static class CreateHelpers
{
    static public (World, Conversation) CreateConversation(WebApplicationFactory<Program> factory)
    {
        var world = ControllerUtil.CreateWorld(factory);
        var conversation = ControllerUtil.CreateConversation(factory, world.ID);
        return (world, conversation);
    }

   static public (World, Conversation, ChatMessage) CreateMessageChain(WebApplicationFactory<Program> factory)
    {
        var (world, conversation) = CreateConversation(factory);
        return (world, conversation, ControllerUtil.CreateChatMessage(factory, conversation.ID));
    }
}
