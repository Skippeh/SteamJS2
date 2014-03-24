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

        private readonly Dictionary<int, Action<CallbackMsg_t>> callbacks = new Dictionary<int, Action<CallbackMsg_t>>(); 

        public SteamEventHandler(int pipe, ISteamFriends013 steamFriends)
        {
            this.steamFriends = steamFriends;
            this.pipe = pipe;

            callbacks.Add(FriendChatMsg_t.k_iCallback, OnFriendMessage);
        }

        public void HandlePendingCallbacks()
        {
            CallbackMsg_t message = default(CallbackMsg_t);
            while (Steamworks.GetCallback(pipe, ref message))
            {
                if (callbacks.ContainsKey(message.m_iCallback))
                {
                    var callback = callbacks[message.m_iCallback];
                    callback(message);
                }

                Steamworks.FreeLastCallback(pipe);
            }
        }

        private void OnFriendMessage(CallbackMsg_t callbackMsgT)
        {
            var message = (FriendChatMsg_t)Marshal.PtrToStructure(callbackMsgT.m_pubParam, typeof (FriendChatMsg_t));

            EChatEntryType messageType = default(EChatEntryType);
            var chatMessageBytes = new byte[2048];
            steamFriends.GetFriendMessage(message.m_ulFriendID, (int)message.m_iChatID, chatMessageBytes, ref messageType);
            var chatMessage = Encoding.UTF8.GetString(chatMessageBytes).TrimEnd((char)0);

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