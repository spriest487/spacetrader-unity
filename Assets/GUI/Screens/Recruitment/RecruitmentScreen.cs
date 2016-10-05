using UnityEngine;

public class RecruitmentScreen : MonoBehaviour
{ 
    public void Close()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.None);
    }
}