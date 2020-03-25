﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePrefabs : MonoBehaviour {


    public MazeGlobals MazeGlobals;
    private int x;
    private int z;

    // Map Prefabs
    [Header("Corridors", order=1)]
    public GameObject CorridorHall;
    public GameObject CorridorHallExterior;
    public GameObject CorridorEnd;
    public GameObject CorridorCorner;
    public GameObject CorridorTJunction;
    public GameObject CorridorIntersection;
    
    [Header("Rooms", order=2)]
    public GameObject Room2x3;
    public GameObject Room2x2;

    [Header("Rooms", order=3)]
    public GameObject RoomWall;
    public GameObject RoomWallAndFloor2x4;
    public GameObject RoomWallAndFloorCorner;
    public GameObject RoomWall2x1;
    public GameObject RoomLongWall;
    public GameObject RoomLongWall2;
    
    [Header("Doorways", order=4)]
    public GameObject Doorway;
    public GameObject DoorwayCorridorToRoom;
    public GameObject DoorwayCorridorToRoomFlip;

    public GameObject DoorwayRoomToRoom;
    public GameObject DoorwayRoomToRoomSlant;

    [Header("Misc.", order=5)]
    public float prefabOffsetX = 0.5f;
    public float prefabOffsetZ = 0.5f;


    void Awake(){
        MazeGlobals = gameObject.GetComponent<MazeGlobals>();
    }

    public void Corridors(){
        int gridX = MazeGlobals.gridX;
        int gridZ = MazeGlobals.gridZ;
        int mapScale = MazeGlobals.mapScale;
        // int prefabOffsetX = MazeGlobals.prefabOffsetX;
        // int prefabOffsetZ = MazeGlobals.prefabOffsetZ;

        // List all candidates for start and end points

        // Reset the prefab list
        MazeGlobals.prefabList = new List<List<GameObject>>();

        for(int X = 0; X < gridX; X++){
            
            List<GameObject> prefabRow = new List<GameObject>();

            for(int Z = 0; Z < gridZ; Z++){
                
                int findCount = 0;

                for (int i = 0; i < 4; i++){
                    if (MazeGlobals.cellData[X][Z][i]==0) findCount++;
                }

                GameObject prefabCell;
                if (MazeGlobals.showPrefabMaze){
                    switch(findCount) {
                         case (1):
                            if (MazeGlobals.cellData[X][Z][0] == 0 ){
                                prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,180,0));
                            } else if (MazeGlobals.cellData[X][Z][1] == 0){
                                prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,270,0));
                            } else if (MazeGlobals.cellData[X][Z][2] == 0){
                                prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                            } else {
                                prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                            }
                            prefabRow.Add(prefabCell);
                            break;
                        case (2):
                            // Hallways
                            if (MazeGlobals.cellData[X][Z][0] == 0  && MazeGlobals.cellData[X][Z][2] == 0 ){
                                if (X==0){
                                    prefabCell = Instantiate(CorridorHallExterior, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                                }else if (X+1==gridX){
                                    prefabCell = Instantiate(CorridorHallExterior, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,180,0));
                                }else{
                                    prefabCell = Instantiate(CorridorHall, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                                }

                            } else if (MazeGlobals.cellData[X][Z][1] == 0  && MazeGlobals.cellData[X][Z][3] == 0 ){
                                prefabCell = Instantiate(CorridorHall, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));

                            // Corners
                            } else if (MazeGlobals.cellData[X][Z][0] == 0  && MazeGlobals.cellData[X][Z][1] == 0 ){
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,270,0));
                            } else if (MazeGlobals.cellData[X][Z][1] == 0  && MazeGlobals.cellData[X][Z][2] == 0 ){
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                            } else if (MazeGlobals.cellData[X][Z][2] == 0  && MazeGlobals.cellData[X][Z][3] == 0 ){
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                            } else{
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,180,0));
                            }
                            prefabRow.Add(prefabCell);
                            break;
                        case (3):
                            // T Junction
                            if (MazeGlobals.cellData[X][Z][0] == 1){
                                prefabCell = Instantiate(CorridorTJunction, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                            } else if (MazeGlobals.cellData[X][Z][1] == 1){
                                prefabCell = Instantiate(CorridorTJunction, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,180,0));
                            } else if (MazeGlobals.cellData[X][Z][2] == 1){
                                prefabCell = Instantiate(CorridorTJunction, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,270,0));
                            } else{
                                prefabCell = Instantiate(CorridorTJunction, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                            }
                            prefabRow.Add(prefabCell);
                            break;
                        case (4):
                            prefabCell = Instantiate(CorridorIntersection, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                            prefabRow.Add(prefabCell);
                            break;
                    
                    }
                    prefabRow[prefabRow.Count-1].tag = "Occludable"; // Tag as prefab
                    prefabRow[prefabRow.Count-1].AddComponent<ObjectHider>();
                    prefabRow[prefabRow.Count-1].transform.parent = MazeGlobals.prefabMazeParent.transform; // Group objects
                    prefabRow[prefabRow.Count-1].name = X+","+Z; // Rename with co-ordinates
                }
            }
            MazeGlobals.prefabList.Add(prefabRow);
        }
        // SetSpawnPoints();
    }



  
    public void Rooms(){
        int mapScale = MazeGlobals.mapScale;
        int gridX = MazeGlobals.gridX;
        int gridZ = MazeGlobals.gridZ;

        List<List<int>> roomCells1 = new List<List<int>>();
        List<List<List<int>>> rooms = new List<List<List<int>>>();
        roomCells1.Add(new List<int>{0,0});
        roomCells1.Add(new List<int>{0,1});
        roomCells1.Add(new List<int>{0,2});
        roomCells1.Add(new List<int>{1,0});
        roomCells1.Add(new List<int>{1,1});
        roomCells1.Add(new List<int>{1,2});
        rooms.Add(roomCells1);

        List<List<int>> roomCells2 = new List<List<int>>();
        roomCells2.Add(new List<int>{0,0});
        roomCells2.Add(new List<int>{0,1});
        roomCells2.Add(new List<int>{1,1});
        roomCells2.Add(new List<int>{1,0});
        rooms.Add(roomCells2);

        int counter = 0;
        int roomUID = 0;

        // Iterate over elements in optimal path
        foreach(List<int> pathElement in MazeGlobals.optimalPath){
            int prefabCounter = 0;
            counter++;

            int X = pathElement[0];
            int Z = pathElement[1];
            foreach(List<List<int>> room in rooms){
                prefabCounter++;

                int cellCount = room.Count;
                int cellX = 0;
                int cellZ = 0;

                foreach(List<int> cell in room){
                    cellX = X+cell[0];
                    cellZ = Z+cell[1];


                    // If cell is in grid && cell is on a path && cell is not room
                    if (cellX<gridX && cellZ<gridZ && MazeGlobals.cellData[cellX][cellZ][5]==1 && MazeGlobals.cellData[cellX][cellZ][6]==0){
                        cellCount--;
                    }else{
                        break;
                    }
                }

                
                if (cellCount==0){
                    // Set global X & Z
                    x=X;
                    z=Z;
                    GameObject prefabRoom;
                    // Place the room prefab
                    switch(prefabCounter) {
                        case (1):
                            prefabRoom = Instantiate(Room2x3, new Vector3((X+.4f)*mapScale, 0f, (Z+.4f)*mapScale), Quaternion.Euler(0,0,0));
                            prefabRoom.transform.parent = MazeGlobals.cellDoorParent.transform;
                        break;

                        case (2):
                            prefabRoom = Instantiate(Room2x2, new Vector3((X+.4f)*mapScale, 0f, (Z+.4f)*mapScale), Quaternion.Euler(0,0,0));
                            prefabRoom.transform.parent = MazeGlobals.cellDoorParent.transform;
                        break;
                    }
                    
                    /* Assign a Unique ID to each room cell so we can identify connections between adjacent rooms vs corridors
                            - Corridors are marked as ID 0
                            - Rooms are marked as ID 1 and up
                    */
                    roomUID++;
                    
                    // Remove walls in room cells
                    foreach(List<int> cell in room){
                        cellX = cell[0]+X;
                        cellZ = cell[1]+Z;
                        MazeGlobals.cellData[cellX][cellZ][7] = roomUID; // assign UID to cell in room

                        RoomCell(cellX,cellZ,room); // n
                    }
                }
            }
        }
        PlaceDoors();
    }






    public void RoomCell(int X, int Z, List<List<int>> room){

        // Disable walls in same room
        foreach(List<int> cell in room){

            int cellX = x+cell[0];
            int cellZ = z+cell[1];

            if (cellX == X && cellZ == Z+1){
                MazeGlobals.cellList[X][Z][0].SetActive(false);
                MazeGlobals.cellData[x][z][0]   = 0;
                MazeGlobals.cellData[x][z+1][2] = 0;

            }else if (cellX == X+1 && cellZ == Z){
                MazeGlobals.cellList[X][Z][1].SetActive(false);
                MazeGlobals.cellData[x][z][1]   = 0;
                MazeGlobals.cellData[x+1][z][3] = 0;

            }else if (cellX == X && cellZ == Z-1){
                MazeGlobals.cellList[X][Z-1][0].SetActive(false);
                MazeGlobals.cellData[X][Z][2]   = 0;
                MazeGlobals.cellData[X][Z-1][0] = 0;

            }else if (cellX == X-1 && cellZ == Z){
                MazeGlobals.cellList[X-1][Z][1].SetActive(false);
                MazeGlobals.cellData[X][Z][3]   = 0;
                MazeGlobals.cellData[X-1][Z][1] = 0;
            }
        }

        MazeGlobals.cellData[X][Z][6]=1;        // Mark cell as room

        // print("(X, Z) ("+X+", "+Z+")");
        Destroy(MazeGlobals.prefabList[X][Z]);
    }



    public void PlaceDoors(){

        int gridX = MazeGlobals.gridX;
        int gridZ = MazeGlobals.gridZ;

        int x=0;
        int z=0;

        int currentRoomUID = 0;

        // Iterate over all cells in maze
        for(int X = 0; X < gridX; X++){
            for(int Z = 0; Z < gridZ; Z++){
                bool northDoor = false;
                bool westDoor = false;

                currentRoomUID = MazeGlobals.cellData[X][Z][7]; // Get cell UID

                // If - Within room cell
                if (MazeGlobals.cellData[X][Z][7]!=0){


                    // WALLS
                    if (MazeGlobals.cellData[X][Z][3]==1){
                        if (Z-1>=0 && MazeGlobals.cellData[X][Z-1][7]!=currentRoomUID){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.4f), new Vector3(1,1,1), 0);
                        }else if (Z+1<gridZ && MazeGlobals.cellData[X][Z+1][7]!=currentRoomUID){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.2f), new Vector3(1,1,1), 0);
                        }else{
                            CreatePrefab(RoomLongWall2, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.2f), new Vector3(1,1,1), 0);
                        }
                    }


                    if (MazeGlobals.cellData[X][Z][0]==1){
                        if (X==0 || (X-1>=0 && MazeGlobals.cellData[X-1][Z][7]!=currentRoomUID)){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.6f), new Vector3(1,1,1), 90);
                        }else if (X+1==gridX || (X+1<gridX && MazeGlobals.cellData[X+1][Z][7]!=currentRoomUID)){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.2f, 0f, Z+.6f), new Vector3(1,1,1), 90);
                        }else{
                            CreatePrefab(RoomLongWall2, MazeGlobals.cellDoorParent, new Vector3(X+.2f, 0f, Z+.6f), new Vector3(1,1,1), 90);
                        }
                    }


                    if (MazeGlobals.cellData[X][Z][1]==1){
                        if (Z-1>=0 && MazeGlobals.cellData[X][Z-1][7]!=currentRoomUID){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.6f, 0f, Z+.8f), new Vector3(1,1,1), 180);
                        }else if (Z+1<gridZ && MazeGlobals.cellData[X][Z+1][7]!=currentRoomUID){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.6f, 0f, Z+.6f), new Vector3(1,1,1), 180);
                        }else{
                            CreatePrefab(RoomLongWall2, MazeGlobals.cellDoorParent, new Vector3(X+.6f, 0f, Z+.6f), new Vector3(1,1,1), 180);
                        }
                    }


                    if (MazeGlobals.cellData[X][Z][2]==1){
                        if (X-1>=0 && MazeGlobals.cellData[X-1][Z][7]!=currentRoomUID){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.8f, 0f, Z+.4f), new Vector3(1,1,1), 270);
                        }else if (X+1<gridX && MazeGlobals.cellData[X+1][Z][7]!=currentRoomUID){
                            CreatePrefab(RoomLongWall, MazeGlobals.cellDoorParent, new Vector3(X+.6f, 0f, Z+.4f), new Vector3(1,1,1), 270);
                        }else{
                            CreatePrefab(RoomLongWall2, MazeGlobals.cellDoorParent, new Vector3(X+.6f, 0f, Z+.4f), new Vector3(1,1,1), 270);
                        }
                    }



                    
                    // DOORS

                    // Check Left (WEST)
                    // If - Withing boundaries of map && If no wall to left of cell
                    if (X-1 >= 0 && MazeGlobals.cellData[X][Z][3]==0){ 

                        if (MazeGlobals.cellData[X-1][Z][7]==0){ // If corridor is north of cell
                            westDoor=true;
                            // If there is a wall to the bottom || different room
                            if (Z == 0 || MazeGlobals.cellData[X][Z-1][7]!=currentRoomUID)
                                CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.2f, 0f, Z+.5f), new Vector3(-1,1,1), -90);
                            else
                                CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.2f, 0f, Z+.5f), new Vector3(1,1,1), -90);

                        }else if (MazeGlobals.cellData[X-1][Z][7]!=currentRoomUID){ // else if - Another room is west of cell
                            westDoor=true;
                            // If there is a wall to the bottom || different room
                            if (Z == 0 || MazeGlobals.cellData[X][Z-1][7]!=currentRoomUID)
                                CreatePrefab(DoorwayRoomToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.4f), new Vector3(1,1,1), -90);
                            else
                                CreatePrefab(DoorwayRoomToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.6f), new Vector3(-1,1,1), -90);
                        }
                    }
                    


                    // Check Left (NORTH)
                    // If - Withing boundaries of map && If no wall to north of cell
                    if (Z+1 < gridZ && MazeGlobals.cellData[X][Z][0]==0 ){
                        

                        if (MazeGlobals.cellData[X][Z+1][7]==0){ // If corridor is north of cell
                            northDoor=true;

                            // If there is a wall to the left || different room
                            if (X+1 >= gridX || MazeGlobals.cellData[X+1][Z][7]!=currentRoomUID)
                                CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.5f, 0f, Z+.8f), new Vector3(1,1,1), 0);
                            else
                                CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.5f, 0f, Z+.8f), new Vector3(-1,1,1), 0);
                        
                        }else if (MazeGlobals.cellData[X][Z+1][7]!=currentRoomUID){ // else if - Another room is north of cell
                            northDoor=true;
                            
                            // If there is a wall to the left || different room
                            if (X+1 >= gridX || MazeGlobals.cellData[X+1][Z][7]!=currentRoomUID)
                                CreatePrefab(DoorwayRoomToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.6f, 0f, Z+.6f), new Vector3(-1,1,1), 0);
                            else
                                CreatePrefab(DoorwayRoomToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.6f), new Vector3(1,1,1), 0);
                        }
                    }

                }else{

                    // Check Left (NORTH)
                    // if within corridor && north cell is room 
                    if ( MazeGlobals.cellData[X][Z][0]==0 && Z+1 < gridX && MazeGlobals.cellData[X][Z+1][7]!=0){
                        // If there is a wall to the left of room cell || North room is different to North east room
                        if (X+1 >= gridX || MazeGlobals.cellData[X+1][Z+1][7]!=MazeGlobals.cellData[X][Z+1][7])
                            CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.5f, 0f, Z+1.2f), new Vector3(-1,1,1), 180);
                        else
                            CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.5f, 0f, Z+1.2f), new Vector3(1,1,1), 180);
                    }


                    // Check UP (WEST)
                    // if within corridor && west cell is room 
                    if ( MazeGlobals.cellData[X][Z][3]==0 && X-1>0 && MazeGlobals.cellData[X-1][Z][7]!=0){

                        if (Z+1 >= gridZ || MazeGlobals.cellData[X-1][Z+1][7]!=MazeGlobals.cellData[X-1][Z][7])
                            CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X-.2f, 0f, Z+.5f), new Vector3(-1,1,1), 90);
                        else
                            CreatePrefab(DoorwayCorridorToRoom, MazeGlobals.cellDoorParent, new Vector3(X-.2f, 0f, Z+.5f), new Vector3(1,1,1), 90);
                    }
                } 
            }
        }
    }
                        // CreatePrefab(RoomWallAndFloor2x4, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.4f), new Vector3(1,1,1), 0);

    public void CreatePrefab(GameObject prefab, GameObject parent, Vector3 pos, Vector3 scale, int rotation){
        int mapScale = MazeGlobals.mapScale;
        GameObject newPrefab;
        newPrefab = Instantiate(prefab, new Vector3(pos.x*mapScale, pos.y, pos.z*mapScale), Quaternion.Euler(0,rotation,0));
        newPrefab.transform.localScale = new Vector3(scale.x,scale.y,scale.z);
        newPrefab.transform.parent = parent.transform;
    }
}



                // if (MazeGlobals.cellData[X][Z][7]!=0){ 
                //     if (MazeGlobals.cellData[X][Z][0]==1){ // If North Wall

                //         // If EAST room or corridor
                //         if (MazeGlobals.cellData[X][Z][1]==1 || X==gridX || (MazeGlobals.cellData[X+1][Z][7]!=currentRoomUID && MazeGlobals.cellData[X+1][Z][7]!=0)){
                //             // Place LEFT - North wall / East Wall / Corner

                //             CreatePrefab(RoomWallAndFloor2x4, MazeGlobals.cellDoorParent, new Vector3(X+.4f, .3f, Z+.6f), new Vector3(1,1,1), 90);
                //             if (MazeGlobals.cellData[X+1][Z][7]!=MazeGlobals.cellData[X+1][Z-1][7]){
                //                 CreatePrefab(RoomWall2x1, MazeGlobals.cellDoorParent, new Vector3(X+.6f, .3f, Z+.6f), new Vector3(1,1,1), 180);

                //             }else if (MazeGlobals.cellData[X][Z][1]==1){
                //                 CreatePrefab(RoomWallAndFloor2x4, MazeGlobals.cellDoorParent, new Vector3(X+.6f, .3f, Z+.4f), new Vector3(1,1,1), 180);
                //                 CreatePrefab(RoomWallAndFloorCorner, MazeGlobals.cellDoorParent, new Vector3(X+.6f, .3f, Z+.6f), new Vector3(1,1,1), 180);
                //             }

                //         }else if (MazeGlobals.cellData[X][Z][3]==1 || X==0 || (MazeGlobals.cellData[X-1][Z][7]!=currentRoomUID && MazeGlobals.cellData[X-1][Z][7]!=0)) {
                //             // Place RIGHT - North wall / Corner
                //             CreatePrefab(RoomWallAndFloor2x4, MazeGlobals.cellDoorParent, new Vector3(X+.6f, .2f, Z+.6f), new Vector3(1,1,1), 90);
                //             CreatePrefab(RoomWallAndFloor2x4, MazeGlobals.cellDoorParent, new Vector3(X+.4f, .2f, Z+.4f), new Vector3(1,1,1), 0);
                //             CreatePrefab(RoomWallAndFloorCorner, MazeGlobals.cellDoorParent, new Vector3(X+.4f, .2f, Z+.6f), new Vector3(1,1,1), 90);

                //         }
                //         // else{ 
                //         //     // Place RIGHT - North wall / Corner
                //         //     CreatePrefab(RoomWallAndFloor2x4, MazeGlobals.cellDoorParent, new Vector3(X+.4f, .1f, Z+.6f), new Vector3(1,1,1), 90);
                //         //     CreatePrefab(RoomWallAndFloorCorner, MazeGlobals.cellDoorParent, new Vector3(X+1.0f, .1f, Z+.6f), new Vector3(1,1,1), 180);
                //         // }
                //     }

                //     // If NORTH/WEST room/corridor
                //     if (Z+1<gridZ){
                //         if (MazeGlobals.cellData[X][Z+1][7]!=currentRoomUID && MazeGlobals.cellData[X][Z][0]==0){
                //             if ((X>0 && MazeGlobals.cellData[X-1][Z][7]!=currentRoomUID) && MazeGlobals.cellData[X-1][Z][3]==0){
                //                 CreatePrefab(RoomWallAndFloorCorner, MazeGlobals.cellDoorParent, new Vector3(X+.4f, .22f, Z+.6f), new Vector3(1,1,1), 90);

                //             }
                //             if ((X+1<gridX && MazeGlobals.cellData[X+1][Z][7]!=currentRoomUID) && MazeGlobals.cellData[X+1][Z][1]==0){
                //                 CreatePrefab(RoomWallAndFloorCorner, MazeGlobals.cellDoorParent, new Vector3(X+.8f, .33f, Z+.6f), new Vector3(1,1,1), 180);
                //             }
                //         }
                //     }

























                            // if (Z==0 || MazeGlobals.cellData[X-1][Z-1][7]!=MazeGlobals.cellData[X-1][Z][7]){
                            // // If - connecting via bottom of a room
                            // if (Z==0 || MazeGlobals.cellData[X-1][Z-1][7]!=MazeGlobals.cellData[X-1][Z][7]){
                            //     // If there is a wall to the bottom || different room
                            //     if (Z == 0 || MazeGlobals.cellData[X][Z-1][7]!=currentRoomUID)
                            //         CreatePrefab(DoorwayRoomToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.4f), new Vector3(1,1,1), -90);
                            //     else
                            //         CreatePrefab(DoorwayRoomToRoom, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.2f), new Vector3(-1,1,1), -90);
                            // }else{
                            //     // // If there is a wall to the bottom || different room
                            //     // if (Z == 0 || MazeGlobals.cellData[X][Z-1][7]!=currentRoomUID)
                            //     //     CreatePrefab(DoorwayRoomToRoomSlant, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.4f), new Vector3(1,1,1), -90);
                            //     // else
                            //     //     CreatePrefab(DoorwayRoomToRoomSlant, MazeGlobals.cellDoorParent, new Vector3(X+.4f, 0f, Z+.2f), new Vector3(-1,1,1), -90);
                            // }
  
                        // else if (MazeGlobals.cellData[X][Z+1][7]!=currentRoomUID){ // else if - Another room is north of cell
                        //     CreatePrefab(DoorwayRoomToRoom, MazeGlobals.cellDoorParent, X+.4f, 0f, Z+.8f, 0);
                        // }

                    // // bool westDoor = false;
                            // cellDoor = Instantiate(DoorwayCorridorToRoom, new Vector3((X+.2f)*mapScale, 0, (Z+.5f)*mapScale), Quaternion.Euler(0,-90,0));
                            // cellDoor = Instantiate(DoorwayRoomToRoom, new Vector3((X+.2f)*mapScale, 0, (Z+.5f)*mapScale), Quaternion.Euler(0,-90,0));

                            // cellDoor = Instantiate(DoorwayCorridorToRoom, new Vector3((X+.5f)*mapScale, 0, (Z+.8f)*mapScale), Quaternion.Euler(0,0,0));
                            // cellDoor = Instantiate(DoorwayRoomToRoom, new Vector3((X+.5f)*mapScale, 0, (Z+.8f)*mapScale), Quaternion.Euler(0,0,0));


