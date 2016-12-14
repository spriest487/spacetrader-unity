using System;
using System.Linq;
using UnityEngine;

namespace SavedGames
{
    [Serializable]
    public class CharacterInfo
    {
        int transientId;
        public int TransientID { get { return transientId; } }

        public string Name { get; private set; }
        public int PortraitIndex { get; private set; }

        public CharacterInfo()
        {
        }

        public CharacterInfo(CrewMember fromCharacter, int transientId)
        {
            this.transientId = transientId;
            Name = fromCharacter.name;

            var portrait = fromCharacter.Portrait;
            if (portrait)
            {
                PortraitIndex = SpaceTraderConfig.CrewConfiguration.GetPortraitIndex(portrait);
            }
        }

        public CrewMember Restore()
        {
            var allPortraits = SpaceTraderConfig.CrewConfiguration.Portraits;
            Sprite portrait;
            if (PortraitIndex < 0 || PortraitIndex >= allPortraits.Count())
            {
                portrait = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
            }
            else
            {
                portrait = SpaceTraderConfig.CrewConfiguration.GetPortrait(PortraitIndex);
            }

            var crewMember = SpaceTraderConfig.CrewConfiguration.NewCharacter(Name, portrait);
            return crewMember;
        }
    }
}
