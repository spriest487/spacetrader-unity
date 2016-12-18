using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SavedGames
{
    public static class SavesFolder
    {
        public const int CURRENT_VERSION = 1;

        public class Entry
        {
            public string Path { get; private set; }
            public SaveHeader Header { get; private set; }

            public Entry(string path, SaveHeader header)
            {
                Path = path;
                Header = header;
            }
        }

        private static readonly string Root = Path.Combine(Application.persistentDataPath, "Saves");

        private static SaveHeader ReadHeader(BinaryReader saveReader, BinaryFormatter binary)
        {
            //read header length and header
            var headerLength = saveReader.ReadInt32();
            var headerBytes = saveReader.ReadBytes(headerLength);

            using (var headerStream = new MemoryStream(headerBytes))
            {
                var fields = binary.Deserialize(headerStream) as Dictionary<string, string>;

                return fields != null? new SaveHeader(fields) : null;
            }
        }

        private class LoadSaveOperation : LoadValueOperation<bool>
        {
            private IEnumerator Load(string path)
            {
                var binary = new BinaryFormatter();

                SavedGame save;
                using (var saveFile = File.OpenRead(path))
                using (var saveReader = new BinaryReader(saveFile))
                {
                    try
                    {
                        var header = ReadHeader(saveReader, binary);

                        if (header.Version != SaveHeader.CURRENT_VERSION)
                        {
                            save = null;
                            Error = new IOException("bad save version in header: " + header.Version);
                            yield break;
                        }

                        //rest of the bytes in the stream should be the savedgame
                        save = (SavedGame)binary.Deserialize(saveFile);
                    }
                    catch (Exception ex)
                    {
                        save = null;
                        Error = ex;
                    }
                }

                if (save != null)
                {
                    yield return Universe.Instance.StartCoroutine(save.RestoreState());

                    Result = true;
                }
                else
                {
                    Error = new IOException("invalid or corrupt save file");
                }
            }

            public LoadSaveOperation(string path)
            {
                Universe.Instance.StartCoroutine(Load(path));
            }
        }

        public static void SaveGame()
        {
            var save = SavedGame.CaptureFromCurrentState();
            var header = save.CreateHeader().ToDictionary();

            var path = Path.Combine(Root, save.UniqueName + ".dat");
            var binary = new BinaryFormatter();

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var saveFile = File.Create(path))
            using (var saveWriter = new BinaryWriter(saveFile))
            using (var headerStream = new MemoryStream())
            using (var gameStream = new MemoryStream())
            {
                binary.Serialize(headerStream, header);
                var headerBytes = headerStream.ToArray();

                //length of header followed by serializer header dictionary
                saveWriter.Write(headerBytes.Length);
                saveWriter.Write(headerBytes);

                //write serialized savegame
                binary.Serialize(saveFile, save);
            }
        }

        public static LoadValueOperation<bool> LoadGame(string path)
        {
            return new LoadSaveOperation(path);
        }

        public static void DeleteGame(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// read the header of every file in the saves root folder
        /// </summary>
        public static IEnumerable<Entry> GetSaves()
        {
            var binary = new BinaryFormatter();
            var saveFiles = Directory.Exists(Root) ? Directory.GetFiles(Root) : new string[0];

            var headers = new List<Entry>(saveFiles.Length);
            foreach (var fileName in saveFiles)
            {
                try
                {
                    using (var saveFile = File.OpenRead(fileName))
                    using (var saveReader = new BinaryReader(saveFile))
                    {
                        var header = ReadHeader(saveReader, binary);
                        if (header == null)
                        {
                            Debug.LogWarningFormat("unrecognized header in file in save folder: {0}", fileName);
                            continue;
                        }
                        headers.Add(new Entry(fileName, header));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("error reading header for file in save folder: {0} ({1})", fileName, e.Message);
                }
            }

            return headers;
        }
    }
}
