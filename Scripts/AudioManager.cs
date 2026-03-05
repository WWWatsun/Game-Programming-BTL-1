using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip menuBgm;
    public AudioClip clickSfx;
    public AudioClip shootSfx;
    public AudioClip summarySfx;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    public void PlayMenuBgm()
    {
        if (menuBgm == null) return;
        if (bgmSource.clip == menuBgm && bgmSource.isPlaying) return;

        bgmSource.clip = menuBgm;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        if (bgmSource.isPlaying) bgmSource.Stop();
    }

    public void PlayClick()   { if (clickSfx != null) sfxSource.PlayOneShot(clickSfx); }
    public void PlayShoot()   { if (shootSfx != null) sfxSource.PlayOneShot(shootSfx); }
    public void PlaySummary() { if (summarySfx != null) sfxSource.PlayOneShot(summarySfx); }
}