using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public enum MusicType { DefaultRoom = 0, DarkRoom = 1, Boss = 2 }
    public static MusicManager Instance;
    public AudioSource sourceA;
    public AudioSource sourceB;
    public AudioClip DefaultRoomMusic;
    public AudioClip BossMusic;
    public MusicType DefaultMusicType = MusicType.DefaultRoom;

    private AudioSource currentSource;
    private AudioSource nextSource;
    private string _currentSourceClipName;
    private Coroutine loopCoroutine;
    public float fadeDuration = 2f;
    public float MusicVolume;
    public float[] SoundVolumes = new float[3];
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        currentSource = sourceA;
        nextSource = sourceB;
    }

    void Start()
    {
        DefaultRoomMusic = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.DefaultRoomMusic);
        BossMusic = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.BossMusic);
        SetupSource(sourceA);
        SetupSource(sourceB);

        PlayLoop(DefaultRoomMusic, DefaultMusicType);
    }


    public void UpdateVolume(float musicVolume)
    {
        MusicVolume = musicVolume;
        Debug.Log(_currentSourceClipName);
        switch(_currentSourceClipName){
            case "Default": currentSource.volume = SoundVolumes[0] * MusicVolume; break;
            case "Dark": currentSource.volume = SoundVolumes[1] * MusicVolume; break;
            case "BossFight": currentSource.volume = SoundVolumes[2] * MusicVolume; break;
            default: break;
        }
    } 
    private void SetupSource(AudioSource source)
    {
        source.rolloffMode = AudioRolloffMode.Custom;
        source.minDistance = 0f;
        // source.maxDistance = 500f;
        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AnimationCurve.Linear(0f, 1f, 1f, 1f));
        // source.spatialBlend = 0;
        // source.spread = 180;
    }

    public void PlayLoop(AudioClip clip, MusicType type)
    {
        if (currentSource.clip == clip) return;
        _currentSourceClipName = clip.name;
        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);

        loopCoroutine = StartCoroutine(LoopWithCrossfade(clip, type));
    }


    private IEnumerator LoopWithCrossfade(AudioClip clip, MusicType type)
    {
        float nextVol = SoundVolumes[(int)type] * MusicVolume;

        nextSource.clip = clip;
        nextSource.volume = 0;
        nextSource.Play();

        float currentVol = currentSource.volume;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;
            currentSource.volume = Mathf.Lerp(currentVol, 0, lerp);
            nextSource.volume = Mathf.Lerp(0, nextVol, lerp);
            yield return null;
        }

        currentSource.Stop();
        (currentSource, nextSource) = (nextSource, currentSource);

        yield return new WaitForSeconds(clip.length - fadeDuration);
    }

}
