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
    private Text selectedMoney;

    [SerializeField]
    private Text selectedLocation;

    [SerializeField]
    private SaveFileEntry entryPrefab;

    [SerializeField]
    private Button loadButton;

    [SerializeField]
    private Button deleteButton;

    private GUIController guiController;

    private PooledList<SaveFileEntry, string> entries;

    private SaveFileEntry selectedEntry;

    private void Awake()
    {
        guiController = GetComponentInParent<GUIController>();
    }

    private void OnEnable()
    {
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

        entries.Refresh(paths, (i, entry, path) =>
        {
            entry.group = fileList;
            entry.Assign(this, saves[i]);
        });

        SelectEntry(entries.FirstOrDefault());

        foreach (var entry in entries)
        {
            entry.isOn = entry == selectedEntry;
        }
    }

    public void Load()
    {
        if (selectedEntry == null)
        {
            return;
        }

        Universe.Instance.StartCoroutine(LoadGameRoutine(selectedEntry.SaveEntry.Path));
    }

    private IEnumerator LoadGameRoutine(string path)
    {
        yield return null;
        yield return guiController.ShowLoadingOverlay();

        var loadSave = SavesFolder.LoadGame(path);
        yield return loadSave;

        if (loadSave.Error != null)
        {
            Debug.LogException(loadSave.Error);
        }

        guiController.DismissLoadingOverlay();
        yield return guiController.SwitchTo(ScreenID.None);
    }

    public void Delete()
    {
        if (selectedEntry == null)
        {
            return;
        }

        SavesFolder.DeleteGame(selectedEntry.SaveEntry.Path);
        Refresh();
    }

    public void SelectEntry(SaveFileEntry entry)
    {
        selectedEntry = entry;

        if (entry)
        {
            var header = entry.SaveEntry.Header;

            selectedPortrait.sprite = header.GetPortraitSprite();
            if (!selectedPortrait.sprite)
            {
                selectedPortrait.sprite = Universe.CrewConfiguration.DefaultPortrait;
            }

            selectedLocation.text = header.Location;
            selectedMoney.text = Market.FormatCurrency(header.Money);
        }

        bool hasEntry = entry;

        loadButton.interactable = hasEntry;
        deleteButton.interactable = hasEntry;
        selectedPortrait.gameObject.SetActive(hasEntry);
    }
}
