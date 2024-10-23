
using UnityEngine;

namespace Assets.Source
{
    public interface IInitiable
    {
        public bool IsLoaded();
        public abstract void Init();
    }

    public abstract class LoaderBase<T> : MonoBehaviour, IInitiable where T : LoaderBase<T>
    {
        protected bool isLoaded = false;
        public virtual void Init() {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject); // Prevent duplicate instances
                return;
            }
            if (this.gameObject.activeSelf) { 
                _instance = (T)this;
            }
        }

        private static T _instance;
        public static T Instance { get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }
                return _instance;
            } }

        public bool IsLoaded()
        {
            return isLoaded;
        }
    }
}
