using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource sourceA;
    public AudioSource sourceB;

    private AudioSource currentSource;
    private AudioSource nextSource;

    private Coroutine loopCoroutine;
    public float fadeDuration = 2f;
    public float SfxVolume;
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
        sourceA.rolloffMode = AudioRolloffMode.Custom;
        sourceA.minDistance = 0f;
        sourceA.maxDistance = 500f;
        sourceA.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AnimationCurve.Linear(0f, 1f, 1f, 1f));
        sourceA.spatialBlend = 1;
        sourceA.spread = 180;

        sourceB.rolloffMode = AudioRolloffMode.Custom;
        sourceB.minDistance = 0f;
        sourceB.maxDistance = 500f;
        sourceB.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AnimationCurve.Linear(0f, 1f, 1f, 1f));
        sourceB.spatialBlend = 1;
        sourceB.spread = 180;

    }

    public void UpdateVolume(float sfxVolume)
    {
        SfxVolume = sfxVolume;
    } 
    public void PlayLoopIfDifferent(AudioClip newClip)
    {
        if (currentSource.clip == newClip) return;

        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);

        loopCoroutine = StartCoroutine(LoopWithCrossfade(newClip));
    }

    private IEnumerator LoopWithCrossfade(AudioClip clip)
    {
        float nextVol;
        switch(clip.name){
            case "DefaultRoomMusic": nextVol = SoundVolumes[0] * SfxVolume; break;
            case "DarkRoomMusic": nextVol = SoundVolumes[1] * SfxVolume; break;
            case "BossMusic": nextVol = SoundVolumes[2] * SfxVolume; break;
            default: Debug.Log(clip.name); nextVol = 0; break;
        }
        float fadeTime = fadeDuration;

        while (true)
        {
            nextSource.clip = clip;
            nextSource.volume = 0;
            nextSource.Play();
            float CVol = currentSource.volume;
            float t = 0;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                float lerp = t / fadeTime;
                currentSource.volume = Mathf.Lerp(CVol, 0, lerp);
                nextSource.volume = Mathf.Lerp(0, nextVol, lerp);
                yield return null;
            }

            currentSource.Stop();
            var temp = currentSource;
            currentSource = nextSource;
            nextSource = temp;

            yield return new WaitForSeconds(clip.length - fadeTime);
        }
    }
}
