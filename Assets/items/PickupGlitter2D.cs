using UnityEngine;

public class PickupGlitter2D : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float minAlpha = 0.75f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPos;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float t = Time.time;

        // Alpha pulse
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(t * pulseSpeed) + 1f) * 0.5f);
            sr.color = c;
        }

        // Small floating bob
        transform.localPosition = startPos + Vector3.up * (Mathf.Sin(t * bobSpeed) * bobAmplitude);
    }
}
