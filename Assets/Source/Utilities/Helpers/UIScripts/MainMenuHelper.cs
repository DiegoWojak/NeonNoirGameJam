

using Assets.Source.Managers;
using UnityEngine;

namespace Assets.Source.Utilities.Helpers.UIScripts
{
    public class MainMenuHelper : MonoBehaviour
    {

        public void GoFirstLevel() { 
            SceneLoaderManager.Instance.ChanceSceneIntroduction();
        }

        public void GoSecondLevel() {
            SceneLoaderManager.Instance.ChangeSceneSecondLevel();
        }

        public void ExitGame() { 
            Application.Quit();
        }
    }
}
