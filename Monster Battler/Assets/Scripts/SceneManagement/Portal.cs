using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoadIndex = -1;

    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());

    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PauseGame(true);

        yield return SceneManager.LoadSceneAsync(sceneToLoadIndex);

        var destinationPortal = FindObjectsOfType<Portal>().First(x => x !=this && x.destinationPortal == this.destinationPortal); //This will retun the portal in the current scene
        player.Character.SetPositionAndSnapToTile(destinationPortal.SpawnPoint.position);

        GameController.Instance.PauseGame(false);
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier{A, B, C, D, E}