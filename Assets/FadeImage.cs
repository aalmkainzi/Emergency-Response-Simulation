using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public class FadeImage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    RawImage image;
    bool started = false;
    void Start()
    {
        image = GetComponent<RawImage>();
        started = true;
        Debug.Log("Start was called");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable was called");
        if (!started)
        {
            return;
        }
        image.color = new Color(0, 0, 0, 0);
        Debug.Log("Fading to black now");
        Tween t =
            PrimeTween.Tween.Custom(
            0.0f, 1.0f, 0.25f,
            onValueChange: (float f) =>
            {
                image.color = new Color(0, 0, 0, f);
            }
            );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
