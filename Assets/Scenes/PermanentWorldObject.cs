using UnityEngine;
using System.Collections;

public class PermanentWorldObject : MonoBehaviour
{
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    void OnWorldEnd()
    {
        GameObject.Destroy(gameObject);
    }
}
