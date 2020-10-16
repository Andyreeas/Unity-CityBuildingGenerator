using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingGenerator : MonoBehaviour {

    //Prefabs
    public Transform wallPrefab;
    public Transform doorPrefab;
    public Transform windowPrefab;
    public Transform floorPrefab;
    public Transform roofPrefab;

    //Buildings settings
    [Header("Building settings")]
    [SerializeField]
    public Building building;

    [System.Serializable]
    public class Building {
        [Range(1, 20)]
        public int rows;
        [Range(1, 20)]
        public int columns;

        public int amountFloors;

        [Range(0, 1)]
        public float windowsPercentChance;
        [Range(0, 1)]
        public float doorPercentChance;

        public bool hasWall;
        public bool hasFloor;
        public bool hasRoof;

        public bool hasInsideWalls;

        [HideInInspector]
        public float buildingSizex;
        [HideInInspector]
        public float buildingSizey;
    }

    [Header("Room size")]
    float scale = 2f;

    bool getsRoof = false;

    Floor[] floors;
    Room[,] rooms;

    public void GenerateBuilding() {
        Generator();
        RemoveInsideWalls();
        Renderer();
        SetBuildingSize();
    }

    public void GenerateRandomizedBuilding() {
        building.rows = Random.Range(1, 10);
        building.columns = Random.Range(1, 10);
        building.amountFloors = Random.Range(1, 6);
        building.windowsPercentChance = Random.Range(0.3f, 1.0f);
        building.doorPercentChance = Random.Range(0.1f, 1.0f);

        building.hasWall = true;
        building.hasFloor = true;
        building.hasRoof = true;
        building.hasInsideWalls = false;

        GenerateBuilding();
    }

    public void GenerateRandomizedBuilding( int buildingRow, 
                                            int buildingColumns, 
                                            int amountFloors, 
                                            float windowsPercentChance,
                                            float doorPercentChance,
                                            bool hasWall,
                                            bool hasFloor,
                                            bool hasRoof,
                                            bool hasInsideWall,
                                            float scale
                                            ) {

        building.rows = buildingRow;
        building.columns = buildingColumns;
        building.amountFloors = amountFloors;
        building.windowsPercentChance = windowsPercentChance;
        building.doorPercentChance = doorPercentChance;

        building.hasWall = hasWall;
        building.hasFloor = hasFloor;
        building.hasRoof = hasRoof;
        building.hasInsideWalls = hasInsideWall;

        this.scale = scale;

        GenerateBuilding();

    }

    //Create Logic for a Building
    void Generator() {
        floors = new Floor[building.amountFloors];
        int floorCount = 0;

        foreach (Floor floor in floors) {
            rooms = new Room[building.rows, building.columns];
            for (int i = 0; i < building.rows; i++) {
                for (int j = 0; j < building.columns; j++) {
                    if (floorCount == building.amountFloors - 1) {
                        getsRoof = true;
                    } else {
                        getsRoof = false;
                    }
                    rooms[i, j] = new Room(new Vector3(i * scale, floorCount, j * scale), floorCount, getsRoof);
                    Room room = rooms[i, j];

                    room.walls = new Wall[4];

                    room.walls[0] = new Wall(new Vector3(room.RoomPosition.x + scale / 2 + wallPrefab.localScale.z / 2, floorCount * scale / 2f + wallPrefab.localScale.y * scale / 2f / 2, room.RoomPosition.z), Quaternion.Euler(0, 90, 0), Wall.WallOrientation.East);
                    room.walls[1] = new Wall(new Vector3(room.RoomPosition.x - scale / 2 - wallPrefab.localScale.z / 2, floorCount * scale / 2f + wallPrefab.localScale.y * scale / 2f / 2, room.RoomPosition.z), Quaternion.Euler(0, -90, 0), Wall.WallOrientation.West);
                    room.walls[2] = new Wall(new Vector3(room.RoomPosition.x, floorCount * scale / 2f + wallPrefab.localScale.y * scale / 2f / 2, room.RoomPosition.z + scale / 2 + wallPrefab.localScale.z / 2), Quaternion.Euler(0, 0, 0), Wall.WallOrientation.North);
                    room.walls[3] = new Wall(new Vector3(room.RoomPosition.x, floorCount * scale / 2f + wallPrefab.localScale.y * scale / 2f / 2, room.RoomPosition.z - scale / 2 - wallPrefab.localScale.z / 2), Quaternion.Euler(0, 180, 0), Wall.WallOrientation.South);
                }
            }
            floors[floorCount] = new Floor(floorCount++, rooms);
        }
    }

    /*
     * Creates the building with the logic from the generator
     * 
     */
    void Renderer() {
        //Parent object if already exists it gets destroyed
        string buildingHolderName = "Building";
        if (GameObject.Find(buildingHolderName)) {
            DestroyImmediate(GameObject.Find(buildingHolderName));
        }
        Transform buildingHolder = new GameObject(buildingHolderName).transform;

        foreach(Floor floor in floors) {
            string floorName = "Floor" + floor.FloorLevel;
            Transform floorHolder = new GameObject(floorName).transform;
            floorHolder.parent = buildingHolder;
            floorHolder.position = new Vector3(building.rows * scale / 2f - (scale / 2), floor.FloorLevel * scale / 2f, building.columns * scale / 2f - (scale / 2));

            for (int i = 0; i < building.rows; i++) {
                for (int j = 0; j < building.columns; j++) {
                    //Create parent object for the room and a room Name
                    string roomName = "Room-" + i + j;
                    Transform roomHolder = new GameObject(roomName).transform;
                    roomHolder.parent = floorHolder;

                    Room room = floor.rooms[i, j];

                    roomHolder.position = new Vector3(room.RoomPosition.x, floor.FloorLevel * scale / 2f + floorPrefab.localScale.y / 2f, room.RoomPosition.z);

                    PlaceFloor(room, floor, roomHolder);
                    PlaceWalls(room, floor, roomHolder);
                    PlaceRoof(room, floor, roomHolder);
                }
            }
        }
    }

    void PlaceFloor(Room room, Floor floor, Transform roomHolder) {
        if (building.hasFloor) {
            Vector3 groundPosition = new Vector3(room.RoomPosition.x, floor.FloorLevel * scale / 2f + floorPrefab.localScale.y / 2f, room.RoomPosition.z);
            Transform ground = Instantiate(floorPrefab, groundPosition, Quaternion.Euler(0, 0, 0));
            ground.localScale = new Vector3(scale, floorPrefab.localScale.y, scale);
            ground.parent = roomHolder;
        }
    }

    void PlaceWalls(Room room, Floor floor, Transform roomHolder) {
        if (building.hasWall) {
            for (int i = 0; i < room.walls.Length; i++) {
                if (room.floorLevel == 0 && room.walls[i].active) {
                    if (Random.Range(0f, 1f) <= building.doorPercentChance) {
                        InstantiateWall(room, doorPrefab, roomHolder, i);
                        // Setting the WallType of the wall to SimpleDoor it is SimpleWall by default
                        room.walls[i].SetTypeOfWall(Wall.WallType.SimpleDoor);
                    } else {
                        InstantiateWall(room, wallPrefab, roomHolder, i);
                    }
                } else if (room.floorLevel >= 1 && room.walls[i].active) {
                    if (Random.Range(0f, 1f) <= building.windowsPercentChance) {
                        InstantiateWall(room, windowPrefab, roomHolder, i);
                        // Setting the WallType of the wall to SimpleDoor it is SimpleWall by default
                        room.walls[i].SetTypeOfWall(Wall.WallType.SimpleWindowBig);
                    } else {
                        InstantiateWall(room, wallPrefab, roomHolder, i);
                    }
                }
            }
        }
    }

    void InstantiateWall(Room room, Transform prefab, Transform roomHolder, int index) {
        Transform wall = Instantiate(prefab, room.walls[index].WallPosition, room.walls[index].WallRotation, roomHolder);
        SetlocalSacleWall(wall, index);
    }

    void SetlocalSacleWall(Transform wall, int index) {
        if (index == 0 || index == 1) {
            wall.localScale = new Vector3(wall.localScale.x * scale + wallPrefab.localScale.z * 2, wall.localScale.y * scale / 2f, wallPrefab.localScale.z);
        } else if (index == 2 || index == 3) {
            wall.localScale = new Vector3(wall.localScale.x * scale, wall.localScale.y * scale / 2f, wallPrefab.localScale.z);
        }
    }

    void PlaceRoof(Room room, Floor floor, Transform roomHolder) {
        if (building.hasRoof && room.HasRoof) {
            Vector3 roofPosition = new Vector3(room.RoomPosition.x, wallPrefab.localScale.y * scale / 2f * (floor.FloorLevel + 1) + roofPrefab.localScale.y / 2, room.RoomPosition.z);
            Transform roof = Instantiate(roofPrefab, roofPosition, Quaternion.Euler(0, 0, 0));
            roof.localScale = new Vector3(roof.localScale.x * scale + wallPrefab.localScale.z * 2, roof.localScale.y, roof.localScale.z * scale + wallPrefab.localScale.z * 2);
            roof.parent = roomHolder;
        }
    }

    void SetBuildingSize() {
        building.buildingSizex = building.rows * scale;
        building.buildingSizey = building.columns * scale;
    }

    void RemoveInsideWalls() {
        if (!building.hasInsideWalls) {
            foreach (Floor floor in floors) {
                for (int i = 0; i < building.rows; i++) {
                    for (int j = 0; j < building.columns; j++) {
                        if (floor.rooms[i, j].state) {
                            int type = GetBinaryCode(floor, i, j);

                            switch (type) {
                                case 0:
                                    SetWallInactive(floor, i, j, 0, 1, 2, 3);
                                    break;
                                case 1:
                                    SetWallInactive(floor, i, j, 0, 1, 2);
                                    break;
                                case 2:
                                    SetWallInactive(floor, i, j, 1, 2, 3);
                                    break;
                                case 3:
                                    SetWallInactive(floor, i, j, 1, 2);
                                    break;
                                case 4:
                                    SetWallInactive(floor, i, j, 0, 1, 3);
                                    break;
                                case 5:
                                    SetWallInactive(floor, i, j, 0, 1);
                                    break;
                                case 6:
                                    SetWallInactive(floor, i, j, 1, 3);
                                    break;
                                case 7:
                                    SetWallInactive(floor, i, j, 1);
                                    break;
                                case 8:
                                    SetWallInactive(floor, i, j, 0, 2, 3);
                                    break;
                                case 9:
                                    SetWallInactive(floor, i, j, 0, 2);
                                    break;
                                case 10:
                                    SetWallInactive(floor, i, j, 2, 3);
                                    break;
                                case 11:
                                    SetWallInactive(floor, i, j, 2);
                                    break;
                                case 12:
                                    SetWallInactive(floor, i, j, 0, 3);
                                    break;
                                case 13:
                                    SetWallInactive(floor, i, j, 0);
                                    break;
                                case 14:
                                    SetWallInactive(floor, i, j, 3);
                                    break;
                                case 15:
                                    // No wall needs to be set inactive
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    int GetBinaryCode(Floor floor, int x, int y) {
        int n1 = 0;
        int n2 = 0;
        int n3 = 0;
        int n4 = 0;

        try {
            if (y - 1 < 0 || floor.rooms[x, y - 1].state == false) {   // 1
                n1 = 1;
            }
        } catch (IndexOutOfRangeException) {
            n1 = 1;
        }
        try {
            if (x + 1 > floor.rooms.GetLength(0) || floor.rooms[x + 1, y].state == false) {   // 2
                n2 = 2;
            }
        } catch (IndexOutOfRangeException) {
            n2 = 2;
        }
        try {
            if (y + 1 > floor.rooms.GetLength(1) || floor.rooms[x, y + 1].state == false) {   // 4
                n3 = 4;
            }
        } catch (IndexOutOfRangeException) {
            n3 = 4;
        }
        try {
            if (x - 1 < 0 || floor.rooms[x - 1, y].state == false) {   // 8
                n4 = 8;
            }
        } catch (IndexOutOfRangeException) {
            n4 = 8;
        }

        int type = n1 + n2 + n3 + n4;
        return type;
    }

    void SetWallInactive(Floor floor, int i, int j, int index01, int index02 = -1, int index03 = -1, int index04 = -1) {
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(index01);
        queue.Enqueue(index02);
        queue.Enqueue(index03);
        queue.Enqueue(index04);
        for (int h = 0; h < 4; h++) {
            ChoseWall(floor, i, j, queue.Dequeue());
        }
    }

    void ChoseWall(Floor floor, int i, int j, int index) {
        switch (index) {
            case -1:
                break;
            case 0:
                // East wall is set inactive
                floor.rooms[i, j].walls[0].SetWallActive(false);
                break;
            case 1:
                // West wall is set inactive
                floor.rooms[i, j].walls[1].SetWallActive(false);
                break;
            case 2:
                // North wall is set inactive
                floor.rooms[i, j].walls[2].SetWallActive(false);
                break;
            case 3:
                // Sout wall is set inactive
                floor.rooms[i, j].walls[3].SetWallActive(false);
                break;
        }
    }
}