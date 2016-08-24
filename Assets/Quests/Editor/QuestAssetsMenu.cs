using UnityEditor;

public class QuestAssetsMenu
{
    [MenuItem("Assets/Create/SpaceTrader/Quests/Collect Quest")]
    public static void CollectItems()
    {
        ScriptableObjectUtility.CreateAsset<CollectItemsQuest>();
    }
    
    [MenuItem("Assets/Create/SpaceTrader/Quests/Kill X Ships Quest")]
    public static void KillXShips()
    {
        ScriptableObjectUtility.CreateAsset<KillShipTypeQuest>();
    }
}
