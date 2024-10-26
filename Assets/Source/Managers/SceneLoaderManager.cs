
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Managers
{
    public class SceneLoaderManager : LoaderBase<SceneLoaderManager>
    {
        public List<string> Scenes = new List<string>();
        public int SceneTarget;
        public override void Init()
        {
            /*StartCoroutine(
                LoadSceneAsync(SceneTarget ,() => { 
                    isLoaded = true;
                })
            );*/
            isLoaded = true;
        }

            private IEnumerator LoadSceneAsync(int sceneIndex,Action post)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Scenes[sceneIndex]);

                while (!asyncLoad.isDone)
                {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                yield return progress;
                #if UNITY_EDITOR
                    Debug.Log("Loading...");
                #endif
                }

                post?.Invoke();
            }

            [ContextMenu("Introduction")]
            public void ChanceSceneIntroduction() {
            LoaderManager.Instance.EnqueueProcess(
                Pre: null,
                ProcessToLoad: LoadSceneAsync(0,
                    post: GameStarterManager.Instance.PreStartGame),
                Post: GameStarterManager.Instance.StartGame);
            }

        [ContextMenu("Second Level")]
        public void ChangeSceneSecondLevel() {
            LoaderManager.Instance.EnqueueProcess(null, LoadSceneAsync(1, null), null);
        }
    }
}
