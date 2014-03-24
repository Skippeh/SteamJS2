using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Steam4NET;

namespace Steam4NetApi
{
    public static class OpenSteamApi
    {
        public static Action<ChatMessage> ChatMessage;

        private static ISteam006 steam006;
        private static ISteamClient012 steamClient;
        private static int pipe;
        private static int user;
        private static ISteamUser016 steamUser016;
        private static ISteamUtils005 steamUtils;
        private static ISteamFriends013 steamFriends013;
        private static ISteamFriends002 steamFriends002;
        private static Thread eventThread;
        private static bool doShutdown = false;
        private static SteamEventHandler eventHandler;

        public static bool LoggedIn
        {
            get
            {
                return steamUser016.BLoggedOn();
            }
        }

        public static bool LoadSteamClient()
        {
            bool success = Steamworks.Load(true);

            if (success)
            {
                steam006 = Steamworks.CreateSteamInterface<ISteam006>();
                var error = new TSteamError();
                steam006.ClearError(ref error);

                if (steam006.Startup(0, ref error) == 0)
                {
                    Console.Error.WriteLine("Failed to startup steam: " + error.szDesc);
                    return false;
                }

                steamClient = Steamworks.CreateInterface<ISteamClient012>();
                pipe = steamClient.CreateSteamPipe();
                user = steamClient.ConnectToGlobalUser(pipe);

                steamUser016 = steamClient.GetISteamUser<ISteamUser016>(user, pipe);
                steamUtils = steamClient.GetISteamUtils<ISteamUtils005>(pipe);
                steamFriends013 = steamClient.GetISteamFriends<ISteamFriends013>(user, pipe);
                steamFriends002 = steamClient.GetISteamFriends<ISteamFriends002>(user, pipe);

                eventHandler = new SteamEventHandler(pipe, steamFriends013);
                eventHandler.ChatMessage += message =>
                {
                    if (ChatMessage != null)
                        ChatMessage(message);
                };

                eventThread = new Thread(StartEventThread);
                eventThread.Start();
            }

            return success;
        }

        private static void StartEventThread()
        {
            while (!doShutdown)
            {
                eventHandler.HandlePendingCallbacks();
                Thread.Sleep(100);
            }
        }

        public static void Shutdown()
        {
            doShutdown = true;
        }

        public static SteamFriend[] GetFriends()
        {
            var result = new List<SteamFriend>();

            int friendCount = steamFriends002.GetFriendCount(EFriendFlags.k_EFriendFlagAll);

            for (int i = 0; i < friendCount; ++i)
            {
                var steamid = steamFriends002.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                result.Add(GetFriend(steamid));
            }

            return result.ToArray();
        }

        public static EPersonaState GetFriendStatus(CSteamID steamid)
        {
            return steamFriends002.GetFriendPersonaState(steamid);
        }

        public static EFriendRelationship GetFriendRelationship(CSteamID steamid)
        {
            return steamFriends002.GetFriendRelationship(steamid);
        }

        public static string GetFriendName(CSteamID steamid)
        {
            return steamFriends002.GetFriendPersonaName(steamid);
        }

        public static void SendFriendMessage(CSteamID steamID, string message)
        {
            steamFriends002.SendMsgToFriend(steamID, EChatEntryType.k_EChatEntryTypeChatMsg, Encoding.UTF8.GetBytes(message));
        }

        public static SteamFriend GetFriend(CSteamID steamid)
        {
            if (!steamid.IsValid())
                throw new Exception("SteamID is not valid.");

            return new SteamFriend
            {
                Name = GetFriendName(steamid),
                Relationship = GetFriendRelationship(steamid),
                Status = GetFriendStatus(steamid),
                SteamID = steamid
            };
        }
    }
}