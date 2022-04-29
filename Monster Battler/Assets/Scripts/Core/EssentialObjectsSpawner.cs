using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake() 
    {
        var essentialCount = FindObjectsOfType<EssentialObjects>();
        if(essentialCount.Length == 0)
        {
            Instantiate(essentialObjectsPrefab, new Vector3(0,0,0), Quaternion.identity);
        }
    }

}
