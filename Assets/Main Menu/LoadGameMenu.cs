using UnityEngine;

public class LoadGameMenu : MonoBehaviour
{
    [SerializeField]
    private Transform fileLayout;

    [SerializeField]
    private GameObject fileEntryPrefab;

    public void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            var fileEntry = Instantiate(fileEntryPrefab);
            fileEntry.transform.SetParent(fileLayout);
        }
    }

    public void LoadGame()
    {
        Debug.Log("Load Game");
    }

}
