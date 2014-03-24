using System;
using System.Collections.Generic;
using Steam4NET;
using Steam4NetApi;
using Xilium.CefGlue;

namespace SteamJS2.JavascriptBindings.Implementations
{
    [JavascriptBinding("steam")]
    public static class Steam
    {
        // Todo: do this automatically.
        // Persona states
        public static int STATUS_ONLINE = (int)EPersonaState.k_EPersonaStateOnline;
        public static int STATUS_BUSY = (int)EPersonaState.k_EPersonaStateBusy;
        public static int STATUS_AWAY = (int)EPersonaState.k_EPersonaStateAway;
        public static int STATUS_OFFLINE = (int)EPersonaState.k_EPersonaStateOffline;
        public static int STATUS_SNOOZE = (int)EPersonaState.k_EPersonaStateSnooze;
        public static int STATUS_LOOKINGTOPLAY = (int)EPersonaState.k_EPersonaStateLookingToPlay;
        public static int STATUS_LOOKINGTOTRADE = (int)EPersonaState.k_EPersonaStateLookingToTrade;
        
        // Relationships
        public static int RELATIONSHIP_FRIEND = (int)EFriendRelationship.k_EFriendRelationshipFriend;
        public static int RELATIONSHIP_BLOCKED = (int)EFriendRelationship.k_EFriendRelationshipBlocked;
        public static int RELATIONSHIP_IGNORED = (int)EFriendRelationship.k_EFriendRelationshipIgnored;
        public static int RELATIONSHIP_IGNOREDFRIEND = (int)EFriendRelationship.k_EFriendRelationshipIgnoredFriend;
        public static int RELATIONSHIP_NONE = (int)EFriendRelationship.k_EFriendRelationshipNone;
        public static int RELATIONSHIP_SUGGESTED = (int)EFriendRelationship.k_EFriendRelationshipSuggested;
        public static int RELATIONSHIP_REQUESTINITIATOR = (int)EFriendRelationship.k_EFriendRelationshipRequestInitiator;
        public static int RELATIONSHIP_REQUESTRECIPIENT = (int)EFriendRelationship.k_EFriendRelationshipRequestRecipient;

        // Message statuses
        public static int CHAT_MESSAGE = (int)EChatEntryType.k_EChatEntryTypeChatMsg;
        public static int CHAT_TYPING = (int)EChatEntryType.k_EChatEntryTypeTyping;

        public static bool IsLoggedIn()
        {
            return OpenSteamApi.LoggedIn;
        }

        public static SteamFriend[] GetFriends()
        {
            return OpenSteamApi.GetFriends();
        }

        public static SteamFriend GetFriend(ulong steamid)
        {
            var id = new CSteamID();
            id.SetFromUint64(steamid);
            return OpenSteamApi.GetFriend(id);
        }
    }
}