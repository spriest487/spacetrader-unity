using UnityEngine;
using System.Collections;

public class ConnectingScreen : MonoBehaviour
{
    public string scene;

    void OnConnectedToServer()
    {
        Destroy(gameObject);

        Application.LoadLevelAdditive(scene);
    }
}
