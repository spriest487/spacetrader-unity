using System;
using UnityEngine;

namespace SavedGames
{
    [Serializable]
    class CharacterInfo
    {
        int transientId;
        public int TransientID { get { return transientId; } }

        string name;
        int portraitIndex = -1;

        public CharacterInfo()
        {
        }

        public CharacterInfo(CrewMember fromCharacter, int transientId)
        {
            this.transientId = transientId;
            name = fromCharacter.name;

            var portrait = fromCharacter.Portrait;
            if (portrait)
            {
                portraitIndex = SpaceTraderConfig.CrewConfiguration.Portraits.IndexOf(portrait);
            }
        }

        public CrewMember Restore()
        {
            var allPortraits = SpaceTraderConfig.CrewConfiguration.Portraits;
            Sprite portrait;
            if (portraitIndex < 0 || portraitIndex < allPortraits.Count)
            {
                portrait = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
            }
            else
            {
                portrait = allPortraits[portraitIndex];
            }

            var crewMember = SpaceTraderConfig.CrewConfiguration.NewCharacter(name, portrait);
            return crewMember;
        }
    }
}
