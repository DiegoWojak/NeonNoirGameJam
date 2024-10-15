using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source
{
    public class LoaderManager : MonoBehaviour
    {
        [UnityEngine.SerializeField]
        List<MonoBehaviour> Dependencies;

        [UnityEngine.SerializeField]
        Queue<IInitiable> _stacks = new Queue<IInitiable>();

        public bool isEverythingLoaded = false;

        public static Action OnEverythingLoaded;
        private void OnEnable()
        {
            _stacks = new Queue<IInitiable>();
            for (int i = 0; i < Dependencies.Count; i++) {
                _stacks.Enqueue(Dependencies[i] as IInitiable);
            }
            isEverythingLoaded = false;
            StartCoroutine(CoroutineLoad());
        }


        private IEnumerator CoroutineLoad() {
            while (_stacks.Count > 0)
            {
                var loader = _stacks.Dequeue();
                loader.Init();

                // Wait until the loader has finished loading
                while (!loader.IsLoaded())
                {
                    Debug.Log("Loading");
                    yield return new WaitForSeconds(0.5f);
                }
                Debug.Log($"Loaded  <-- {(loader as MonoBehaviour).name}");
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log("Everything is loaded");
            isEverythingLoaded = true;
            OnEverythingLoaded?.Invoke();
        }
    }
}
