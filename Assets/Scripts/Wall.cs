using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall {

    public enum WallType {
        SimpleWall,
        SimpleDoor,
        SimpleWindowSmall
    }

    public Vector3 WallPosition { get; private set; }
    public Quaternion WallRotation { get; private set; }

    public WallType TypeOfWall { get; private set; }

    public Wall(Vector3 position, Quaternion rotation, WallType type = WallType.SimpleWall) {
        WallPosition = position;
        WallRotation = rotation;
        TypeOfWall = type;
    }

    public void SetWallType() {

    }

}
