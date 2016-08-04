#pragma warning disable 0649

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField]
    private Image portrait;

    [SerializeField]
    private InputField nameInput;

    [SerializeField]
    private Text careerDescription;

    [SerializeField]
    private Text shipDescription;
    
    [SerializeField]
    private string newGameScene;

    [SerializeField]
    private CareerOptionButton selectedCareer;

    [SerializeField]
    private List<CareerOptionButton> careers;

    public void OnMenuScreenActivate()
    {
        portrait.sprite = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
        SelectCareer(0);
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

    public void SelectCareer(int index)
    {
        Debug.Assert(index >= 0 && index < careers.Count);
        var career = careers[index];

        careerDescription.text = career.Description;
        shipDescription.text = career.ShipType.name;

        careers.ForEach(c => c.SetHighlight(c == career));
    }

    public void Submit()
    {
        //run on global obj to persist through level change
        SpaceTraderConfig.Instance.StartCoroutine(LoadNextLevel(nameInput.text, portrait.sprite));
    }

    private IEnumerator LoadNextLevel(string pcName, Sprite pcPortrait)
    {
        var loading = ScreenManager.Instance.CreateLoadingScreen();
        
        yield return SceneManager.LoadSceneAsync(newGameScene);
        yield return new WaitForEndOfFrame();

        var ship = SpaceTraderConfig.LocalPlayer.Ship;
        if (ship)
        {
            var pc = SpaceTraderConfig.CrewConfiguration.NewCharacter(pcName, pcPortrait);
            pc.Assign(ship, CrewAssignment.Captain);
        }

        loading.Dismiss();
    }
}
