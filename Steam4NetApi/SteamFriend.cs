using Steam4NET;

namespace Steam4NetApi
{
    public class SteamFriend
    {
        public CSteamID SteamID;
        public string Name;
        public EFriendRelationship Relationship;
        public EPersonaState Status;

        public void SendChatMessage(string message)
        {
            OpenSteamApi.SendFriendMessage(SteamID, message);
        }
    }
}