using System;
using System.Collections.Generic;
using Steam4NET;
using Steam4NetApi;
using Xilium.CefGlue;

namespace SteamJS2.JavascriptBindings.Implementations
{
    [JavascriptBinding("steam")]
    public class Steam
    {
        // Todo: do this automatically.
        // Persona states
        public int PERSONASTATE_ONLINE = (int)EPersonaState.k_EPersonaStateOnline;
        public int PERSONASTATE_BUSY = (int)EPersonaState.k_EPersonaStateBusy;
        public int PERSONASTATE_AWAY = (int)EPersonaState.k_EPersonaStateAway;
        public int PERSONASTATE_OFFLINE = (int)EPersonaState.k_EPersonaStateOffline;
        public int PERSONASTATE_SNOOZE = (int)EPersonaState.k_EPersonaStateSnooze;
        public int PERSONASTATE_LOOKINGTOPLAY = (int)EPersonaState.k_EPersonaStateLookingToPlay;
        public int PERSONASTATE_LOOKINGTOTRADE = (int)EPersonaState.k_EPersonaStateLookingToTrade;
        
        // Relationships
        public int RELATIONSHIP_FRIEND = (int)EFriendRelationship.k_EFriendRelationshipFriend;
        public int RELATIONSHIP_BLOCKED = (int)EFriendRelationship.k_EFriendRelationshipBlocked;
        public int RELATIONSHIP_IGNORED = (int)EFriendRelationship.k_EFriendRelationshipIgnored;
        public int RELATIONSHIP_IGNOREDFRIEND = (int)EFriendRelationship.k_EFriendRelationshipIgnoredFriend;
        public int RELATIONSHIP_NONE = (int)EFriendRelationship.k_EFriendRelationshipNone;
        public int RELATIONSHIP_SUGGESTED = (int)EFriendRelationship.k_EFriendRelationshipSuggested;
        public int RELATIONSHIP_REQUESTINITIATOR = (int)EFriendRelationship.k_EFriendRelationshipRequestInitiator;
        public int RELATIONSHIP_REQUESTRECIPIENT = (int)EFriendRelationship.k_EFriendRelationshipRequestRecipient;

        public bool IsLoggedIn()
        {
            return OpenSteamApi.LoggedIn;
        }

        public SteamFriend[] GetFriends()
        {
            return OpenSteamApi.GetFriends();
        }
    }
}