using UnityEngine;
using System.Collections;

public class SceneContext : MonoBehaviour {
    public static Context Instance { get; private set; }

    private Context context;

    public Component[] values;    
    
    void OnEnable()
    {
        if (Instance != null)
        {
            throw new UnityException("one scene context is already active");
        }

        Instance = context = new Context(values);

        foreach (var injector in FindObjectsOfType<ContextInjector>())
        {
            injector.Inject();
        }
    }
    
    void OnDisable()
    {
        if (Instance == context)
        {
            foreach (var injector in FindObjectsOfType<ContextInjector>())
            {
                injector.Uninject();
            } 
            
            Instance = null;
        }
        else
        {
            throw new UnityException("another context was active while disabling scene context");
        }
    }
}
