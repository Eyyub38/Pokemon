using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.Collections;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    [SerializeField] AudioClip sceneMusic;

    public AudioClip SceneMusic => sceneMusic;

    public bool IsLoaded{ get; private set; }
    
    List<SavableEntity> savebleEntities;
    
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){

            LoadScene(); 
            GameController.Instance.SetCurrentScene(this);
            if(sceneMusic != null){
                AudioManager.i.PlayMusic(sceneMusic,fade: true);
            }

            //Load all connected Scenes
            foreach(var scene in connectedScenes){
                scene.LoadScene();
            }

            //Unload the scenes that are no longer connected
            var prevScene = GameController.Instance.PrevScene;

            if(prevScene != null){
                var previouslyLoadedScenes = prevScene.connectedScenes;

                foreach(var scene in previouslyLoadedScenes){
                    if( !connectedScenes.Contains(scene) && scene != this){
                        scene.UnloadScene();
                    }
                }

                if(!connectedScenes.Contains(prevScene)){
                    prevScene.UnloadScene();
                }
            }
        }
    }

    public void LoadScene(){
        if(!IsLoaded){
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
            operation.completed += (AsyncOperation op) =>{
                savebleEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savebleEntities);
            };
        }
    }

    public void UnloadScene(){
        if(IsLoaded){
            SavingSystem.i.CaptureEntityStates(savebleEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false; 
        }
    }

    List<SavableEntity> GetSavableEntitiesInScene(){
            var currScene = SceneManager.GetSceneByName(gameObject.name);
            var savebleEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
            return savebleEntities;
    }
}
