using System;
using System.Reflection;
using UnityEngine;

public class GetComponents : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour mono in sceneActive)
        {
            FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < objectFields.Length; i++)
            {
                GetComponent attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(GetComponent)) as GetComponent;
                if (attribute != null)
                {
                    Type t = objectFields[i].FieldType;
                    MethodInfo method = typeof(GameObject).GetMethod("GetComponent", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
                    MethodInfo generic = method.MakeGenericMethod(t);
                    var obj = generic.Invoke(mono.gameObject, null);
                    var x = Convert.ChangeType(obj, t);
                    if (x.ToString() == "null")
                    {
                        Debug.LogError("Could not find " + t + " on " + mono.name + " though " + objectFields[i].DeclaringType + "."+ objectFields[i].Name + " is marked [GetComponent]");
                    }
                    objectFields[i].SetValue(mono, x);
                }
            }
        }
    }
}
