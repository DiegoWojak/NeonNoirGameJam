
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.UI.Dialog
{
    public class DialogEntity : MonoBehaviour
    {
        private int page;
        [SerializeField]
        TextMeshProUGUI _title;
        [SerializeField]
        TextMeshProUGUI _text;
        [SerializeField]
        Image _image;
        [SerializeField]
        Image _iconBtn;
        string title = string.Empty;
        string msg = string.Empty;

        private void OnEnable()
        {
            title = DialogManager.Instance.currentFrom;
            msg = DialogManager.Instance.currentString;
            _image.sprite = DialogManager.Instance.CurrentSprite;
            _iconBtn.sprite = DialogManager.Instance.CurrentSpriteBtn;
            _title.SetText($"Device Name: {title}");
            _text.SetText(msg);
            page = 1;
        }

        private void OnDisable()
        {

        }

        public void NextPage() {
            Debug.Log($"Requesting next page");
            //SoundManager <--
            if (msg.Length > 500 * page) {
                page++;
            }
        }

        public void Close() {
            Debug.Log($"Requesting Close");
            DialogManager.Instance?.RequestClose();
        }
    }
}
