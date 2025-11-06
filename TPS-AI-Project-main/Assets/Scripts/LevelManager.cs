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

    [Header("Particles")]
    public Transform[] Particle;
    [Header("Gameplay")]
    
    private List<GameObject> enemies = new List<GameObject>();
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

    [Header("Boss & Combat Sounds")]
    public AudioClip bossDeathSound;
    public AudioClip bossHitPlayerSound;
    [Header("Coin UI")]
    public TextMeshProUGUI coinText;


    [Header("Audio Sources")]
    public AudioSource musicSource;

    private void Awake()
        {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }
    public void OnEnemyDied(GameObject enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);

        if (enemies.Count == 0)
            OnAllEnemiesDefeated();
    }
    public void OnAllEnemiesDefeated()
    {
        Debug.Log("All enemies defeated! Restoring main theme.");
        bossFightActive = false;
        PlayMusic(mainMusic);
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
