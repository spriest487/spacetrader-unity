using System.IO;
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
    private int selectedIndex, fileCount;
    private string selectedFilePath;

    private void OnEnable()
    {
        if (fileList == null || entryPrefab == null)
        {
            Debug.LogWarning("LoadGameMenu: missing reference", this);
            return;
        }

        if (entries == null)
            entries = new PooledList<SaveFileEntry, string>(fileList.transform, entryPrefab);

        ClearSelection();
        Select(-1, "");
        Refresh();
    }

    private void Refresh()
    {
        var paths = SavesFolder.GetFilePaths();

        fileCount = paths.Length;

        entries.Refresh(paths, (i, entry, path) =>
        {
            entry.Init();
            entry.group = fileList;
            entry.SetText(Path.GetFileNameWithoutExtension(path));
            entry.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                    Select(i, path);
            });

        });

        if (fileCount == 0 || selectedIndex < 0)
            return;

        ClearSelection();

        entries[Mathf.Clamp(selectedIndex, 0, fileCount - 1)].isOn = true;            
    }

    public void Select(int index, string path)
    {
        selectedIndex = index;
        selectedFilePath = path;
    }

    public void ClearSelection()
    {
        if (selectedIndex < 0 || selectedIndex >= entries.Count)
            return;

        fileList.allowSwitchOff = true;
        entries[selectedIndex].isOn = false;
        fileList.allowSwitchOff = false;
    }

    public void Load()
    {
        if (string.IsNullOrEmpty(selectedFilePath))
            return;

        SavesFolder.LoadGame(selectedFilePath);
    }

    public void Delete()
    {
        if (string.IsNullOrEmpty(selectedFilePath))
            return;

        SavesFolder.DeleteGame(selectedFilePath);
        Refresh();
    }

}
