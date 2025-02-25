﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour{


    [Header("Maze Settings", order=0)]
    public int gridX;
    public int gridZ;
    public int endX;
    public int endZ;
    private int startX=0;
    private int startZ=0;
    public bool showRawMaze = true;
    public bool showPrefabMaze = true;
    public bool hideBase = true;
    public bool hideWalls = true;
    public bool hideWaypoint = true;

    // Map Prefabs
    [Header("Tile Prefabs", order=1)]
    public GameObject CorridorHall;
    public GameObject CorridorEnd;
    public GameObject CorridorCorner;
    public GameObject CorridorTJunction;
    public GameObject CorridorIntersection;
    public GameObject Doorway;
    public GameObject Room2x3;
    public GameObject Room2x2;
    public GameObject RoomWall;
    float prefabOffsetX = .5f;
    float prefabOffsetZ = 0.5f;


    // RawMaze
    [Header("Raw Maze", order=2)]
    public GameObject wall;
    public GameObject[ ] walls;
    public GameObject goalObject;
    public GameObject playerObject;

    public List<List<List<GameObject>>> cellList = new List<List<List<GameObject>>>();
    public List<List<List<int>>> cellData = new List<List<List<int>>>();

    [Header("Materials", order=3)]
    public Material wallMat;
    public Material playerMat;
    public Material whiteMat;

    [Header("Misc.", order=4)]

    [HideInInspector] public float gridOffset;
    [HideInInspector] public int x;
    [HideInInspector] public int z;
    [HideInInspector] public int endDist = -1;
    [HideInInspector] public int mapScale = 10;
    [HideInInspector] private int startDistance = 0;
    [HideInInspector] private float wallHeight = .3f;
    
    [HideInInspector] public List<List<int>> stack = new List<List<int>>();
    [HideInInspector] public List<List<int>> visited = new List<List<int>>();
    [HideInInspector] public List<List<int>> optimalPath = new List<List<int>>();
    
    [HideInInspector] public List<List<GameObject>> prefabList = new List<List<GameObject>>();
    [HideInInspector] public CharacterController charController;
    [HideInInspector] public GameObject prefabMazeParent;
    [HideInInspector] public GameObject rawMazeParent;
    [HideInInspector] public GameObject guideCubeParent;
    [HideInInspector] public GameObject cellDoorParent;
    [HideInInspector] public GameObject cellWallParent;
    [HideInInspector] public GameObject mapObjects;


    public void Start(){
        charController = playerObject.GetComponent<CharacterController>();

        rawMazeParent = GameObject.Find("MapObjects/rawMazeParent");
        guideCubeParent = GameObject.Find("MapObjects/guideCubeParent");
        prefabMazeParent = GameObject.Find("MapObjects/prefabMazeParent");
        cellDoorParent = GameObject.Find("MapObjects/cellDoorParent");
        cellWallParent = GameObject.Find("MapObjects/cellWallParent");
        mapObjects = GameObject.Find("MapObjects");

    }



    public void Reset(){
        for(int X = 0; X < gridX; X++){
            for(int Z = 0; Z < gridZ; Z++){

                // if (showPrefabMaze){
                cellData[X][Z][0] = 1;
                cellData[X][Z][1] = 1;
                cellData[X][Z][2] = 1;
                cellData[X][Z][3] = 1;
                cellData[X][Z][4] = 0;
                cellData[X][Z][5] = 0;
                cellData[X][Z][6] = 0;
                // }

                if (showRawMaze){
                    cellList[X][Z][0].SetActive(true);
                    cellList[X][Z][1].SetActive(true);
                    // cellList[X][Z][2].SetActive(true);
                    // cellList[X][Z][3].SetActive(true);
                }
            }
        }


        /*You can deactive the container by using SetActive(false); instead of destroying it. And then run a coroutine to destroy every child object every frame. This might speed things up. By the way it is always good to show a loading screen while swithing between levels.*/
        foreach (Transform child in rawMazeParent.transform) child.gameObject.SetActive(true);
        foreach (Transform child in guideCubeParent.transform) GameObject.Destroy(child.gameObject);
        foreach (Transform child in prefabMazeParent.transform) GameObject.Destroy(child.gameObject);
        foreach (Transform child in cellDoorParent.transform) GameObject.Destroy(child.gameObject);
        foreach (Transform child in cellWallParent.transform) GameObject.Destroy(child.gameObject);

        // Destroy(prefabMazeParent);
        // Destroy(cellDoorParent);
        // prefabMazeParent = new GameObject("prefabMazeParent");
        // cellDoorParent = new GameObject("cellDoorParent");


        RecursiveBacktrack(startX,startZ); // Build Maze
        FindMainPath();     // Crawl through the maze and find the main path
        AddRooms();         // Interspace rooms along that path 
        HidePrefabs();
    }


    public void HidePrefabs(){ // hides all prefabs
        // foreach (Transform child in prefabMazeParent.transform) child.transform.GetChild(0).gameObject.SetActive(false);
        // foreach (Transform child in prefabMazeParent.transform) child.gameObject.SetActive(false);
    }










    // Start is called before the first frame update 
    public void startMap() {
        

        for(int cellX = 0; cellX < gridX; cellX++){
            List<List<GameObject>> cellRow = new List<List<GameObject>>();
            List<List<int>> cellDataRow = new List<List<int>>();
            // List<GameObject> prefabRow = new List<GameObject>();
            for(int cellZ = 0; cellZ < gridZ; cellZ++){

                List<GameObject> cell = new List<GameObject>();
                List<int> cellDataGroup = new List<int>();
                
                // GameObject cellPrefab = new GameObject();
                // prefabRow.Add(cellPrefab); 

                if (showRawMaze){
                    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube1.transform.localScale = new Vector3(.25f, 2*mapScale, 1*mapScale);
                    cube1.transform.Rotate(0f, 90f, 0f);
                    cube1.GetComponent<Renderer>().material = wallMat;
                    cube1.AddComponent<ObjectHider>();
                    cube1.tag = "Occludable";
                    cell.Add(cube1); // North Wall

                    GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube2.transform.localScale = new Vector3(.25f, 2*mapScale, 1*mapScale);
                    cube2.GetComponent<Renderer>().material = wallMat;
                    cube2.AddComponent<ObjectHider>();
                    cube2.tag = "Occludable";
                    cell.Add(cube2); // East Wall

                    if (hideWalls){
                        cube1.GetComponent<MeshRenderer>().enabled = false;
                        cube2.GetComponent<MeshRenderer>().enabled = false;
                    }

                    // Place object under a single parent
            		cube1.transform.parent = rawMazeParent.transform;
            		cube2.transform.parent = rawMazeParent.transform;

            		// Set position of walls
                    cube1.transform.position = new Vector3(mapScale*(.5f+cellX),    wallHeight,  mapScale*(1f+cellZ));
                    cube2.transform.position = new Vector3(mapScale*(cellX+1f),     wallHeight,  mapScale*(cellZ+.5f));
                }
                
                // Add objects to list
                cellRow.Add(cell);
                cellDataGroup.Add(1);  // N
                cellDataGroup.Add(1);  // E
                cellDataGroup.Add(1);  // S
                cellDataGroup.Add(1);  // W 
                cellDataGroup.Add(0);  // Distance from start
                cellDataGroup.Add(0);  // On path to goal
                cellDataGroup.Add(0);  // Room cell
                cellDataGroup.Add(0);  // RoomCellID
                cellDataRow.Add(cellDataGroup);
            }
            cellList.Add(cellRow);
            cellData.Add(cellDataRow);
            // prefabList.Add(prefabRow); 
        }



        // Create level base
        // GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // ground.transform.position   = new Vector3(mapScale*(gridX*.5f), 0, mapScale*(gridZ*.5f));
        // ground.transform.localScale = new Vector3(mapScale*gridX, 0.01f, mapScale*gridZ);
        // ground.layer = 8;
        // ground.transform.parent = rawMazeParent.transform; // Place object under a single parent
        // if (hideBase) ground.GetComponent<MeshRenderer>().enabled = false;
        // ground.transform.parent = mapObjects.transform; // Place object under a single parent

        GameObject mapGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mapGround.transform.position   = new Vector3(mapScale*(gridX*.5f), -4f, mapScale*(gridZ*.5f));
        mapGround.transform.localScale = new Vector3(mapScale*gridX, 0.01f, mapScale*gridZ);
        mapGround.GetComponent<Renderer>().material = whiteMat;
        mapGround.layer = 8;
        mapGround.transform.parent = mapObjects.transform; // Place object under a single parent

        // South Wall
        GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube3.transform.localScale = new Vector3(.25f, 1*mapScale, 1*mapScale*gridX);
        cube3.transform.Rotate(0f, 90f, 0f);
        cube3.GetComponent<Renderer>().material = wallMat;
        cube3.transform.position = new Vector3(cube3.transform.position.x+(gridX*.5f*mapScale), cube3.transform.position.y, cube3.transform.position.x);
        cube3.transform.parent = mapObjects.transform; // Place object under a single parent

        // West Wall
        GameObject cube4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube4.transform.localScale = new Vector3(.25f, 1*mapScale, 1*mapScale*gridZ);
        cube4.transform.position = new Vector3(cube4.transform.position.x, cube4.transform.position.y, cube4.transform.position.x+(gridZ*.5f*mapScale));
        cube4.GetComponent<Renderer>().material = wallMat;
        cube4.transform.parent = mapObjects.transform; // Place object under a single parent




        RecursiveBacktrack(startX,startZ); // Build Maze
        FindMainPath();     // Crawl through the maze and find the main path
        AddRooms();         // Interspace rooms along that path 

        float pcPosX = (startX+.5f) * mapScale;
        float pcPosZ = (startZ+.5f) * mapScale;

        // Temporarily disable controller while repositioning the player object
        charController.enabled = false;
        playerObject.transform.position = new Vector3(pcPosX, playerObject.transform.position.y, pcPosZ);
        charController.enabled = true;
    }








    public void FindMainPath(){
        // startX,startZ
        // endX, endZ
        optimalPath.Clear();
        bool endFound = false;
        int counter = 0;
        x = endX-1;
        z = endZ-1;

        int distance;

        // Get starting distance based off goal location
        print("Start Point ("+x+", "+z+")");

        distance = cellData[x][z][4];
        while (!endFound){
            counter++;

            // Find neighboring cell with lowest distance from start


            // If there is a adjacent path && distance is less than previous (i.e. getting closer to the start.)
            if (cellData[x][z][0]==0 && cellData[x][z+1][4]<distance){ // North
                distance=cellData[x][z+1][4]; z++;

            }else if (cellData[x][z][1]==0 && cellData[x+1][z][4]<distance){ // East
                distance=cellData[x+1][z][4]; x++;

            }else if (cellData[x][z][2]==0 && cellData[x][z-1][4]<distance){ // South
                distance=cellData[x][z-1][4]; z--;

            }else if (cellData[x][z][3]==0 && cellData[x-1][z][4]<distance){ // West
                distance=cellData[x-1][z][4]; x--;
            }

            if (hideWaypoint==false) GuideCube(x,z);
            cellData[x][z][5]=1;   // Mark cell as on path
            optimalPath.Add(new List<int>{x,z});


            if (distance==0){
                endFound = true;
            }
        }
    }




    public void GuideCube(int cubeX, int cubeZ){
        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube2.transform.localScale = new Vector3(1f, 1f, 1f);
        cube2.GetComponent<Renderer>().material = playerMat;
        cube2.transform.position = new Vector3((cubeX+.5f)*mapScale, 10f, (cubeZ+.5f)*mapScale);
        cube2.transform.parent = guideCubeParent.transform;

    }








    public void AddRooms(){


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

        // Iterate over elements in optimal path
        foreach(List<int> pathElement in optimalPath){
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
                    if (cellX<gridX && cellZ<gridZ && cellData[cellX][cellZ][5]==1 && cellData[cellX][cellZ][6]==0){
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
                            prefabRoom = Instantiate(Room2x3, new Vector3((X+1f)*mapScale, 0f, (Z+1.5f)*mapScale), Quaternion.Euler(0,90,0));
                            prefabRoom.transform.parent = cellDoorParent.transform;
                        break;

                        case (2):
                            prefabRoom = Instantiate(Room2x2, new Vector3((X+1f)*mapScale, 0f, (Z+1f)*mapScale), Quaternion.Euler(0,90,0));
                            prefabRoom.transform.parent = cellDoorParent.transform;
                        break;
                    }
                    
                    // Remove walls in room cells
                    foreach(List<int> cell in room){
                        cellX = cell[0]+X;
                        cellZ = cell[1]+Z;

                        roomCell(cellX,cellZ,room); // n
                    }

                    // Place room doors
                    foreach(List<int> cell in room){
                        cellX = cell[0]+X;
                        cellZ = cell[1]+Z;

                        placeDoors(cellX,cellZ); // n
                    }

                    // // Place Exterior walls
                    // foreach(List<int> cell in room){
                    //     cellX = cell[0]+X;
                    //     cellZ = cell[1]+Z;

                    //     placeWalls(cellX,cellZ); // n
                    // }
                }
            }
        }
    }





    public void roomCell(int X, int Z, List<List<int>> room){
        // Disable walls in same room

        // cellList[X][Z][0].GetComponent<Renderer>().material = playerMat;
        // print("ROOM CELL ("+X+", "+Z+")");

        foreach(List<int> cell in room){
            int cellX = x+cell[0];
            int cellZ = z+cell[1];
            // print("     ("+cellX+", "+cellZ+")");
            if (cellX == X && cellZ == Z+1){
                cellList[X][Z][0].SetActive(false);

            }else if (cellX == X+1 && cellZ == Z){
                cellList[X][Z][1].SetActive(false);

            }else if (cellX == X && cellZ == Z-1){
                cellList[X][Z-1][0].SetActive(false);

            }else if (cellX == X-1 && cellZ == Z){
                cellList[X-1][Z][1].SetActive(false);
            }
        }


        cellData[X][Z][6]=1;    // Mark cell as room
        Destroy(prefabList[X][Z]);
    }



    public void placeDoors(int X, int Z){

        // Run code to place the door here, otherwise you may run the issue of rooms being connected when they shouldn't

        GameObject cellDoor;
        // If, in grid && next cell is not room
        if (X<gridX && Z+1<gridZ && cellData[X][Z+1][6]==0){
            switch(cellData[X][Z][0]){
                case(0): // If Hallway, make door
                cellDoor = Instantiate(Doorway, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+1f)*mapScale), Quaternion.Euler(0,0,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                break;
                case(1): // If wall, make wall
                cellDoor = Instantiate(RoomWall, new Vector3(X*mapScale, 0f, (Z+.95f)*mapScale), Quaternion.Euler(0,180,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                break;
            }
        }

        if (X+1<gridX && Z<gridZ && cellData[X+1][Z][6]==0){
            switch(cellData[X][Z][1]){
                case(0): // If Hallway, make door
                cellDoor = Instantiate(Doorway, new Vector3((X+1f)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                break;
                case(1): // If wall, make wall
                cellDoor = Instantiate(RoomWall, new Vector3((X+1f)*mapScale, 0f, (Z+.95f)*mapScale), Quaternion.Euler(0,270,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                // cellDoor = Instantiate(RoomWall, new Vector3((X+.95f)*mapScale, 0f, (Z+1f*mapScale)), Quaternion.Euler(0,270,0));
                // cellDoor.transform.parent = cellDoorParent.transform;
                break;
            }
        }

        if (X<gridX && Z-1>=0 && cellData[X][Z-1][6]==0){
            switch(cellData[X][Z][2]){
                case(0): // If Hallway, make door
                cellDoor = Instantiate(Doorway, new Vector3((X+prefabOffsetX)*mapScale, 0, Z*mapScale), Quaternion.Euler(0,0,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                break;
                case(1): // If wall, make wall
                cellDoor = Instantiate(RoomWall, new Vector3((X+1f)*mapScale, 0f, (Z+0.05f)*mapScale), Quaternion.Euler(0,0,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                // cellDoor = Instantiate(RoomWall, new Vector3((X+.5f)*mapScale, 0f, (Z+0.05f)*mapScale), Quaternion.Euler(0,0,0));
                // cellDoor.transform.parent = cellDoorParent.transform;
                break;
            }
        }

        if (X-1>=0 && Z<gridZ && cellData[X-1][Z][6]==0){
            switch(cellData[X][Z][3]){
                case(0): // If Hallway, make door
                cellDoor = Instantiate(Doorway, new Vector3((X)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                break;
                case(1): // If wall, make wall
                cellDoor = Instantiate(RoomWall, new Vector3((X+0.05f)*mapScale, 0f, Z*mapScale), Quaternion.Euler(0,90,0));
                cellDoor.transform.parent = cellDoorParent.transform;
                break;
            }
        }

    }





    public bool availableCell(int cellX, int cellZ){
        // Debug.Log(""+cellX+">"+(gridX-1)+"   "+cellZ+">"+(gridZ-1)+"   "+cellX+"<0    "+cellZ+"<0");
        if (cellX>gridX-1 || cellZ>gridZ-1 || cellX<0 || cellZ<0) return false;
        
        for(int i = 0; i < visited.Count; i++){
            // Debug.Log("Compare: ("+ cellX + ", "+cellZ+") to ("+visited[i][0]+ ", " + visited[i][1] + ")");
            if (visited[i][0]==cellX && visited[i][1]==cellZ ){
                return false;
            }
        }
        return true;
    }
















    public void moveN(){
        cellData[x][z][0] = 0;
        cellData[x][z+1][2] = 0;
        
        if (showRawMaze){
            cellList[x][z][0].SetActive(false);
            // cellList[x][z+1][2].SetActive(false);
        }
        z+=1;
    }

    public void moveE(){
        cellData[x][z][1] = 0;
        cellData[x+1][z][3] = 0;

        if (showRawMaze){
            cellList[x][z][1].SetActive(false);
            // cellList[x+1][z][3].SetActive(false);
        }
        x+=1;
    }

    public void moveS(){
        cellData[x][z][2] = 0;
        cellData[x][z-1][0] = 0;

        if (showRawMaze){
            // cellList[x][z][2].SetActive(false);
            cellList[x][z-1][0].SetActive(false);
        }
        z-=1;
    }

    public void moveW(){
        cellData[x][z][3] = 0;
        cellData[x-1][z][1] = 0;

        if (showRawMaze){
            // cellList[x][z][3].SetActive(false);
            cellList[x-1][z][1].SetActive(false);
        }
        x-=1;                   // make this cell the current cell
    }



















    public void RecursiveBacktrack(int startX, int startZ){
        print("Recursive Backtrack");
        // Set start points to last end point
        startX = endX;
        startZ = endZ;
        x = endX-1;
        z = endZ-1;

        // Reset distance counters
        endDist = -1;
        startDistance = 0;

        stack.Clear();
        visited.Clear();

        List<int> currentCell = new List<int>();
        currentCell.Add(x);
        currentCell.Add(z);
        // single_cell(x, z)                    // starting positing of maze

        stack.Add(currentCell);                 // place starting cell into stack
        visited.Add(currentCell);               // add starting cell to visited list


        while(stack.Count > 0){
            // Log cells relative distance from start
            // int positionFromStart = startDistance;
            cellData[x][z][4] = startDistance;

            List<string> cellList = new List<string>();

            if (availableCell(x,z+1)) cellList.Add("n");
            if (availableCell(x+1,z)) cellList.Add("e");
            if (availableCell(x,z-1)) cellList.Add("s");
            if (availableCell(x-1,z)) cellList.Add("w");

            // Log cells relative distance from start
            cellData[x][z][4] = startDistance;
            
            if (cellList.Count > 0){
                startDistance+=1;


                int cellChosen = Random.Range(0, cellList.Count);

                if (cellList[cellChosen] == "n"){
                    moveN();
                } 

                if (cellList[cellChosen] == "e"){
                    moveE();
                } 

                if (cellList[cellChosen] == "s"){
                    moveS();
                } 

                if (cellList[cellChosen] == "w"){
                    moveW();
                }
                List<int> nextCell = new List<int>();
                nextCell.Add(x);
                nextCell.Add(z);
                stack.Add(nextCell);     // add to visited list
                visited.Add(nextCell);   // place current cell on to stack

                // Debug.Log(cellList[cellChosen] + " ("+x+", "+z+")");
            }else{
                startDistance-=1;
                stack.RemoveAt(stack.Count - 1);
                if (stack.Count == 0) break;
                x = stack[stack.Count-1][0];
                z = stack[stack.Count-1][1];

            }
        }
        GetCellEnds();

    }



















    public void GetCellEnds(){
        print("Get Cell Ends");
        // List all candidates for start and end points

        // Reset the prefab list
        prefabList = new List<List<GameObject>>();

        for(int X = 0; X < gridX; X++){
            List<GameObject> prefabRow = new List<GameObject>();

            for(int Z = 0; Z < gridZ; Z++){
                
                int findCount = 0;

                for (int i = 0; i < 4; i++){
                    if (cellData[X][Z][i]==0) findCount++;
                }


                GameObject prefabCell;
                if (showPrefabMaze || findCount==1){
                    switch(findCount) {
                         case (1):
                            if (showPrefabMaze){
                                if (cellData[X][Z][0] == 0 ){
                                    prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,180,0));
                                } else if (cellData[X][Z][1] == 0){
                                    prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,270,0));
                                } else if (cellData[X][Z][2] == 0){
                                    prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                                } else {
                                    prefabCell = Instantiate(CorridorEnd, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                                }
                                prefabRow.Add(prefabCell);
                            }

                            // If end cell is further than the current end cell
                            if (cellData[X][Z][4]>endDist  ||  endDist==-1){
                                // set new end point
                                endX = X+1;
                                endZ = Z+1;
                                endDist = cellData[X][Z][4];
                            }

                            break;
                        case (2):
                        	// Hallways
                            if (cellData[X][Z][0] == 0  && cellData[X][Z][2] == 0 ){
                                prefabCell = Instantiate(CorridorHall, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                        	} else if (cellData[X][Z][1] == 0  && cellData[X][Z][3] == 0 ){
                                prefabCell = Instantiate(CorridorHall, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));

                            // Corners
                        	} else if (cellData[X][Z][0] == 0  && cellData[X][Z][1] == 0 ){
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,270,0));
                        	} else if (cellData[X][Z][1] == 0  && cellData[X][Z][2] == 0 ){
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,0,0));
                        	} else if (cellData[X][Z][2] == 0  && cellData[X][Z][3] == 0 ){
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                        	} else{
                                prefabCell = Instantiate(CorridorCorner, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,180,0));
                        	}
                            prefabRow.Add(prefabCell);
                            break;
                        case (3):
                        	// T Junction
                        	if (cellData[X][Z][0] == 1){
                                prefabCell = Instantiate(CorridorTJunction, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,90,0));
                        	} else if (cellData[X][Z][1] == 1){
                                prefabCell = Instantiate(CorridorTJunction, new Vector3((X+prefabOffsetX)*mapScale, 0, (Z+prefabOffsetZ)*mapScale), Quaternion.Euler(0,180,0));
                        	} else if (cellData[X][Z][2] == 1){
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
                    prefabRow[prefabRow.Count-1].transform.parent = prefabMazeParent.transform; // Group objects
                    prefabRow[prefabRow.Count-1].name = X+","+Z; // Rename with co-ordinates
                }
            }
            prefabList.Add(prefabRow);
        }
        SetSpawnPoints();

    }








    public void SetSpawnPoints(){
        float gX, gZ = 0f;
        gX  = (endX-.5f) * mapScale;
        gZ  = (endZ-.5f) * mapScale;

        // print("Goal: (" + endX + ", " + endZ + ")");
        // Place the goal object in a new location
        goalObject.transform.position = new Vector3(gX, goalObject.transform.position.y, gZ);


    }












    public void resetMap(){
        Reset();

    }
}





// foreach(List<int> cell in room){
//     int cellX = X+cell[0];
//     int cellZ = Z+cell[1];

//     if (cellX == x && cellZ == z+1){
//         moveN();
//     }else if (cellX == x+1 && cellZ == z){
//         moveE();
//     }else if (cellX == x && cellZ == z-1){
//         moveS();
//     }else if (cellX == x-1 && cellZ == z){
//         moveW();
//     }
//     // Mark current cell as a room cell
//     cellData[x][z][6]=1;
//     Destroy(prefabList[x][z]);
// }