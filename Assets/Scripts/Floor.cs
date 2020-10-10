using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor {

    public int amountFloors { get; private set; }
    public Room[,] rooms;

    public Floor(int floor, Room[,] room) {
        amountFloors = floor;
        rooms = room;
    }

}
