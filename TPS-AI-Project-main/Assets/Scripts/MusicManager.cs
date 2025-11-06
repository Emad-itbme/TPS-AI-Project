using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Music Tracks")]
    public AudioSource mainTheme;
    public AudioSource bossTheme;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PlayMainTheme()
    {
        if (bossTheme.isPlaying)
            StartCoroutine(FadeOut(bossTheme, 1.5f));
        if (!mainTheme.isPlaying)
            StartCoroutine(FadeIn(mainTheme, 1.5f));
    }

    public void PlayBossTheme()
    {
        if (mainTheme.isPlaying)
            StartCoroutine(FadeOut(mainTheme, 1.5f));
        if (!bossTheme.isPlaying)
            StartCoroutine(FadeIn(bossTheme, 1.5f));
    }

    private IEnumerator FadeOut(AudioSource audio, float fadeTime)
    {
        float startVol = audio.volume;
        while (audio.volume > 0)
        {
            audio.volume -= startVol * Time.deltaTime / fadeTime;
            yield return null;
        }
        audio.Stop();
        audio.volume = startVol;
    }

    private IEnumerator FadeIn(AudioSource audio, float fadeTime)
    {
        audio.volume = 0;
        audio.Play();
        while (audio.volume < 1f)
        {
            audio.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}