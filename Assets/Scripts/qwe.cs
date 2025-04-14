using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTester : MonoBehaviour
{
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    //DELETE
    public bool StartAudio = false;
    private bool isRunning = false;

    // List of all sound IDs you want to test
    private readonly SoundManager.SoundID[] soundIDs = new SoundManager.SoundID[]
    {
        SoundManager.SoundID.RhinoWalk, SoundManager.SoundID.RhinoCurl, SoundManager.SoundID.RhinoHitWall, SoundManager.SoundID.RhinoRolling,
        SoundManager.SoundID.GoblinWalk, SoundManager.SoundID.SecretZoneAppearence,
        SoundManager.SoundID.SwordSwing1, SoundManager.SoundID.SwordSwing2, SoundManager.SoundID.SwordSwing3,
        SoundManager.SoundID.SwordSwing4, SoundManager.SoundID.SwordSwing5,
        SoundManager.SoundID.HeroDamage, SoundManager.SoundID.CatbossAppear, SoundManager.SoundID.CatbossHide,
        SoundManager.SoundID.BulletFly, SoundManager.SoundID.HeroJump, SoundManager.SoundID.HeroLand,
        SoundManager.SoundID.DashPickUp, SoundManager.SoundID.SwordPickUp,
        SoundManager.SoundID.WallJumpPickUp1, SoundManager.SoundID.WallJumpPickUp2,
        SoundManager.SoundID.SwordUpgrade, SoundManager.SoundID.Invoke,
        SoundManager.SoundID.RhinoGetDamage, SoundManager.SoundID.GoblinGetDamage,
        SoundManager.SoundID.MoneyBagBreak, SoundManager.SoundID.CoinHitGround1, SoundManager.SoundID.CoinHitGround2,
        SoundManager.SoundID.CoinCollect1, SoundManager.SoundID.CoinCollect2, SoundManager.SoundID.CoinCollect3,
        SoundManager.SoundID.CoinCollect4, SoundManager.SoundID.Dash,
        SoundManager.SoundID.GateOpen, SoundManager.SoundID.GateClose,
        SoundManager.SoundID.BossTopAttack, SoundManager.SoundID.TentaclesAttack, SoundManager.SoundID.UiButtonSelection, SoundManager.SoundID.UiButtonConfirm,
        SoundManager.SoundID.BlacksmithTalk1, SoundManager.SoundID.BlacksmithTalk2, SoundManager.SoundID.BlacksmithTalk3,
        SoundManager.SoundID.BlacksmithTalk4, SoundManager.SoundID.BlacksmithTalk5, SoundManager.SoundID.BlacksmithTalk6,
        SoundManager.SoundID.BlacksmithTalk7,  SoundManager.SoundID.TentaclesHide
    };
    private readonly float[] volumeadd = new float[]
    {
        0.25f, 0.3f, 0.35f, 0.05f,
        0.35f, 0.25f,
        0.55f, 0.55f, 0.55f, 
        0.55f, 0.55f, 
        0.35f, 0.15f, 0.15f,
        0.05f, 0.25f, 0.35f,
        0.3f, 0.3f,
        0.4f, 0.4f,
        0.3f, 0.35f,
        0.2f, 0.25f,
        0.1f,  0.05f, 0.05f,
        0.15f, 0.15f, 0.15f, 
        0.15f, 0.25f,
        0.3f, 0.3f,
        0.4f, 0.4f, 0.1f, 0.1f,
        0.6f, 0.6f, 0.6f, 
        0.6f, 0.6f, 0.6f, 
        0.6f, 0.5f
    }; //все 1.5
    void Update()
    {
        if (StartAudio && !isRunning)
        {
            StartAudio = false;
            StartCoroutine(PlaySoundsSequentially());
        }
    }

    private float targetDb = -6f; // Target dB for normalization

IEnumerator PlaySoundsSequentially()
{
    isRunning = true;

    for (int i = 0; i < soundIDs.Length; i++)
    {
        SoundManager.SoundID id = soundIDs[i];
        AudioClip clip = SoundManager.Instance.GetClipFromLibrary(id);

        if (clip != null)
        {
            float db = CalculateClipLoudness(clip);
            float volumeMultiplier = Mathf.Pow(10f, (targetDb - db) / 20f);
            volumeMultiplier = Mathf.Clamp01(volumeMultiplier); // keep it between 0 and 1

            Debug.Log($"{id} - dB: {db:F2}, Volume Multiplier: {volumeMultiplier:F2}");

            SoundManager.Instance.PlaySound(id, volumeUpdate: volumeadd[i] * 1.5f, worldPos: transform.position);
            yield return new WaitForSeconds(clip.length + 0.1f);
            // SoundManager.Instance.PlaySound(SoundManager.SoundID.AWALK, volumeUpdate: volumeMultiplier, worldPos: transform.position);
            // yield return new WaitForSeconds(clip.length + 0.1f);
            // SoundManager.Instance.PlaySound(id, volumeUpdate: volumeadd[i], worldPos: transform.position);
            // yield return new WaitForSeconds(clip.length + 0.2f);
        }
        else
        {
            Debug.LogWarning($"Clip for {id} is missing!");
        }
    }
    
    isRunning = false;
}


    float CalculateClipLoudness(AudioClip clip)
    {
        if (clip == null)
            return -80f;

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        double sum = 0;
        foreach (float sample in samples)
        {
            sum += sample * sample;
        }

        float rms = Mathf.Sqrt((float)(sum / samples.Length));
        float db = 20f * Mathf.Log10((float)rms);

        return Mathf.Clamp(db, -80f, 0f);
    }
}
