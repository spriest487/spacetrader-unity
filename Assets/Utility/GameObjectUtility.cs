using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GameObjectUtility
{
    public static void SetLayerRecursive(this GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursive(layer);
        }
    }
}
