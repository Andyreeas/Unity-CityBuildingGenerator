using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    public Vector3 RoomPosition { get; private set; }

    public bool state = true;

    public Wall[] walls;
    public int floorLevel;

    public bool HasRoof { get; private set; }

    public Room(Vector3 position, int floor, bool roof) {
        RoomPosition = position;
        floorLevel = floor;
        HasRoof = roof;
    }

}
