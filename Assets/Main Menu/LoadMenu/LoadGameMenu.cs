#pragma warning disable 0649

using System.Collections;
using SavedGames;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class LoadGameMenu : MonoBehaviour
{
    public class SaveDateOrder : IComparer<SavesFolder.Entry>
    {
        public int Compare(SavesFolder.Entry x, SavesFolder.Entry y)
        {
            return x.Header.TimeStamp.CompareTo(y.Header.TimeStamp);
        }
    }

    [SerializeField]
    private ToggleGroup fileList;

    [SerializeField]
    private Image selectedPortrait;

    [SerializeField]
    private SaveFileEntry entryPrefab;

    private PooledList<SaveFileEntry, string> entries;
    private int selectedIndex;
    
    private string GetSelectedFilePath()
    {
        return entries.Data.Skip(selectedIndex).FirstOrDefault();
    }

    private void OnMenuScreenActivate()
    {
        Debug.Assert(fileList);
        Debug.Assert(entryPrefab);
                
        Refresh();
    }

    private void Refresh()
    {
        if (entries == null)
        {
            entries = new PooledList<SaveFileEntry, string>(fileList.transform, entryPrefab);
        }

        var saves = SavesFolder.GetSaves().ToList();
        saves.Sort(new SaveDateOrder());

        var paths = saves.Select(s => s.Path);

        var fileCount = saves.Count;

        entries.Refresh(paths, (i, entry, path) =>
        {
            var header = saves[i].Header;

            string caption;

            var incompatible = header.Version != SaveHeader.CURRENT_VERSION;
            if (incompatible)
            {
                caption = "Incompatible save";
            }
            else
            {
                caption = string.Format("{0} ({1})", header.CharacterName, header.TimeStamp);
            }

            entry.Init();
            entry.group = fileList;
            entry.SetText(caption);
            entry.interactable = !incompatible;
            entry.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    selectedIndex = i;
                    selectedPortrait.sprite = header.GetPortraitSprite();
                }
            });

        });

        selectedIndex = Mathf.Clamp(selectedIndex, 0, fileCount - 1);

        if (selectedIndex >= 0 && selectedIndex < entries.Count)
        {
            entries[selectedIndex].isOn = true;
        }
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
        var path = GetSelectedFilePath();

        if (string.IsNullOrEmpty(path))
            return;

        SpaceTraderConfig.Instance.StartCoroutine(LoadGameRoutine(path));
    }

    private IEnumerator LoadGameRoutine(string path)
    {
        yield return null;
        var loading = ScreenManager.Instance.CreateLoadingScreen();

        var loadSave = SavesFolder.LoadGame(path);
        yield return loadSave;

        loading.Dismiss();

        if (loadSave.Error != null)
        {
            Debug.LogException(loadSave.Error);
        }
    }

    public void Delete()
    {
        var path = GetSelectedFilePath();

        if (string.IsNullOrEmpty(path))
            return;

        SavesFolder.DeleteGame(path);
        Refresh();
    }
}
