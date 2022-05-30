using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objects; //note we are using the object name not the given name (i.e. MonsterName vs scriptabal object name)

    public static void Init()
    {
        objects = new Dictionary<string, T>();

        var objectArray = Resources.LoadAll<T>(""); //loads all MonsterBase scriptable objects in the Resources folder

        foreach(var obj in objectArray)
        {
            
            if(objects.ContainsKey(obj.name)) //if the dictionary already contains the name of the monster it is trying to add
            {
                Debug.Log($"Two objects have the {obj.name} unable to add duplicate, check game object named {obj.name}");

            }

            objects[obj.name] = obj; //adds each monster to the dictionary
        }
    }

    public static T GetObjectByName(string objectName)
    {
        if(!objects.ContainsKey(objectName))
        {
            Debug.Log($"Object with name {objectName} doese not exist.");
            return null;
        }

        return objects[objectName];
    }
}