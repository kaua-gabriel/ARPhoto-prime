using UnityEngine;

public class PulseUI : MonoBehaviour
{
    [Header("Configurações")]
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float speed = 4f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float pulse = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        float scale = Mathf.Lerp(minScale, maxScale, pulse);

        transform.localScale = originalScale * scale;
    }
}
