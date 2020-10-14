using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

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
    //x
    public float width;
    //y
    public float height;
    //z
    public float length;

    bool getsRoof = false;


    Floor[] floors;
    Room[,] rooms;

    public void GenerateBuilding() {
        Generator();
        RemoveInsideWalls();
        Renderer();
        SetBuildingSize();
    }

    public void generateRandomizedBuilding() {
        
        building.rows = Random.Range(1, 10);
        building.columns = Random.Range(1, 10);
        building.amountFloors = Random.Range(1, 6);
        building.windowsPercentChance = Random.Range(0.3f, 1.0f);
        building.doorPercentChance = Random.Range(0.1f, 1.0f);

        building.hasWall = true;
        building.hasFloor = true;
        building.hasRoof = true;
        building.hasInsideWalls = false;

        width = 2f;
        height = 1f;
        length = 2f;

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
                    rooms[i, j] = new Room(new Vector3(i * width, floorCount, j * length), floorCount, getsRoof);
                    Room room = rooms[i, j];

                    room.walls = new Wall[4];

                    room.walls[0] = new Wall(new Vector3(room.RoomPosition.x + width / 2 + wallPrefab.localScale.z / 2, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z), Quaternion.Euler(0, 90, 0), Wall.WallOrientation.East);
                    room.walls[1] = new Wall(new Vector3(room.RoomPosition.x - width / 2 - wallPrefab.localScale.z / 2, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z), Quaternion.Euler(0, -90, 0), Wall.WallOrientation.West);
                    room.walls[2] = new Wall(new Vector3(room.RoomPosition.x, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z + length / 2 + wallPrefab.localScale.z / 2), Quaternion.Euler(0, 0, 0), Wall.WallOrientation.North);
                    room.walls[3] = new Wall(new Vector3(room.RoomPosition.x, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z - length / 2 - wallPrefab.localScale.z / 2), Quaternion.Euler(0, 180, 0), Wall.WallOrientation.South);
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
            floorHolder.position = new Vector3(building.rows * width / 2f - (width / 2), floor.FloorLevel * height, building.columns * length / 2f - (length / 2));

            for (int i = 0; i < building.rows; i++) {
                for (int j = 0; j < building.columns; j++) {
                    //Create parent object for the room and a room Name
                    string roomName = "Room-" + i + j;
                    Transform roomHolder = new GameObject(roomName).transform;
                    roomHolder.parent = floorHolder;

                    Room room = floor.rooms[i, j];

                    roomHolder.position = new Vector3(room.RoomPosition.x, floor.FloorLevel * height + floorPrefab.localScale.y / 2f, room.RoomPosition.z);

                    PlaceFloor(room, floor, roomHolder);
                    PlaceWalls(room, floor, roomHolder);
                    PlaceRoof(room, floor, roomHolder);
                }
            }
        }
    }

    void PlaceFloor(Room room, Floor floor, Transform roomHolder) {
        if (building.hasFloor) {
            Vector3 groundPosition = new Vector3(room.RoomPosition.x, floor.FloorLevel * height + floorPrefab.localScale.y / 2f, room.RoomPosition.z);
            Transform ground = Instantiate(floorPrefab, groundPosition, Quaternion.Euler(0, 0, 0));
            ground.localScale = new Vector3(width, floorPrefab.localScale.y, length);
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
            wall.localScale = new Vector3(wall.localScale.x * length + wallPrefab.localScale.z * 2, wall.localScale.y * height, wallPrefab.localScale.z);
        } else if (index == 2 || index == 3) {
            wall.localScale = new Vector3(wall.localScale.x * width, wall.localScale.y * height, wallPrefab.localScale.z);
        }
    }

    void PlaceRoof(Room room, Floor floor, Transform roomHolder) {
        if (building.hasRoof && room.HasRoof) {
            Vector3 roofPosition = new Vector3(room.RoomPosition.x, wallPrefab.localScale.y * height * (floor.FloorLevel + 1) + roofPrefab.localScale.y / 2, room.RoomPosition.z);
            Transform roof = Instantiate(roofPrefab, roofPosition, Quaternion.Euler(0, 0, 0));
            roof.localScale = new Vector3(roof.localScale.x * width + wallPrefab.localScale.z * 2, roof.localScale.y, roof.localScale.z * length + wallPrefab.localScale.z * 2);
            roof.parent = roomHolder;
        }
    }

    void SetBuildingSize() {
        building.buildingSizex = building.rows * width;
        building.buildingSizey = building.columns * length;
    }

    void RemoveInsideWalls() {
        foreach (Floor floor in floors) {
            if (!building.hasInsideWalls && !(building.rows == 1) && !(building.columns == 1)) {
                for (int i = 0; i < building.rows; i++) {
                    for (int j = 0; j < building.columns; j++) {
                        if(i == 0) {
                            if (j == 0) {
                                SetWallInactive(floor, i, j, 0, 2);
                            } else if (j > 0) {
                                if (j < building.columns - 1) {
                                    SetWallInactive(floor, i, j, 0, 2, 3);
                                } else if (j == building.columns - 1) {
                                    SetWallInactive(floor, i, j, 0, 3);
                                }
                            }
                        } else if (i > 0 && i < building.rows - 1) {
                            if (j == 0) {
                                if (i < building.rows - 1) {
                                    SetWallInactive(floor, i, j, 0, 1, 2);
                                } else if (i == building.rows - 1) {
                                    SetWallInactive(floor, i, j, 1, 2);
                                }
                            } else if (j > 0) {
                                if (i < building.rows - 1 && j < building.columns - 1) {
                                    SetWallInactive(floor, i, j, 0, 1, 2, 3);
                                } else if (j == building.columns - 1 && i < building.rows - 1) {
                                    SetWallInactive(floor, i, j, 0, 1, 3);
                                }
                            }
                        } else if (i == building.rows - 1) {
                            if(j == 0) {
                                SetWallInactive(floor, i, j, 1, 2);
                            }
                            if (j > 0) {
                                if (j < building.columns - 1) {
                                    SetWallInactive(floor, i, j, 1, 2, 3);
                                } else if (j == building.columns - 1) {
                                    SetWallInactive(floor, i, j, 1, 3);
                                }
                            }
                        }
                    }
                }
            } else if (!building.hasInsideWalls && building.rows == 1 && building.columns > 1) {
                for (int j = 0; j < building.columns; j++) {
                    if (j < building.columns - 1) {
                        SetWallInactive(floor, 0, j, 2);
                        if (j > 0) {
                            SetWallInactive(floor, 0, j, 3);
                        }
                    }
                    if (j == building.columns - 1) {
                        SetWallInactive(floor, 0, j, 3);
                    }
                }
            } else if (!building.hasInsideWalls && building.rows > 1 && building.columns == 1) {
                for (int i = 0; i < building.rows; i++) {
                    if (i < building.rows - 1) {
                        SetWallInactive(floor, i, 0, 0);
                        if (i > 0) {
                            SetWallInactive(floor, i, 0, 1);
                        }
                    }
                    if (i == building.rows - 1) {
                        SetWallInactive(floor, i, 0, 1);
                    }
                }
            }
        }
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
                floor.rooms[i, j].walls[0].SetWallActive(false);
                break;
            case 1:
                floor.rooms[i, j].walls[1].SetWallActive(false);
                break;
            case 2:
                floor.rooms[i, j].walls[2].SetWallActive(false);
                break;
            case 3:
                floor.rooms[i, j].walls[3].SetWallActive(false);
                break;
        }
    }
}