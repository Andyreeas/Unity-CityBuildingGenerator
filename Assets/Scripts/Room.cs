using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    public Vector2 roomPosition { get; private set; }

    public Room(Vector2 position) {
        roomPosition = position;
    }

}
