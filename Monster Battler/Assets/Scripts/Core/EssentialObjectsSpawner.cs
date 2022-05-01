using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;
    Vector3 startPosition = new Vector3 (0,0,0);

    private void Awake() 
    {
        var essentialCount = FindObjectsOfType<EssentialObjects>();
        if(essentialCount.Length == 0)
        {
            // If there is a grid, then spawn at the center of the grid.
            var grid = FindObjectOfType<Grid>();

            if(grid != null)
            {
                startPosition = grid.transform.position;
            }

            Instantiate(essentialObjectsPrefab, startPosition, Quaternion.identity);
        }

    }

}
