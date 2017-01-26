using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SavedGames
{
    [Serializable]
    public class SaveHeader
    {
        public const int CURRENT_VERSION = 1;

        public int Version { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public string CharacterName { get; private set; }
        public int CharacterPortrait { get; private set; }

        public string Location {get; private set; }
        public int Money { get; private set; }

        public bool IsCompatible { get { return Version == CURRENT_VERSION; } }

        public SaveHeader(CharacterInfo playerCharacter,
            int playerMoney,
            string playerLocation)
        {
            Version = CURRENT_VERSION;
            TimeStamp = DateTime.UtcNow;

            if (playerCharacter != null)
            {
                CharacterName = playerCharacter.Name;
                CharacterPortrait = playerCharacter.PortraitIndex;
                Money = playerMoney;
                Location = playerLocation;
            }
            else
            {
                CharacterName = "Unnamed";
                CharacterPortrait = -1;
                Money = 0;
                Location = null;
            }
        }

        public SaveHeader(Dictionary<string, string> fields)
        {
            string versionField;
            int version;
            if (!fields.TryGetValue("Version", out versionField)
                || !int.TryParse(versionField, out version))
            {
                throw new IOException("missing version header field");
            }

            Version = version;

            if (!IsCompatible)
            {
                //don't try to read any more
                return;
            }

            string timeStampField;
            long timeStamp;
            if (!fields.TryGetValue("TimeStamp", out timeStampField)
                || !long.TryParse(timeStampField, out timeStamp))
            {
                throw new IOException("missing header field mandatory for this version");
            }
            TimeStamp = DateTime.FromBinary(timeStamp);

            string characterNameField;
            if (!fields.TryGetValue("CharacterName", out characterNameField))
            {
                throw new IOException("missing header field mandatory for this version");
            }
            CharacterName = characterNameField;

            string characterPortraitField;
            int characterPortrait;
            if (!fields.TryGetValue("CharacterPortrait", out characterPortraitField)
                || !int.TryParse(characterPortraitField, out characterPortrait))
            {
                throw new IOException("missing header field mandatory for this version");
            }
            CharacterPortrait = characterPortrait;

            string location;
            if (fields.TryGetValue("Location", out location))
            {
                Location = location;
            }
            else
            {
                Location = "?";
            }

            string money;
            int moneyAmount;
            if (fields.TryGetValue("Money", out money) && int.TryParse(money, out moneyAmount))
            {
                Money = moneyAmount;
            }
            else
            {
                Money = 0;
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>()
            {
                { "TimeStamp", TimeStamp.ToBinary().ToString() },
                { "Version", Version.ToString() },
                { "CharacterName", CharacterName },
                { "CharacterPortrait", CharacterPortrait.ToString() },
                { "Money", Money.ToString() },
                { "Location", Location }
            };
        }

        public Sprite GetPortraitSprite()
        {
            var crew = Universe.CrewConfiguration;
            if (CharacterPortrait >= 0 && CharacterPortrait < crew.PortraitCount)
            {
                return crew.GetPortrait(CharacterPortrait);
            }

            return null;
        }
    }
}
