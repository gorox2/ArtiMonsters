using UnityEngine;
using System.Collections;
using System;

public class BossFightManager : MonoBehaviour
{
    public static BossFightManager instance;
    public event Action OnBossDefeated;

    [SerializeField] BossController boss;
    [SerializeField] BossHealthBarUI bossUI;
    [SerializeField] BossFightTrigger bossTrigger;
    [SerializeField] BossDeathReset bossDeathReset;
    //[SerializeField] BossArenaLock arenaLock;

    [Header("Intro")]
    [SerializeField] float introDelay = 0.3f;
    [SerializeField] bool freezePlayerDuringIntro = true;
    [SerializeField] playerMove playerMovementScript; 

    [Header("Audio")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip bossMusic;

    bool started;
    bool bossDead;

    public bool BossDead => bossDead;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void OnEnable()
    {    
        if (boss != null)
            boss.OnDied += HandleBossDied;
        if (bossDeathReset != null) bossDeathReset.onDeathReset += ResetFightState;
    }
    private void HandleBossDied()
    {
        if (bossDead) return;
        Debug.Log("boss dead set to true");
        bossDead = true;

        OnBossDefeated?.Invoke();
    }
    public void StartFight()
    {  
        if (started) return; 
        started = true;
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        //arenaLock?.Lock();

        if (boss != null) boss.gameObject.SetActive(true);

        if (bossUI != null) bossUI.Show(boss);

        if (freezePlayerDuringIntro && playerMovementScript != null)
            playerMovementScript.canMove = false;

        yield return new WaitForSeconds(introDelay);

        boss.StartBossFight(); 

        
        yield return new WaitUntil(() => boss.HasFightBegun); 

        if (freezePlayerDuringIntro && playerMovementScript != null)
            playerMovementScript.canMove = true;

        if (musicSource != null && bossMusic != null)
        {
            musicSource.clip = bossMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void ResetFightState()
    {
        started = false;
        bossDead = false;
        if (boss != null)
        {
            boss.ResetBossFight();
            boss.gameObject.SetActive(false); 
        }
        
        if (bossUI != null) bossUI.Hide(); 
        if(bossTrigger !=  null) bossTrigger.triggered = false;

        
        if (musicSource != null && musicSource.isPlaying && musicSource.clip == bossMusic)
            musicSource.Stop();

        
    }

    private void OnDisable()
    {
        if (boss != null)
            boss.OnDied -= HandleBossDied;
        if (bossDeathReset != null) bossDeathReset.onDeathReset -= ResetFightState; 
    }
}
