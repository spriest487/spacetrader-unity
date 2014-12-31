using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string newGameScene;

    public void NewGame()
    {
        Application.LoadLevel(newGameScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
