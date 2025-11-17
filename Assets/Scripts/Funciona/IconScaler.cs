using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeIcon : MonoBehaviour
{
    private RectTransform rect;
    private float initialDistance;
    private Vector3 initialScale;
    private float initialAngle;
    private float rotationOffset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Distances
            Vector2 p0 = t0.position;
            Vector2 p1 = t1.position;

            float currentDistance = Vector2.Distance(p0, p1);

            // ROTATION angle between touches
            float currentAngle = Mathf.Atan2(p1.y - p0.y, p1.x - p0.x) * Mathf.Rad2Deg;

            // When second finger starts
            if (t1.phase == TouchPhase.Began)
            {
                initialDistance = currentDistance;
                initialScale = rect.localScale;
                initialAngle = currentAngle;
                rotationOffset = rect.eulerAngles.z;
            }
            else
            {
                // SCALE
                float scaleFactor = currentDistance / initialDistance;
                rect.localScale = initialScale * scaleFactor;

                // ROTATION
                float angleDelta = currentAngle - initialAngle;
                rect.rotation = Quaternion.Euler(0, 0, rotationOffset + angleDelta);
            }
        }
    }
}
