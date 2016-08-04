using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField]
    private Image portrait;

    [SerializeField]
    private InputField nameInput;

    private int loadScene;

    public void ScreenActive(int loadScene)
    {
        portrait.sprite = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
    }

    public void CyclePortrait(int diff)
    {
        var portraits = SpaceTraderConfig.CrewConfiguration.Portraits;
        Debug.Assert(portraits.Any());

        int selected = portraits.IndexOf(portrait.sprite);
        if (selected == -1)
        {
            selected = 0;
        }
        else
        {
            var portraitCount = portraits.Count;

            selected += diff;
            if (selected >= portraitCount)
            {
                selected = 0;
            }
            else if (selected < 0)
            {
                selected = portraitCount - 1;
            }
        }

        portrait.sprite = portraits[selected];
    }

    public void Submit()
    {
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        var pcPortrait = portrait.sprite;
        var pcName = nameInput.text;

        yield return SceneManager.LoadSceneAsync(loadScene);

        var ship = SpaceTraderConfig.LocalPlayer.Ship;
        if (ship)
        {
            var pc = SpaceTraderConfig.CrewConfiguration.NewCharacter(pcName, pcPortrait);
            pc.Assign(ship, CrewAssignment.Captain);
        }
    }
}
