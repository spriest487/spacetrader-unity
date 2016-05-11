using UnityEngine;

public class RecruitmentScreen : MonoBehaviour
{
    public void Close()
    {
        ScreenManager.Instance.ScreenID = ScreenID.None;
    }
}