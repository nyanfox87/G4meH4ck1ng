using UnityEngine;
using UnityEngine.UI;

public class FallingCookie : MonoBehaviour
{
    public float fallSpeed = 100f;
    private RectTransform rectTransform;
    private Image image;
    private float alpha = 1f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Random horizontal offset
        if (rectTransform != null)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.x += Random.Range(-80f, 80f);
            rectTransform.anchoredPosition = pos;
            rectTransform.localScale = Vector3.one * Random.Range(0.3f, 0.7f);
            rectTransform.Rotate(0, 0, Random.Range(-30f, 30f));
        }
    }

    void Update()
    {
        if (rectTransform != null)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y -= fallSpeed * Time.deltaTime;
            rectTransform.anchoredPosition = pos;
        }

        // Fade out
        alpha -= Time.deltaTime * 0.5f;
        if (image != null)
        {
            Color c = image.color;
            c.a = Mathf.Max(0, alpha);
            image.color = c;
        }
    }
}
