using UnityEngine;

public class Room : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomManager.Instance.RegisterRoom(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomManager.Instance.UnregisterRoom(this);
        }
    }
}

// WITH PRELOAD SYSTEM
    // public List<Room> neighborRooms;

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         RoomManager.Instance.RegisterRoom(this);
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         RoomManager.Instance.UnregisterRoom(this);
    //     }
    // }
