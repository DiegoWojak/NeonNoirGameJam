
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextWritingBehavior : MonoBehaviour
{
    public TextMeshProUGUI textComponentA, textComponentB;  // Reference to the TextMeshPro component
    public float typingSpeed = 0.05f;      // Time between each character being typed
    public float delayAfterTyping = 2.0f;  // Delay before starting to delete the text
    public float delayBetweenText = 1.0f;
    public float deletingSpeed = 0.03f;
    [SerializeField]
    private string fullTextA;
    [SerializeField]
    private string fullTextB;

    public bool requested = true;
    private void Start()
    {
        if (!requested) return;

        StartCoroutine(TypeText( () => {
            StartCoroutine(AndDelete());
        }));
    }


    private IEnumerator TypeText(Action OnComplete)
    {
        // Typing effect
        for (int i = 0; i <= fullTextA.Length; i++)
        {
            textComponentA.text = fullTextA.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }

        // Wait for some time after typing is done
        yield return new WaitForSeconds(delayBetweenText);

        for (int i = 0; i <= fullTextB.Length; i++)
        {
            textComponentB.text = fullTextB.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }

        // Wait for some time after typing is done
        yield return new WaitForSeconds(delayAfterTyping);
        OnComplete?.Invoke();
    }

    private IEnumerator AndDelete(Action OnComplete=null) { 

        for (int i = fullTextB.Length; i >= 0; i--)
        {
            textComponentB.text = fullTextB.Substring(0, i);
            yield return new WaitForSeconds(deletingSpeed);
        }
        
        for (int i = fullTextA.Length; i >= 0; i--)
        {
            textComponentA.text = fullTextA.Substring(0, i);
            yield return new WaitForSeconds(deletingSpeed);
        }

        gameObject.SetActive(false);

        OnComplete?.Invoke();
    }

    public void RequestWriteMessage(string TiteA , string subtitle, Action OnComplete = null) {
        fullTextA = TiteA;
        fullTextB = subtitle;

        StartCoroutine(TypeText(() => {
            StartCoroutine(AndDelete(OnComplete));
        }));
    }
}
