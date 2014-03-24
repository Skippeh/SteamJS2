using System;
using Steam4NetApi;
using SteamJS2.JavascriptBindings.Implementations;

namespace SteamJS2
{
    internal static class SteamEventHandler
    {
        public static void StartListening()
        {
            OpenSteamApi.ChatMessage += OnChatMessage;
        }

        private static void OnChatMessage(ChatMessage message)
        {
            JSEvents.Execute("steam_chat_message", message);
        }
    }
}