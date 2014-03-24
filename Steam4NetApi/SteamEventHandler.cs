using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Steam4NET;

namespace Steam4NetApi
{
    internal class SteamEventHandler
    {
        public Action<ChatMessage> ChatMessage = null;

        private int pipe;
        private ISteamFriends013 steamFriends;

        private readonly Dictionary<int, Action<object>> callbacks = new Dictionary<int, Action<object>>();
        private readonly Dictionary<int, Type> callbackArgTypes = new Dictionary<int, Type>();

        public SteamEventHandler(int pipe, ISteamFriends013 steamFriends)
        {
            this.steamFriends = steamFriends;
            this.pipe = pipe;

            AddCallback(FriendChatMsg_t.k_iCallback, typeof (FriendChatMsg_t), OnFriendMessage);
        }

        private void AddCallback(int messageId, Type messageType, Action<object> callback)
        {
            callbacks.Add(messageId, callback);
            callbackArgTypes.Add(messageId, messageType);
        }

        public void HandlePendingCallbacks()
        {
            CallbackMsg_t message = default(CallbackMsg_t);
            while (Steamworks.GetCallback(pipe, ref message))
            {
                if (callbacks.ContainsKey(message.m_iCallback))
                {
                    var callback = callbacks[message.m_iCallback];

                    var argMessage = Marshal.PtrToStructure(message.m_pubParam, callbackArgTypes[message.m_iCallback]);
                    callback(argMessage);
                }

                Steamworks.FreeLastCallback(pipe);
            }
        }

        private void OnFriendMessage(object objMessage)
        {
            var message = (FriendChatMsg_t)objMessage;

            EChatEntryType messageType = default(EChatEntryType);
            var chatMessageBytes = new byte[2048];
            steamFriends.GetFriendMessage(message.m_ulFriendID, (int)message.m_iChatID, chatMessageBytes, ref messageType);
            var chatMessage = Encoding.UTF8.GetString(chatMessageBytes).TrimEnd((char)0); // Todo: can probably optimize this if needed

            var sender = OpenSteamApi.GetFriend(message.m_ulSenderID);

            OnChatMessage(new ChatMessage
                          {
                              Message = chatMessage,
                              Sender = sender,
                              Type = messageType
                          });
        }

        private void OnChatMessage(ChatMessage chatMessage)
        {
            if (ChatMessage != null)
                ChatMessage(chatMessage);
        }
    }
}