using Assets.Source;
using Assets.Source.Managers;
using Assets.Source.UI.Dialog;
using Assets.Source.Utilities;
using System;

using UnityEngine;

public class DialogManager : LoaderBase<DialogManager>
{
    public string currentFrom;
    public string currentString;
    public DialogEntity UIDialog;
    
    public Action<string,string> OnRequestStringChange;
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
        if (UIOpened == false) {
            UIManager.Instance.RequestOpenUI(this, (_request) => {
                if (_request)
                {
                    UIDialog?.gameObject.SetActive(true);
                    GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.ComputerInteracting);
                    UpdateUItatus(true);
                }
                else 
                {
                    GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.ComputerClose);
                }
            });
        }
    }

    public void RequestClose() 
    {
        if (UIOpened == true) {
            UIManager.Instance.RequestCloseUI(this, (_request) => {
                if (_request)
                {
                    UIDialog?.gameObject.SetActive(false);
                    GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.ComputerClose);
                    UpdateUItatus(false);
                }
                else
                {
                    GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.ComputerTurning);
                }
            });
        }
        //Check if anyOtherUIIsOpened
    }

    private void UpdateUItatus(bool st) {
        UIOpened = st;
    }


    private void OnDisable()
    {
        OnRequestStringChange -= ChangeString;
        OnRequestClean -= Clean;
    }


    private void ChangeString(string from, string msg) {
        currentFrom = from;
        currentString = msg;
    }
    private void Clean() {
        currentFrom = "nobody";
        currentString = "none";
    }

}
