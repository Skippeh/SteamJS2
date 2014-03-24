using Steam4NET;

namespace Steam4NetApi
{
    public class ChatMessage
    {
        public SteamFriend Sender;
        public string Message;
        public EChatEntryType Type;
    }
}