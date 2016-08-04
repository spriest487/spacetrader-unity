using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public void Update()
    {
        //stay on top
        transform.SetAsLastSibling();
    }

    public void Dismiss()
    {
        Destroy(gameObject);
    }
}
