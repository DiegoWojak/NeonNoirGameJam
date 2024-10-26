
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public List<Sprite> ImageList;
    private List<Sprite> imageList;
    [SerializeField] private Image displayImage;      // The UI Image component where images will be displayed
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade between images
    [SerializeField] private float displayTime = 2f;  // Time to display each image before transitioning

    private int currentImageIndex = 0;
    private float timer = 0f;
    private bool isFading = false;

    private void Start()
    {
        imageList = ImageList;
        if (imageList.Count > 0)
        {
            displayImage.sprite = imageList[currentImageIndex]; // Start with the first image
            displayImage.color = new Color(1, 1, 1, 1);         // Ensure image is fully visible
        }
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!isFading)
        {
            timer += Time.deltaTime;

            if (timer >= displayTime)
            {
                StartCoroutine(FadeOutIn());  // Start the fade transition
            }
        }
    }

    private IEnumerator FadeOutIn()
    {
        isFading = true;

        // Fade out the current image
        yield return StartCoroutine(FadeImage(1f, 0f));

        // Change to the next image in the list
        currentImageIndex = (currentImageIndex + 1) % imageList.Count;
        displayImage.sprite = imageList[currentImageIndex];

        // Fade in the next image
        yield return StartCoroutine(FadeImage(0f, 1f));

        timer = 0f;
        isFading = false;
    }

    private IEnumerator FadeImage(float fromAlpha, float toAlpha)
    {
        float elapsedTime = 0f;
        Color tempColor = displayImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tempColor.a = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / fadeDuration);
            displayImage.color = tempColor;
            yield return null;
        }

        // Ensure the final alpha is set precisely
        tempColor.a = toAlpha;
        displayImage.color = tempColor;
    }
}
