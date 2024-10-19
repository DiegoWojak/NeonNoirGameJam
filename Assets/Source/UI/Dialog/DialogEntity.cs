
using TMPro;
using UnityEngine;

namespace Assets.Source.UI.Dialog
{
    public class DialogEntity : MonoBehaviour
    {
        private int page;
        [SerializeField]
        TMP_Text _title;
        [SerializeField]
        TMP_Text _text;
        string title = string.Empty;
        string msg = string.Empty;

        private void OnEnable()
        {
            title = DialogManager.Instance.currentFrom;
            msg = DialogManager.Instance.currentString;
            _title.SetText(title);
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
