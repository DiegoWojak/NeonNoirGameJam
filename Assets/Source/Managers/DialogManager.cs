using Assets.Source;
using Assets.Source.Managers;
using Assets.Source.UI.Dialog;

using System;

using UnityEngine;

public class DialogManager : LoaderBase<DialogManager>
{
    public string currentFrom;
    public string currentString;
    [HideInInspector]
    public Sprite CurrentSprite;
    [HideInInspector]
    public Sprite CurrentSpriteBtn;

    public DialogEntity UIDialog;
    
    public Action<string,string, Sprite, Sprite> OnRequestStringChange;
    public Action OnRequestClean;
    public bool UIOpened = false;
    [SerializeField]
    private Sprite backupSprite;
    [SerializeField]
    private Sprite backupSpriteBtn;
    public override void Init() 
    {
        OnRequestStringChange += ChangeString;
        OnRequestClean += Clean;

        isLoaded = true;
    }
    public void RequestOpen() 
    {
        if (UIOpened == false) {
            UIManager.Instance.RequestOpenUI(this, true ,(_request) => {
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


    private void ChangeString(string from, string msg, Sprite _sprite, Sprite _btnSprite) {
        currentFrom = from;
        currentString = msg;
        CurrentSprite = _sprite!=null?_sprite:backupSprite;
        CurrentSpriteBtn = _btnSprite != null ?_btnSprite:backupSpriteBtn;
    }
    private void Clean() {
        currentFrom = "nobody";
        currentString = "none";
        CurrentSprite = backupSprite;
        CurrentSpriteBtn = backupSpriteBtn;
    }

}
