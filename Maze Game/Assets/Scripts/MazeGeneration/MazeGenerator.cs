﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour{


    public GameObject GoalCube;
    public GameObject GoalHackCube;
    public GameObject GoalGroup;
    private GoalDetector GoalDetector;

    [Header("Scripts", order=1)]
    public RecursiveBacktrack RecursiveBacktrack;
    public MazeHackPrefabs MazeHackPrefabs;
    public InitializeMaze InitializeMaze;
    public SymmetricMaze SymmetricMaze;
    public MazePrefabs MazePrefabs;
    public MazeGlobals MazeGlobals;
    public ResetMaze ResetMaze;
    public Check Check;

    // private int startX = 0;
    // private int startZ = 0;
    private int endX = 0;
    private int endZ = 0;
    private int gridX = 10; // Initial grid size
    private int gridZ = 10; // Initial grid size

    private GameObject stationCam;
    private Camera cam1;
    private GameObject hackerCam;
    private Camera cam2;

    [Header("Objects", order=2)]
    public GameObject hackGameGroup;
    public GameObject spaceStationGroup;
    private PlatformController PlatformController;

    [Header("Parameters", order=3)]
    public bool enableCulling=false;
    public bool enableMeshCombining=false;
    public bool enableDebugRaycast=false;

    private GameObject lockedDoor;

    public int gameStatus = 0; // 1 if hacking / 0 if station
    private PlayerManager PlayerManager;
    private GameObject PlayerGroup;
    private GameObject StationCamStandard;



    // Called before any Start function
    void Awake(){
        GoalDetector = GoalCube.GetComponent<GoalDetector>();

        RecursiveBacktrack  = gameObject.GetComponent<RecursiveBacktrack>();
        MazeHackPrefabs     = gameObject.GetComponent<MazeHackPrefabs>();
        InitializeMaze      = gameObject.GetComponent<InitializeMaze>();
        SymmetricMaze       = gameObject.GetComponent<SymmetricMaze>();
        MazePrefabs         = gameObject.GetComponent<MazePrefabs>();
        MazeGlobals         = gameObject.GetComponent<MazeGlobals>();
        ResetMaze           = gameObject.GetComponent<ResetMaze>();
        Check               = gameObject.GetComponent<Check>();

        PlatformController = hackGameGroup.GetComponent<PlatformController>();
        PlayerManager = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
        PlayerGroup = GameObject.FindWithTag("PlayerGroup");
        StationCamStandard = GameObject.FindWithTag("StationCamStandard");
    }

    void Start(){
        // // GenerateSpaceStation();
        // float pcPosX = (startX+.5f) * MazeGlobals.mapScale;
        // float pcPosZ = (startZ+.5f) * MazeGlobals.mapScale;

        // Temporarily disable controller while repositioning the player object
        MazeGlobals.charController.enabled = false;
        MazeGlobals.playerGroup.transform.position = new Vector3(5f, MazeGlobals.playerGroup.transform.position.y, 5f);
        MazeGlobals.charController.enabled = true;
    }


    public void GenerateSpaceStation(){

        ResetMaze.Reset();

        switch(MazeGlobals.type){
            case(0):
                RecursiveBacktrack.Generate(endX, endZ); // Generate maze from end of maze
            break;
            case(1):
                SymmetricMaze.Generate(endX, endZ); // Generate maze from end of maze
            break;
        }

        Check.CellEnds();

        MazePrefabs.Base();

        MazePrefabs.Corridors();
        
        Check.MainPath(); // Crawl through the maze and find the main path

        MazePrefabs.Rooms(); // Interspace rooms along that path

        SetGoalPosition();

        StationCamStandard.transform.position = new Vector3(StationCamStandard.transform.position.x,1f,StationCamStandard.transform.position.z);
    }

    public void SetGoalPosition(){
        float gX, gZ = 0f;
        gX  = (MazeGlobals.endX-.5f) * MazeGlobals.mapScale;
        gZ  = (MazeGlobals.endZ-.5f) * MazeGlobals.mapScale;

        GoalGroup.transform.position = new Vector3(gX, GoalGroup.transform.position.y, gZ);

        endX = MazeGlobals.endX;
        endZ = MazeGlobals.endZ;
    }



    // Update is called once per frame
    void Update(){
        // Debug.Log(GoalDetector.goalTrigger);
        if (GoalDetector.goalTrigger){
            // PlayerGroup = GameObject.FindWithTag("PlayerGroup");
            // if (PlayerGroup!=null) PlayerGroup.SetActive(false);
            
            GoalDetector.goalTrigger = false;

            MazeGlobals.mode = 0;

            // Update grid size before reset
            PlayerManager.ResetMetrics();   // Reset metrics and generate meterics for next maze generation

            // gridX = 10;
            // gridZ = 10;
            // MazeGlobals.gridX = gridX;
            // MazeGlobals.gridZ = gridZ;
            ResetMaze.Reset();
            GenerateSpaceStation();

            StationCamStandard.transform.position = new Vector3(StationCamStandard.transform.position.x,10000f,StationCamStandard.transform.position.z);
            // if (PlayerGroup!=null) PlayerGroup.SetActive(true);
        }
    }


    public void HackingGame(GameObject doorway, int gridX, int gridZ){
        gameStatus = 1;

        lockedDoor = doorway; // Store callback for later reference - so door can be opened on win

        PlatformController.ResetTries();    // Reset try counter / Rotation
        PlatformController.ResetBall();     // Ball position - Do this after or you may trigger a loss on startup

        // cam1.enabled = !cam1.enabled;
        // cam2.enabled = !cam2.enabled;
        PlayerManager.GameToHack();

        MazeGlobals.mode = 1;

        // Set new grid size before reset
        MazeGlobals.gridX = gridX;
        MazeGlobals.gridZ = gridZ;
        ResetMaze.Reset();

        SymmetricMaze.Generate(0,0);

        Check.CellEnds();

        MazeHackPrefabs.Corridors();

        SetHackGoalPosition();

        // Toggle section groups
        hackGameGroup.SetActive(true);
        spaceStationGroup.SetActive(false);
        
    }




    /*
    NOTE: 
        For the hacking game system it may be easier for the
        platform function to call back to the door directly.
    */
    public void HackingGameWin(){
        lockedDoor.GetComponent<Doorway>().HackWin();
        HackingGameEnd();
        PlayerManager.RaiseHackDifficulty();
    }

    public void HackingGameLoss(){
        lockedDoor.GetComponent<Doorway>().HackLoss();
        HackingGameEnd();
        PlayerManager.LowerHackDifficulty();
    }

    void HackingGameEnd(){
        PlayerManager.HackToGame();

        // hackGameGroup.SetActive(false);
        // spaceStationGroup.SetActive(true);
        gameStatus = 0;

    }

    public void SetHackGoalPosition(){
        GoalHackCube.transform.position = new Vector3((MazeGlobals.gridX-.5f)*10f, GoalHackCube.transform.position.y+2f, (MazeGlobals.gridZ-.5f)*10f);
    }
}