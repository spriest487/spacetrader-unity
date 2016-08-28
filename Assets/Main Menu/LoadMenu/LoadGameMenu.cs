using System.IO;
using System.Linq;
using SavedGames;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenu : MonoBehaviour
{
    [SerializeField]
    private ToggleGroup fileList;

    [SerializeField]
    private SaveFileEntry entryPrefab;

    private PooledList<SaveFileEntry, string> entries;

    private string[] filePaths;
    private string selectedPath;

    private void OnEnable()
    {
        if (fileList == null || entryPrefab == null)
        {
            Debug.LogWarning("LoadGameMenu: missing reference", this);
            return;
        }

        Refresh(true);
    }

    private void Refresh(bool first = false)
    {
        if (entries == null)
            entries = new PooledList<SaveFileEntry, string>(fileList.transform, entryPrefab);

        if (first)
            selectedPath = "";

        var paths = SavesFolder.GetFilePaths();

        var anyOn = false;

        entries.Refresh(paths, (i, entry, path) =>
        {
            if (entry.isOn)
            {
                anyOn |= entry.isOn;
                Select(path);
            }
                
            entry.Init();
            entry.group = fileList;
            entry.SetText(Path.GetFileNameWithoutExtension(path));
            entry.onValueChanged.AddListener(value =>
            {
                if (value)
                    Select(path);
            });

            if (!first && !anyOn && i == entries.Count - 1) //then the entry previously at this position was deleted
            {
                anyOn = true;
                entry.isOn = true;
            }
                
        });

        if (!first && !anyOn && paths.Length > 0) //then entry was deleted from the bottom select bottom
        {
            for (var i = 0; i < entries.Count; i++)
            {
                if (!entries[i].IsActive())
                    entries[i - 1].isOn = true;
            }
        }         
    }

    public void Select(string path)
    {
        selectedPath = path;
        Debug.Log("Selected: " + path);
    }

    public void Load()
    {
        if (string.IsNullOrEmpty(selectedPath))
            return;

        SavesFolder.LoadGame(selectedPath);
    }

    public void Delete()
    {
        if (string.IsNullOrEmpty(selectedPath))
            return;

        SavesFolder.DeleteGame(selectedPath);
        Refresh();
    }

}
