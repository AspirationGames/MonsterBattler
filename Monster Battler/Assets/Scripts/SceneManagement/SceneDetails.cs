using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded {get; private set;}
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            
            LoadScene();
            GameController.Instance.SetCurrentScene(this);
            
            
            //Load all connected scenes
            foreach (var scene in connectedScenes)
            {
                scene.LoadScene();
            }
            //Unload no longer connected scenes.
            if(GameController.Instance.PreviousScene != null)
            {
                var previosulyLoadedScenes = GameController.Instance.PreviousScene.connectedScenes;
                foreach (var scene in previosulyLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }

                }
            }

        }
    }

    public void LoadScene()
    {
        if(!IsLoaded)
            {
                SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive); //remeber scene needs to be the same name as the trigger game object.
                IsLoaded = true;
            }
    }

    public void UnloadScene()
    {
        if(IsLoaded)
            {
                SceneManager.UnloadSceneAsync(gameObject.name); //remeber scene needs to be the same name as the trigger game object.
                IsLoaded = false;
            }
    }
}
