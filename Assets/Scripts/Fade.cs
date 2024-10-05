using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image Panel;
    public float FadeDuration = 2.0f;

    private float timer = 0f;

    void Start()
    {
        if (Panel == null)
        {
            Panel = GetComponent<Image>();
        }

        // Set the initial color with alpha 0 (fully transparent)
        Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, 0f);
    }

    void Update()
    {
        if (timer < FadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / FadeDuration); 
            Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, alpha);
        }
    }
}
