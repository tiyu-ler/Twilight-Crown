using UnityEngine;

public class DirectionalMusicTrigger : MonoBehaviour
{
    private AudioClip DefaultRoomMusic, DarkRoomMusic, BossMusic;

    private Vector2 lastPosition;

    void Start()
    {
        DefaultRoomMusic = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.DefaultRoomMusic);
        DarkRoomMusic = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.DarkRoomMusic);
        BossMusic = SoundManager.Instance.GetClipFromLibrary(SoundManager.SoundID.BossMusic);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            lastPosition = other.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 exitPosition = other.transform.position;
            Vector2 direction = exitPosition - lastPosition;

            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
            {
                if (direction.y > 0)
                    MusicManager.Instance.PlayLoop(DefaultRoomMusic, MusicManager.MusicType.DefaultRoom); // up

                else
                    MusicManager.Instance.PlayLoop(DarkRoomMusic, MusicManager.MusicType.DarkRoom); // down
            }
        }
    }
}
