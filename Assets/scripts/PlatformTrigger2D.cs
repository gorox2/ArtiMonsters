using UnityEngine;

public class PlatformTrigger2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovingPlatform2D targetPlatform;

    [Header("Player Filter")]
    [SerializeField] private string playerTag = "Player";

    [Header("Behavior")]
    [Tooltip("If true, it activates once and never turns off.")]
    [SerializeField] private bool oneShot = true;

    [Tooltip("If false and oneShot is false, platform deactivates when player exits trigger.")]
    [SerializeField] private bool stayActiveAfterExit = false;

    private bool used;

    private void Reset()
    {
        // Ensure this collider is trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (targetPlatform == null)
        {
            // Convenience: auto-find sibling/parent platform
            targetPlatform = GetComponentInParent<MovingPlatform2D>();
        }

        if (targetPlatform == null)
        {
            Debug.LogError($"[{name}] No MovingPlatform assigned/found.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (targetPlatform == null) return;
        if (!other.CompareTag(playerTag)) return;

        if (oneShot && used) return;

        targetPlatform.Activate();
        used = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (targetPlatform == null) return;
        if (!other.CompareTag(playerTag)) return;

        if (oneShot) return;
        if (stayActiveAfterExit) return;

        targetPlatform.Deactivate();
    }
}
