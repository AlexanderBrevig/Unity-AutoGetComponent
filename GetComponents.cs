using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GetComponents : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        FindAllComponentAttributes();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FindAllComponentAttributes()
    {
        MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour mono in sceneActive)
        {
            FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in objectFields)
            {
                GetComponent attribute = Attribute.GetCustomAttribute(field, typeof(GetComponent)) as GetComponent;
                if (attribute != null)
                {
                    Type t = field.FieldType;
                    MethodInfo method = typeof(GameObject).GetMethod("GetComponent", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
                    MethodInfo generic = method.MakeGenericMethod(t);
                    var obj = generic.Invoke(mono.gameObject, null);
                    var x = Convert.ChangeType(obj, t);
                    if (x.ToString() == "null")
                    {
                        Debug.LogError("Could not find " + t + " on " + mono.name + " though " + field.DeclaringType + "." + field.Name + " is marked [GetComponent]");
                    }
                    field.SetValue(mono, x);
                }
            }
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        FindAllComponentAttributes();
    }
}

#if UNITY_EDITOR
[InitializeOnLoad]
public class ExecutionSetter
{
    static ExecutionSetter()
    {
        var target = UnityEngine.Object.FindObjectsOfType(typeof(GetComponents)).FirstOrDefault();
        if (target != null)
        {
            var executionOrder = -3000;
            MonoScript script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
            if (MonoImporter.GetExecutionOrder(script) != executionOrder)
            {
                MonoImporter.SetExecutionOrder(script, executionOrder);
            }
        }
    }
}
#endif
