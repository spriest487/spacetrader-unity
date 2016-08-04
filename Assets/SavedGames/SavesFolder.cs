using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.Serialization;

namespace SavedGames
{
    public static class SavesFolder
    {
        private static readonly string Root = Path.Combine(Application.persistentDataPath, "Saves");
        
        private class LoadSaveOperation : LoadValueOperation<bool>
        {
            private IEnumerator Load(string path)
            {
                var binary = new BinaryFormatter();

                SavedGame save;
                using (var saveFile = File.OpenRead(path))
                {
                    try
                    {
                        save = binary.Deserialize(saveFile) as SavedGame;
                    }
                    catch (Exception ex)
                    {
                        save = null;
                        Error = ex;
                    }
                }

                if (save != null)
                {
                    yield return SpaceTraderConfig.Instance.StartCoroutine(save.RestoreState());

                    Result = true;
                }
                else
                {
                    Error = new InvalidOperationException("invalid or corrupt save file");
                }
            }

            public LoadSaveOperation(string path)
            {
                SpaceTraderConfig.Instance.StartCoroutine(Load(path));
            }
        }

        public static void SaveGame()
        {
            var save = SavedGame.CaptureFromCurrentState();

            var path = Path.Combine(Root, "Save1.dat");
            var binary = new BinaryFormatter();
            
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var saveFile = File.Create(path))
            {
                binary.Serialize(saveFile, save);
            }
        }

        public static LoadValueOperation<bool> LoadGame()
        {
            var path = Path.Combine(Root, "Save1.dat");

            return new LoadSaveOperation(path);
        }
    }
}
