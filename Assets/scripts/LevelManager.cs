using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Win Condition")]
    [SerializeField] private int totalArtifactsInLevel = 5;

    [Header("References")]
    [SerializeField] private GameObject portalObject;          
    [SerializeField] private LevelCompletionUI levelCompleteUI;
    

    [SerializeField] Transform startingPoint;
    GameObject playerGO;

    private bool hasAllArtifacts;
    private bool bossDefeated;
    private bool levelCompleted;

    public bool BossDefeated => bossDefeated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (playerGO == null) playerGO = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        InventoryManager.Instance.SetTotalArtifactsForLevel(totalArtifactsInLevel);
        InventoryManager.Instance.ClearInventory();

        playerGO.transform.position = startingPoint.position;
        if (portalObject != null)
            portalObject.SetActive(false);

        
    }

    public void levelStart()
    {
       
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnArtifactProgressChanged += OnArtifactCountChanged;
        if (BossFightManager.instance != null)
            BossFightManager.instance.OnBossDefeated += OnBossDied;
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnArtifactProgressChanged -= OnArtifactCountChanged;
        if (BossFightManager.instance != null)
            BossFightManager.instance.OnBossDefeated -= OnBossDied;
    }


    private void OnArtifactCountChanged(int current, int total)
    {
        if (current < total) return;
        hasAllArtifacts = true;
        TryCompleteLevel();
    }

    
    public void OnBossDied()
    {
        bossDefeated = true;
        TryCompleteLevel();
    }

    private void TryCompleteLevel()
    {
        if (levelCompleted) return;

        if (!hasAllArtifacts) return;
        if (!bossDefeated) return;

        levelCompleted = true;

        
        if (levelCompleteUI != null)
            levelCompleteUI.Show();

        
        if (portalObject != null)
            portalObject.SetActive(true);
    }
}



