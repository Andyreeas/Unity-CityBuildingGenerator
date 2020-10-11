using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor {

    public int FloorLevel { get; private set; }

    public Room[,] rooms;

    public Floor(int floor, Room[,] room) {
        FloorLevel = floor;
        rooms = room;
    }

}
