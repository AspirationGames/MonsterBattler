using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded {get; private set;}

    List<SavableEntity> savableEntities;
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
                var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive); //remeber scene needs to be the same name as the trigger game object.
                IsLoaded = true;
                //we save the scene in a variable to determine if the scene loading has been completed. 
                //This will prefent the script from completing other function that require scene loading to be completed first.
                
                //This lamda function will ensure that the scene is completely loaded before the savable entities are loaded.
                operation.completed += (AsyncOperation op) =>
                {
                    savableEntities = GetSaveAbleEntitiesInScene();
                    SavingSystem.i.RestoreEntityStates(savableEntities);
                };

                
            }
    }

    public void UnloadScene()
    {
        if(IsLoaded)
            {
                SavingSystem.i.CaptureEntityStates(savableEntities);

                SceneManager.UnloadSceneAsync(gameObject.name); //remeber scene needs to be the same name as the trigger game object.
                IsLoaded = false;
            }
    }

    List<SavableEntity> GetSaveAbleEntitiesInScene()
    {
        var currentScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();

        return savableEntities;
    }
}
