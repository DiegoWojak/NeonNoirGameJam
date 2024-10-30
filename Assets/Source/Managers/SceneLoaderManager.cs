
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Managers
{
    public class SceneLoaderManager : LoaderBase<SceneLoaderManager>
    {
        [Serializable]
        public struct SceneInformation {
            public string SceneName;
            public string FinalMessageOnGameEnd;
            public string SomeRanking;
            public string ChapterIntro;
            public string ChapterSubName;
            public PredefinedMusics MusicBackgroundURL;
        }

        public List<SceneInformation> Scenes = new List<SceneInformation>();
        public int SceneTarget;
        public override void Init()
        {
            isLoaded = true;
        }

        private IEnumerator LoadSceneAsync(int sceneIndex,Action post)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Scenes[sceneIndex].SceneName);

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

        private IEnumerator LoadSceneAsync(string sceneNamex, Action post)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNamex);

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
        SceneTarget = 0;
        LoaderManager.Instance.EnqueueProcess(
            Pre: null,
            ProcessToLoad: LoadSceneAsync(SceneTarget,
                post: GameStarterManager.Instance.PreStartGame),
            Post: GameStarterManager.Instance.StartGame);
        }

        [ContextMenu("Second Level")]
        public void ChangeSceneSecondLevel() {
            SceneTarget = 1;
            LoaderManager.Instance.EnqueueProcess(
                Pre: null,
                ProcessToLoad: LoadSceneAsync(SceneTarget,
                    post: GameStarterManager.Instance.PreStartGame),
                Post: GameStarterManager.Instance.StartGame);
        }


        public void LoadNextLevel() {
            if (SceneTarget < Scenes.Count - 1)
            {
                SceneTarget++;

                LoaderManager.Instance.EnqueueProcess(
                Pre: null,
                ProcessToLoad: LoadSceneAsync(SceneTarget,
                    post: GameStarterManager.Instance.PreStartGame),
                Post: GameStarterManager.Instance.StartGame);
            }
            else {
                LoaderManager.Instance.EnqueueProcess(
                Pre: null,
                ProcessToLoad: LoadSceneAsync("10_MainMenu",
                null), null);
            }
        }
    }
}
