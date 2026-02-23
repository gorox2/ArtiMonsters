using TMPro;
using UnityEngine;

public class ArtifactTrackerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText;

    private void Start()
    {
        InventoryManager.Instance.OnArtifactProgressChanged += UpdateText;
        UpdateText(InventoryManager.Instance.CollectedArtifacts, InventoryManager.Instance.TotalArtifactsInLevel);
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnArtifactProgressChanged -= UpdateText;
    }

    private void UpdateText(int collected, int total)
    {
        progressText.text = $"Artifacts: {collected}/{total}";
    }
}
