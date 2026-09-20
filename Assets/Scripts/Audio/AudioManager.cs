using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySFX(AudioClip clipToPlay)
    {
        Instance.sfxSource.pitch = 1;
        Instance.sfxSource.PlayOneShot(clipToPlay);
    }

    public static void PlaySFX(AudioClip clipToPlay, float pitch)
    {
        Instance.sfxSource.pitch = pitch;
        Instance.sfxSource.PlayOneShot(clipToPlay);
    }

    public static void PlayMusic(AudioClip musicToPlay)
    {
        Instance.musicSource.clip = musicToPlay;
        Instance.musicSource.Play();
    }

    public static void SetMusicVolume(float volume = 1)
    {
        Instance.musicSource.volume = volume;
    }

    public static void StopMusic()
    {
        Instance.musicSource.Stop();
    }
}