using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

class ContextInjector : MonoBehaviour
{
    private static readonly MethodInfo getMethod = typeof(Context).GetMethod("Get");
    private delegate void EachInjectedField(Component component, FieldInfo field, System.Type injectedType);

    private void ForInjectedFields(Component[] components, EachInjectedField action)
    {
        foreach (var component in components)
        {
            foreach (var field in component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var fieldType = field.FieldType;

                var injectAttrs = field.GetCustomAttributes(typeof(InjectedAttribute), true);
                if (injectAttrs.Length > 0)
                {
                    var attr = (InjectedAttribute)injectAttrs[0];

                    var fieldGenericArgs = fieldType.GetGenericArguments();
                    if (!typeof(ContextComponent<>).MakeGenericType(fieldGenericArgs).IsAssignableFrom(fieldType))
                    {
                        throw new UnityException("found injected field that was incompatible with IContextRef: " + field);
                    }

                    action(component, field, fieldGenericArgs[0]);
                }
            }
        }
    }

    private bool IsSingleton(Component component)
    {
        var componentAttrs = component.GetType().GetCustomAttributes(typeof(ContextSingletonAttribute), false);

        foreach (var attr in componentAttrs)
        {
            if (attr.GetType() == typeof(ContextSingletonAttribute))
            {
                return true;
            }
        }

        return false;
    }

    internal void Inject()
    {
        Context context = SceneContext.Instance;
        if (context != null)
        {
            var components = GetComponents<Component>();
            foreach (var component in components)
            {
                if (IsSingleton(component))
                {
                    context.Install(component.GetType(), component);
                }   
            }

            ForInjectedFields(components, (component, field, injectedType) =>
            {
                var contextRef = getMethod.MakeGenericMethod(injectedType).Invoke(context, new object[0]);

                field.SetValue(component, contextRef);
            });
        }
        else
        {
            Uninject();
        }
    }

    internal void Uninject()
    {
        var context = SceneContext.Instance;        
        var components = GetComponents<Component>();

        if (context != null)
        {
            foreach (var component in components)
            {
                if (IsSingleton(component))
                {
                    context.Uninstall(component.GetType());
                }
            }
        }

        /* we don't need an active context to uninject values */
        ForInjectedFields(components, (component, field, injectedType) =>
        {
            field.SetValue(component, null);
        });
    }

    void OnEnable()
    {
        Inject();
    }    

    void OnDisable()
    {
        Uninject();
    }
}