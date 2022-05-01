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

    Fader fader;
    
    private void Start() 
    {
        fader = FindObjectOfType<Fader>(); //this has to be in start because the Fader is instantiated in Awake
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.CharacterAnimator.IsMoving = false;
        StartCoroutine(SwitchScene());

    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);
        yield return SceneManager.LoadSceneAsync(sceneToLoadIndex);
        
        var destinationPortal = FindObjectsOfType<Portal>().First(x => x !=this && x.destinationPortal == this.destinationPortal); //This will retun the portal in the current scene
        player.Character.SetPositionAndSnapToTile(destinationPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier{A, B, C, D, E}