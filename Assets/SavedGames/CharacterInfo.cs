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
                PortraitIndex = Universe.CrewConfiguration.GetPortraitIndex(portrait);
            }
        }

        public CrewMember Restore()
        {
            var allPortraits = Universe.CrewConfiguration.Portraits;
            Sprite portrait;
            if (PortraitIndex < 0 || PortraitIndex >= allPortraits.Count())
            {
                portrait = Universe.CrewConfiguration.DefaultPortrait;
            }
            else
            {
                portrait = Universe.CrewConfiguration.GetPortrait(PortraitIndex);
            }

            var crewMember = Universe.CrewConfiguration.NewCharacter(Name, portrait);
            return crewMember;
        }
    }
}
