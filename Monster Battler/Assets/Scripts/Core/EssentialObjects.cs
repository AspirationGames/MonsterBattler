using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    private void Awake() 
    {
        DontDestroyOnLoad(gameObject); //doesn't destroy on load this game object    
    }
}
