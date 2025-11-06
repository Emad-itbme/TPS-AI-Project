using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class BossTrigger : MonoBehaviour
{
    public Transform boss;                // assign BossEnemy in Inspector
    public AudioClip bossMusic;           // dramatic boss theme
    public AudioClip normalMusic;         // normal background music
    public CinemachineCamera bossCam;     // temporary focus camera
    public AudioClip scaryLaugh;

    private bool triggered = false;
    AudioSource sfx;

    void Start()
    {
        sfx = GetComponent<AudioSource>();
        if (!sfx) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        Debug.Log("Boss fight started!");

        // focus camera
        var camController = FindObjectOfType<BossCameraController>();
        if (camController != null)
            camController.FocusOnBoss(3f);

        // play scary laugh first, then switch music
        if (scaryLaugh != null)
        {
            sfx.clip = scaryLaugh;
            sfx.Play();
            StartCoroutine(StartBossMusicAfter(scaryLaugh.length));
        }
        else
        {
            StartBossMusic();
        }
    }
    void StartBossMusic()
    {
        LevelManager.instance.bossFightActive = true;
        LevelManager.instance.PlayMusic(bossMusic);
    }

    IEnumerator StartBossMusicAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartBossMusic();
    }
    private IEnumerator FocusOnBoss()
    {
        
        if (bossCam != null)
            bossCam.Priority = 20;
        else
            Debug.LogWarning("Boss camera not assigned!");

        
        if (LevelManager.instance != null)
        {
            LevelManager.instance.bossFightActive = true;
            LevelManager.instance.PlayMusic(LevelManager.instance.bossMusic);
        }
        else
        {
            Debug.LogError("LevelManager instance not found!");
        }

        yield return new WaitForSeconds(3f);

        if (bossCam != null)
            bossCam.Priority = 5;

       
        if (LevelManager.instance != null)
        {
            LevelManager.instance.PlayMusic(normalMusic);
        }
    }
}