//     public void placeDoors(int X, int Z){
//         int gridX = MazeGlobals.gridX;
//         int gridZ = MazeGlobals.gridZ;
//         float mapScale = MazeGlobals.mapScale;

//         // Run code to place the door here, otherwise you may run the issue of rooms being connected when they shouldn't

//         GameObject cellDoor;
//         // If, in grid && next cell is not room
//         if (X<gridX && Z+1<gridZ && MazeGlobals.cellData[X][Z+1][6]==0){
//             switch(MazeGlobals.cellData[X][Z][0]){
//                 case(0): // If Hallway, make door
//                 cellDoor = Instantiate(Doorway, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+1f)*mapScale), Quaternion.Euler(0,0,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;
//                 case(1): // If wall, make wall
//                 cellDoor = Instantiate(RoomWall, new Vector3(X*mapScale, 0f, (Z+.95f)*mapScale), Quaternion.Euler(0,180,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;
//             }
//         }

//         if (X+1<gridX && Z<gridZ && MazeGlobals.cellData[X+1][Z][6]==0){
//             switch(MazeGlobals.cellData[X][Z][1]){
//                 case(0): // If Hallway, make door
//                 cellDoor = Instantiate(Doorway, new Vector3((X+1f)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;
//                 case(1): // If wall, make wall
//                 cellDoor = Instantiate(RoomWall, new Vector3((X+1f)*mapScale, 0f, (Z+.95f)*mapScale), Quaternion.Euler(0,270,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 // cellDoor = Instantiate(RoomWall, new Vector3((X+.95f)*mapScale, 0f, (Z+1f*mapScale)), Quaternion.Euler(0,270,0));
//                 // cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;
//             }
//         }

