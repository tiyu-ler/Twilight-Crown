using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    private HashSet<Room> activeRooms = new HashSet<Room>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterRoom(Room room)
    {
        activeRooms.Add(room);
        room.gameObject.SetActive(true);
    }

    public void UnregisterRoom(Room room)
    {
        activeRooms.Remove(room);
        room.gameObject.SetActive(false);
    }
}

// WITH PRELOAD SYSTEM
    // [SerializeField] private Room defaultRoom;
    //     [SerializeField] private int preloadDistance = 1;

    //     private void Awake()
    //     {
    //         if (Instance == null) Instance = this;

    //         if (defaultRoom != null)
    //         {
    //             RegisterRoom(defaultRoom);
    //             PreloadAdjacentRooms(defaultRoom);
    //         }
    //     }

    //     public void RegisterRoom(Room room)
    //     {
    //         if (!activeRooms.Contains(room))
    //         {
    //             activeRooms.Add(room);
    //             room.gameObject.SetActive(true);
    //             PreloadAdjacentRooms(room);
    //         }
    //     }

    //     public void UnregisterRoom(Room room)
    //     {
    //         if (activeRooms.Contains(room))
    //         {
    //             activeRooms.Remove(room);
    //             room.gameObject.SetActive(false);
    //         }
    //     }

    //     private void PreloadAdjacentRooms(Room room)
    //     {
    //         foreach (Room neighbor in room.neighborRooms)
    //         {
    //             if (!activeRooms.Contains(neighbor))
    //             {
    //                 neighbor.gameObject.SetActive(true);
    //                 activeRooms.Add(neighbor);
    //             }
    //         }
    //     }
