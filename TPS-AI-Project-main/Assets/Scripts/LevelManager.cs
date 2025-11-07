using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
    {
    public static LevelManager instance;
    [Header("Start Sound")]
    public AudioClip playerStartSound;
    [Header("Scene References")]
    public Transform player;
    [Header("Score")]
    public int score;
    public int coins;
    public TextMeshProUGUI scoreText;
    [Header("Victory")]
    public string playerVictoryTrigger = "Victory"; 
    public float victoryAnimLeadTime = 1.5f;        // let the anim play before pausing

    [Header("Particles")]
    public Transform[] Particle;

    [Header("UI")]
    public EndGameUI endGameUI;

    [Header("Gameplay")]
    private HashSet<GameObject> enemies = new HashSet<GameObject>();
    private bool victoryStarted = false;  
    public bool bossFightActive = false;
    [Header("Music")]
    //public AudioSource musicSource;
    public AudioClip mainMusic;
    public AudioClip bossMusic;
    public AudioClip[] levelSounds;
    public AudioClip[] enemyDamageSounds;
    public AudioClip[] playerDamageSounds;
    public AudioClip[] swordSwingSounds;
    public Transform MainCanvas;
    [Header("Endgame Music")]
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;
    [Header("Boss & Combat Sounds")]
    public AudioClip bossDeathSound;
    public AudioClip bossHitPlayerSound;
    [Header("Coin UI")]
    public TextMeshProUGUI coinText;


    [Header("Audio Sources")]
    public AudioSource musicSource;

    private void Awake()
        {

        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        // make sure this is reset when the scene loads
        CharacterStats.playerIsDead = false;
    }
        void Start()
    {
        UpdateCoinUI();
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        PlayMusic(mainMusic);

        // Cache enemies at start
        
        Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
           
        UpdateScoreUI(); // Initialize the text at start
        UpdateEnemyList();
    }
    public void UpdateEnemyList()
    {
        enemies.Clear();

        // Find all CharacterStats in the scene (ignores GFX tags completely)
        var allStats = GameObject.FindObjectsOfType<CharacterStats>(includeInactive: true);
        foreach (var cs in allStats)
        {
            if (cs == null || !cs.enabled) continue;

            // Count as enemy if this object OR its parent has an EnemyController or BossController
            bool countsAsEnemy = false;
            if (cs.GetComponentInParent<EnemyController>() != null)
                countsAsEnemy = true;
            else if (cs.GetComponentInParent<BossController>() != null)
                countsAsEnemy = true;

            if (countsAsEnemy)
                enemies.Add(cs.transform.root.gameObject);
        }

        PruneEnemies();
    }
    
    public void OnEnemyDied(GameObject enemy)
    {
        var root = enemy != null ? enemy.transform.root.gameObject : null;

        PruneEnemies();
        if (root != null) enemies.Remove(root);
        PruneEnemies();

        Debug.Log($"Enemy died. Remaining enemies: {enemies.Count}");
        if (enemies.Count == 0) OnAllEnemiesDefeated();
    }
    private void PruneEnemies()
    {
        enemies.RemoveWhere(e => e == null);
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy == null) return;
        enemies.Add(enemy.transform.root.gameObject);
    }

    public void OnAllEnemiesDefeated()
    {
        if (victoryStarted) return;  
        StartCoroutine(VictorySequence());
    }
    private IEnumerator VictorySequence()
    {
        victoryStarted = true;
        Debug.Log("All enemies defeated!");
        Debug.Log("VictorySequence started.");
        bossFightActive = false;

        // Music: swap to victory theme (falls back to main if not set)
        PlayMusic(victoryMusic != null ? victoryMusic : mainMusic);

        // Stop gameplay HUD
        if (MainCanvas != null) MainCanvas.gameObject.SetActive(false);

        // Trigger player victory anim (and stop movement)
        TryTriggerPlayerVictory();

        yield return new WaitForSeconds(victoryAnimLeadTime);

        if (endGameUI != null)
        {
            Debug.Log("Showing Victory UI.");
            endGameUI.ShowVictory(score, coins);
        }
        else
        {
            Debug.LogError("EndGameUI reference is NOT assigned on LevelManager. Please drag the EndGameUI Canvas into the LevelManager.");
        }

        
       


    }
    private void TryTriggerPlayerVictory()
    {
        if (player == null) return;
        var anim = player.GetComponentInChildren<Animator>();
        if (anim != null && !string.IsNullOrEmpty(playerVictoryTrigger))
            anim.SetTrigger(playerVictoryTrigger);

        var move = player.GetComponent<CharacterMovement>();
        if (move != null) move.enabled = false;
    }
    public void ShowDefeatScreen()
    {
        // Music: defeat sting or theme
        if (defeatMusic != null) PlayMusic(defeatMusic);

        // Hide gameplay HUD
        if (MainCanvas != null) MainCanvas.gameObject.SetActive(false);

        // Show defeat (already has a short delay in EndGameUI)
        if (endGameUI != null) endGameUI.ShowDefeat(score, coins);
    }


    void Update()
        {
            // Handle level updates here
        }
    public void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "x" + coins.ToString();
        else
            Debug.LogWarning("Coin Text is not assigned in LevelManager!");
    }

    public void AddScore(int amount)
         {
          score += amount;
          UpdateScoreUI();
         }
    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
        else
            Debug.LogWarning("Score Text is not assigned in LevelManager!");
    }
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        StartCoroutine(FadeToMusic(clip, 1.5f));
    }

    private IEnumerator FadeToMusic(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;

        // Fade out
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += Time.deltaTime / duration;
            yield return null;
        }
    }
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    

    public void PlaySound(AudioClip sound,Vector3 ownerPos )
    {
        GameObject obj = SoundFXPooler.current.GetPooledObject();
        AudioSource audio = obj.GetComponent<AudioSource>();

        obj.transform.position = ownerPos;
        obj.SetActive(true);
        audio.PlayOneShot(sound);

    }
    IEnumerator DisableSound(AudioSource audio)
    {
        while (audio.isPlaying)
            yield return new WaitForSeconds(0.5f);
        audio.gameObject.SetActive(false);
    }
}
