using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Source.UI.Dialog
{
    public class DialogEntity : MonoBehaviour
    {
        private int page;
        [SerializeField]
        TMP_Text _text;
        string msg = string.Empty;
        private void OnEnable()
        {
            msg = DialogManager.Instance.currentString;
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
