using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public enum SoundID
    {
        //Music
        MainMenuMusic, DefaultRoomMusic, BossMusic, DarkRoomMusic,
        //Ambient
        RhinoWalk, RhinoCurl, RhinoHitWall, RhinoRolling, GoblinWalk, BlacksmithTalk1, BlacksmithTalk2,
        BlacksmithTalk3, BlacksmithTalk4, BlacksmithTalk5, BlacksmithTalk6, BlacksmithTalk7, SecretZoneAppearence,
        //SFX 17
        SwordSwing1, SwordSwing2, SwordSwing3, SwordSwing4, SwordSwing5, HeroDamage, CatbossAppear, CatbossHide, BulletFly,
        HeroJump, HeroLand, DashPickUp, SwordPickUp, WallJumpPickUp1, WallJumpPickUp2, SwordUpgrade, Invoke, RhinoGetDamage, 
        GoblinGetDamage, MoneyBagBreak, CoinHitGround1, CoinHitGround2, CoinCollect1, CoinCollect2, CoinCollect3, 
        CoinCollect4, Dash, GateOpen, GateClose, BossTopAttack, TentaclesAttack, UiButtonSelection, UiButtonConfirm,
        TentaclesHide, AWALK, SphereDestructSoundGlass, SphereDestructSoundSouls, Meow
    }
    
    [System.Serializable]
    public struct SoundData
    {
        public SoundID ID;
        public AudioClip Clip;
    }      
    private List<AudioSource> MusicSounds = new List<AudioSource>();
    private List<AudioSource> AmbientSounds = new List<AudioSource>();
    private List<AudioSource> SoundEffects = new List<AudioSource>();
    public List<SoundData> Sounds = new List<SoundData>();
    private Dictionary<SoundID, AudioClip> soundLibraryDict = new Dictionary<SoundID, AudioClip>();
    public AudioSource PlayerWalk;
    public float MasterVolume;
    public float MusicVolume;
    public float AmbientVolume;
    public float SfxVolume;
    private const int _initialSFXPoolSize = 10;
    // public List<AudioSource> RhinoSound = new List<AudioSource>();
    // public List<AudioSource> GoblinSound = new List<AudioSource>();
    public List<GoblinMonster> Goblins = new List<GoblinMonster>();
    public List<RhinoMonster> Rhinos = new List<RhinoMonster>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // optional, if you want the sound manager to persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SetVolume();
        InitializeSoundLibraries();
    }

    public void PlaySound(SoundID id, Vector2? worldPos = null, float? volumeUpdate = 1, bool loop = false, int soundType = 3, float pitch = 1)
    {
        AudioClip clip = GetClipFromLibrary(id);
        if (clip == null) return; 

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        switch(soundType)
        {
            case 1: MusicSounds.Add(audioSource); break;
            case 2: AmbientSounds.Add(audioSource); break;
            case 3: SoundEffects.Add(audioSource); break;
        }

        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.minDistance = 0f;
        audioSource.minDistance = pitch;
        audioSource.maxDistance = 500f;
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AnimationCurve.Linear(0f, 1f, 1f, 1f));
        audioSource.transform.position = worldPos ?? transform.position;
        audioSource.clip = clip;
        audioSource.spatialBlend = 1;
        audioSource.spread = 180;
        audioSource.volume = MasterVolume * GetVolumeForType(soundType) * (float)volumeUpdate;
        audioSource.loop = loop;

        audioSource.Play();

        if (!loop)
        {
            Destroy(audioSource, clip.length);
        }
    }
    public float GetVolumeForType(int soundType)
    {
        switch(soundType)
        {
            case 1: return MusicVolume;
            case 2: return AmbientVolume;
            case 3: return SfxVolume;
            default: return 0.5f;
        }
    }
    public void SetVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        AmbientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.5f);

        PlayerWalk.volume = MasterVolume * SfxVolume;

        foreach (GoblinMonster goblin in Goblins)
        {
            goblin.UpdateVolume();
        }
        foreach (RhinoMonster rhino in Rhinos)
        {
            rhino.UpdateVolume(MasterVolume*AmbientVolume, MasterVolume*SfxVolume);
        }
        // for (int i = 0; i < RhinoSound.Count; i++)
        // {
        //     if (i % 2 == 0)
        //     {
        //         RhinoSound[i].volume = MasterVolume * AmbientVolume; //ambient
        //     }
        //     else
        //     {
        //         RhinoSound[i].volume = MasterVolume * SfxVolume; //sfx
        //     }
        // }
        // for (int i = 0; i < GoblinSound.Count; i++)
        // {
        //     if (i % 2 == 0)
        //     {
        //         GoblinSound[i].volume = MasterVolume * AmbientVolume; //ambient
        //     }
        //     else
        //     {
        //         GoblinSound[i].volume = MasterVolume * SfxVolume; //sfx
        //     }
        // }
    }
    public void PauseAndContinueSound(bool pause, int soundType)
    {
        List<AudioSource> sources = soundType switch
        {
            1 => MusicSounds,
            2 => AmbientSounds,
            3 => SoundEffects,
            _ => null
        };
        
        if (sources != null)
        {
            foreach (var audioSource in sources)
            {
                if (pause)
                    audioSource.Pause();
                else
                    audioSource.UnPause();
            }
        }
    }
    public AudioClip GetClipFromLibrary(SoundID id)
    {
        return soundLibraryDict.TryGetValue(id, out var clip) ? clip : null;
    }
    private void InitializeSoundLibraries()
    {
        foreach (var sound in Sounds)
        {
            soundLibraryDict[sound.ID] = sound.Clip;
        }
    }
}

//  MUSIC 
// id 0 музыка главного меню
// id 1 музыка окружения для обычной комнаты
// id 2 музыка для босса
// id 3 музыка окружения для темной комнаты

//  AMBIENT
// id 4 звук ходьбы носорога - rhino_walk
// id 5 звук скручивания носорога - roller_curl
// id 6 звук удара носорога об стену - roller_hit_wall
// id 7 звук разгона носорога (до удара) - roller_rolling
// id 8 звук ходьбы гоблина - goblin_walk
// id 9 диалог нпс - Nailmmaster?
// id 10 раскрытие скрытной зоны (секрет найден, музыкальный акцент) - Secret

//  SFX 
// id 11 ходьба (игрока) - player_walk
// id 12 удар игрока - sword
// id 13 урон игрока - hero_damage
// id 14 появление босса - catboss_appear
// id 15 босс прячеться - catboss_hide
// id 16 прыжок игрока - hero_jump
// id 17 приземление игрока - hero_land
// id 18 подъем кристалла (улучшение) - dash_pickUp
// id 19 подъем меча (улучшение) - sword_pickUp
// id 20 подъем прыжков от стен (улучшение) - wallJump_pickUp
// id 21 улучшение меча (звук наковальни) - swordUpgrade
// id 22 активировать Обелиск (сохранение) - invoke
// id 23 получение урона носорогом - rhino_damage
// id 24 получение урона гоблином - goblin_damage 
// id 25 разрушение мешков с деньгами - MoneyBag
// id 26 удар монет о землю - coin_hit_ground - не уверен, что буду делать
// id 27 подбор монет - coin_collect
// id 28 dash - dash
// id 29 открытие ворот - gate_open
// id 30 закытие ворот - gate_slam
// id 31 звуки атаки босса сверху - boss_top_attack
// id 32 звуки атаки босса щупальцы (высунуть и спрятать) - tentacle
// id 33 наведение на кнопку UI - ui_change_selection
// id 34 нажатие на кнопку UI - ui_button_confirm