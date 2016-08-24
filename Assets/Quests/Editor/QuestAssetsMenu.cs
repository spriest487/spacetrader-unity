using UnityEditor;

public class QuestAssetsMenu
{
    [MenuItem("Assets/Create/SpaceTrader/Quests/Collect Quest")]
    public static void QuestBoard()
    {
        ScriptableObjectUtility.CreateAsset<CollectItemsQuest>();
    }
}