//         if (X<gridX && Z-1>=0 && MazeGlobals.cellData[X][Z-1][6]==0){
//             switch(MazeGlobals.cellData[X][Z][2]){
//                 case(0): // If Hallway, make door
//                 cellDoor = Instantiate(Doorway, new Vector3((X+prefabOffsetX)*mapScale, 0, Z*mapScale), Quaternion.Euler(0,0,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;
//                 case(1): // If wall, make wall
//                 cellDoor = Instantiate(RoomWall, new Vector3((X+1f)*mapScale, 0f, (Z+0.05f)*mapScale), Quaternion.Euler(0,0,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 // cellDoor = Instantiate(RoomWall, new Vector3((X+.5f)*mapScale, 0f, (Z+0.05f)*mapScale), Quaternion.Euler(0,0,0));
//                 // cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;
//             }
//         }

//         if (X-1>=0 && Z<gridZ && MazeGlobals.cellData[X-1][Z][6]==0){
//             switch(MazeGlobals.cellData[X][Z][3]){
//                 case(0): // If Hallway, make door
//                 cellDoor = Instantiate(Doorway, new Vector3((X)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;

//                 case(1): // If wall, make wall
//                 cellDoor = Instantiate(RoomWall, new Vector3((X+0.05f)*mapScale, 0f, Z*mapScale), Quaternion.Euler(0,90,0));
//                 cellDoor.transform.parent = MazeGlobals.cellDoorParent.transform;
//                 break;
//             }
//         }
//     }
// }