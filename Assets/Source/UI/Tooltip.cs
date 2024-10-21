
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(content))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text= content;
        int _h_lenght = headerField.text.Length;
        int _c_lenght = contentField.text.Length;
        layoutElement.enabled = (_h_lenght > characterWrapLimit || _c_lenght > characterWrapLimit);
    }


    private void Update()
    {
        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.x / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
