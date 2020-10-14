using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall {

    public enum WallType {
        SimpleWall,
        SimpleDoor,
        SimpleWindowBig
    }

    public enum WallOrientation {
        North,
        East,
        South,
        West
    }

    public Vector3 WallPosition { get; private set; }
    public Quaternion WallRotation { get; private set; }
    public WallType TypeOfWall { get; private set; }
    public WallOrientation OrientationOfWall { get; private set; }

    public bool active { get; private set; } = true;

    public Wall(Vector3 position, Quaternion rotation, WallOrientation Orientation, WallType type = WallType.SimpleWall) {
        WallPosition = position;
        WallRotation = rotation;
        TypeOfWall = type;
        OrientationOfWall = Orientation;

        TypeOfWall = WallType.SimpleWall;
    }

    public void SetTypeOfWall(WallType type) {
        TypeOfWall = type;
    }

    public void SetOrientationOfWall(WallOrientation orientation) {
        OrientationOfWall = orientation;
    }

    public void SetWallActive(bool state) {
        active = state;
    }

}
