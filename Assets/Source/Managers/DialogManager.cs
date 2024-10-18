using Assets.Source;
using Assets.Source.Managers;
using Assets.Source.UI.Dialog;
using Assets.Source.Utilities;
using System;

using UnityEngine;

public class DialogManager : LoaderBase<DialogManager>
{
    public string currentString;
    public DialogEntity UIDialog;

    public Action<string> OnRequestStringChange;
    public Action OnRequestClean;
    public bool UIOpened = false;

    public override void Init() 
    {
        OnRequestStringChange += ChangeString;
        OnRequestClean += Clean;

        isLoaded = true;
    }
    public void RequestOpen() 
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (UIOpened == false) { 
            UIDialog?.gameObject.SetActive(true);
            //GameStarterManager.Instance.CameraReadingChange(true);
            GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.ComputerInteracting);
            UpdateUItatus(true);
        }
    }

    public void RequestClose() 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (UIOpened == true) { 
            UIDialog?.gameObject.SetActive(false);
            //GameStarterManager.Instance.CameraReadingChange(false);
            GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.ComputerClose);
            UpdateUItatus(false);
        }
    }

    private void UpdateUItatus(bool st) {
        UIOpened = st;
    }


    private void OnDisable()
    {
        OnRequestStringChange -= ChangeString;
        OnRequestClean -= Clean;
    }


    private void ChangeString(string from) {
        currentString = from;
    }
    private void Clean() { 
        currentString = "none";
    }

}
