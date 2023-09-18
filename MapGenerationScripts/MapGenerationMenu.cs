using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;

public class MapGenerationMenu : EditorWindow
{
    int mapWidth;
    GameObject whiteHoleCenter; // this will be used to delete the entire map
    GameObject aWall, bWall, cWall, dWall, eWall;
    ///
    /// The hunter areana
    /// 
    GameObject theHunter;
    BlockConditionsSO hunterDetermineCondition;
    RaycastHit hitInfo;
    GameObject objectBelow, objectRight, objectAbove, objectLeft;
    List<GameObject> sensorPods = new List<GameObject>();

    // Hunter pathfinding
    int hunterTotalIterations = 1;
    float hunterDistance = 10;
    int numTimesHunterRevolves = 0;

    private MovementState hunterMoveState = MovementState.UpRight;
    Dictionary<string, Quaternion> blockRotationDictionary = new Dictionary<string, Quaternion>()
    {
         {"0000", Quaternion.Euler(0, 0, 0)}, // a

        {"1000", Quaternion.Euler(0, 90, 0)}, // b0
        {"0100", Quaternion.Euler(0, 0, 0)}, // b1
        {"0010", Quaternion.Euler(0, 270, 0)},// b2
        {"0001", Quaternion.Euler(0, 180, 0)},// b3

        {"0101", Quaternion.Euler(0, 90, 0)}, // c0
        {"1010", Quaternion.Euler(0, 0, 0)}, // c1

        {"1100", Quaternion.Euler(0, 90, 0)}, // d0
        {"0110", Quaternion.Euler(0, 0, 0)}, // d1
        {"0011", Quaternion.Euler(0, 270, 0)}, // d2
        {"1001", Quaternion.Euler(0, 180, 0)}, //d3

        {"1110", Quaternion.Euler(0, 90, 0)}, // e1
        {"0111", Quaternion.Euler(0, 0, 0)}, // e1
        {"1011", Quaternion.Euler(0, 270, 0)}, // e2
        {"1101", Quaternion.Euler(0, 180, 0)}, //e3

    };

    //Random Generation
    private float blockGenerator;

    [MenuItem("Tools/Generate Whitehole Map")]
    public static void ShowWindow() { GetWindow(typeof(MapGenerationMenu));  }

    private void OnEnable()
    {
        BlockConditionsSO.SendBlockInformationEV += UpdateNewBlockType;
    }
    private void OnDisable()
    {
        BlockConditionsSO.SendBlockInformationEV -= UpdateNewBlockType;
    }

    private void OnGUI()
    {
        mapWidth = EditorGUILayout.IntField("Max Map block size", mapWidth);

        aWall = EditorGUILayout.ObjectField("No walls(A)", aWall, typeof(GameObject), true) as GameObject;
        bWall = EditorGUILayout.ObjectField("Single wall(B)", bWall, typeof(GameObject), true) as GameObject;
        cWall = EditorGUILayout.ObjectField("Double wall(C)", cWall, typeof(GameObject), true) as GameObject;
        dWall = EditorGUILayout.ObjectField("Corner wall(D)", dWall, typeof(GameObject), true) as GameObject;
        eWall = EditorGUILayout.ObjectField("Deadend Wall(E)", eWall, typeof(GameObject), true) as GameObject;
       

        hunterDetermineCondition = EditorGUILayout.ObjectField("Block Condition Statements", hunterDetermineCondition, typeof(BlockConditionsSO), true) as BlockConditionsSO;

        if (GUILayout.Button("Generate Whitehole Map"))
        {
            GenerateMap();
        }

    }

    private void GenerateMap()
    {
        DestroyMap(); // Clean up previous map first; reset everything to fresh

        if (mapWidth <= 0)
        {
            Debug.LogError("You put an invalid number. You can't have negative width, and the map can't have size 0");
            return;
        }


        if (whiteHoleCenter == null)
        {
            whiteHoleCenter = new GameObject();
            whiteHoleCenter.name = "MapCore";
            whiteHoleCenter.transform.position = new Vector3(0, 0, 0);
        }
        WhiteHoleBegin();
    }

    private void DestroyMap()
    {
        //Clean up and reset vars to default
        hunterDetermineCondition.ResetLists();
        hunterTotalIterations = 1;
        hunterDistance = 10;
        numTimesHunterRevolves = 0;
        hunterMoveState = MovementState.UpRight; 
        DestroyImmediate(theHunter);
        DestroyImmediate(whiteHoleCenter);
    }
    
    private void WhiteHoleBegin()
    {
        
        blockGenerator = UnityEngine.Random.Range(1, 36);
   
            switch (blockGenerator)
            {
                case float n when (n <= 10 && n >= 1):
                    PrefabUtility.InstantiatePrefab(aWall, whiteHoleCenter.transform);
                    break;

                case float n when (n <= 20 && n >= 11):
                    PrefabUtility.InstantiatePrefab(bWall, whiteHoleCenter.transform);
                    break;

                case float n when (n <= 28 && n >= 21):
                    PrefabUtility.InstantiatePrefab(cWall, whiteHoleCenter.transform);
                    break;

                case float n when (n <= 32 && n >= 29):
                    PrefabUtility.InstantiatePrefab(dWall, whiteHoleCenter.transform);
                    break;

                case float n when (n <= 36 && n >= 33):
                    PrefabUtility.InstantiatePrefab(eWall, whiteHoleCenter.transform);
                    break;

            }

        SendOutTheHunters(); 
    
    }    

    //Begin the process of sending out an empty game object that will go around each newly spawned piece counter-clockwise. 
    // This function determines what block to spawn depending on what it finds in its environment and based on the tile ruleset.

    private void SendOutTheHunters() // I want all of those lifepods.... DESTROYED
    {
        if (theHunter == null)
        {
            theHunter = new GameObject();
            theHunter.name = "THE HUNTER";
            theHunter.transform.parent = whiteHoleCenter.transform;
        }

        // Base case, hunter will always move to the default position below the Whitehole Center to start the chain
        theHunter.transform.localPosition = whiteHoleCenter.transform.position + new Vector3(0, 0, -hunterDistance * hunterTotalIterations); 
        HunterSpawnBlock();
      
        if (hunterMoveState == MovementState.UpRight)
        {
                
                for (int i = 0; i < hunterTotalIterations; i++)
                {
                    theHunter.transform.localPosition += new Vector3(10, 0, 10); // move up 1, to the right 1
                    HunterSpawnBlock();
                   
                }
            
            hunterMoveState = hunterMoveState.GetNextMovementDirection(); // State changer 
            
        }

            if (hunterMoveState == MovementState.UpLeft)
            {
                    for (int i = 0; i < hunterTotalIterations; i++)
                    {
                        theHunter.transform.localPosition += new Vector3(-10, 0, 10); // move up 1, to the left 1
                        HunterSpawnBlock();
                   
                    }
           
                hunterMoveState = hunterMoveState.GetNextMovementDirection(); // State changer 
            
            }
            

            if (hunterMoveState == MovementState.LeftDown)
            {
                for (int i = 0; i < hunterTotalIterations; i++)
                {
                    theHunter.transform.localPosition += new Vector3(-10, 0, -10); // move left 1, down 1
                    HunterSpawnBlock();
                    
                }

                if(hunterTotalIterations == 1)
                {
                    // check condition only on first round
                    HunterCheckIterationConditions();
                }
                

            hunterMoveState = hunterMoveState.GetNextMovementDirection(); // State changer 
    

            }
                  
            if (hunterMoveState == MovementState.DownRight)
            { 
                if (numTimesHunterRevolves == 0 && hunterTotalIterations == 2) // will be true only once
                {
                    // Keep this as default condition
                }
                else
                {
                        for (int i = 0; i < hunterTotalIterations -1; i++)
                        {

                            theHunter.transform.localPosition += new Vector3(10, 0, -10); // move down 1, right 1
                            HunterSpawnBlock();
                        }   
                    HunterCheckIterationConditions();
                }

            hunterMoveState = hunterMoveState.GetNextMovementDirection(); // State changer  
            }

     
                if(hunterTotalIterations != mapWidth + 1) // keep spawning map to map size
                     SendOutTheHunters();
                else
                {
                    Debug.Log("Map done processing!");
                    return;
                }
        
    }

    private void HunterSpawnBlock()
    {
        Debug.DrawRay(theHunter.transform.position, Vector3.left * 10, Color.green, .1f);
        Debug.DrawRay(theHunter.transform.position, Vector3.right * 10, Color.red, .1f);
        Debug.DrawRay(theHunter.transform.position, Vector3.forward * 10, Color.blue, .1f);
        Debug.DrawRay(theHunter.transform.position, Vector3.back * 10, Color.yellow, .1f);

        // Send out the SENSORS. These will detect what type of blocks are around the Hunter
        if (Physics.Raycast(theHunter.transform.localPosition, Vector3.back, out hitInfo))
        {
            objectBelow = hitInfo.transform.gameObject;
        }
        else
            objectBelow = aWall; // since A wall is compatable with everything, empty spaces are also Awalls
        if (Physics.Raycast(theHunter.transform.localPosition, Vector3.right, out hitInfo))
        {
            objectRight = hitInfo.transform.gameObject;
        }
        else
            objectRight = aWall;
        if (Physics.Raycast(theHunter.transform.localPosition, Vector3.forward, out hitInfo))
        {
            objectAbove = hitInfo.transform.gameObject;
        }
        else
            objectAbove = aWall;
        if (Physics.Raycast(theHunter.transform.localPosition, Vector3.left, out hitInfo))
        {
            objectLeft = hitInfo.transform.gameObject;
        }
        else
            objectLeft = aWall;

        sensorPods.Add(objectBelow);
        sensorPods.Add(objectRight);
        sensorPods.Add(objectAbove);
        sensorPods.Add(objectLeft);
        hunterDetermineCondition.DetermineBlock(sensorPods);

        sensorPods.Clear(); // reset list for next time
        numTimesHunterRevolves++;
  
    }

    private void HunterCheckIterationConditions()
    {
        
        // Formula for checking condition = 4 * total iterations
        if (numTimesHunterRevolves == (4 * hunterTotalIterations))
        {
            hunterTotalIterations++;
            numTimesHunterRevolves = 0;
        }
    }

    private void UpdateNewBlockType(string finalKey)
    {
        GameObject temp; // use this to get around b1,b2,etc REFERENCING the actual prefab. This temp has no connection to any of the prefabs; thus will not affect them

         if (finalKey == "1000" || finalKey == "0100" || finalKey == "0010" || finalKey == "0001")
            temp = PrefabUtility.InstantiatePrefab(bWall, theHunter.transform) as GameObject;
        else if (finalKey == "0101" || finalKey == "1010")
            temp = PrefabUtility.InstantiatePrefab(cWall, theHunter.transform) as GameObject;
        else if (finalKey == "1100" || finalKey == "0110" || finalKey == "0011" || finalKey == "1001")
            temp = PrefabUtility.InstantiatePrefab(dWall, theHunter.transform) as GameObject;
        else if (finalKey == "1110" || finalKey == "0111" || finalKey == "1011" || finalKey == "1101")
            temp = PrefabUtility.InstantiatePrefab(eWall, theHunter.transform) as GameObject;
        else
            temp = PrefabUtility.InstantiatePrefab(aWall, theHunter.transform) as GameObject;

            temp.transform.rotation = blockRotationDictionary[finalKey];

       
        // set the block to follow the master parent
        temp.transform.parent = whiteHoleCenter.transform;
    }
    
}

public enum MovementState
{
    UpRight,
    UpLeft,
    LeftDown,
    DownRight
}
public static class HunterMovementExtention
{
    public static MovementState GetNextMovementDirection(this MovementState currentMoveDirection) => currentMoveDirection switch
    {
        MovementState.UpRight => MovementState.UpLeft,
        MovementState.UpLeft => MovementState.LeftDown,
        MovementState.LeftDown => MovementState.DownRight,
        MovementState.DownRight => MovementState.UpRight,

        _ => throw new InvalidOperationException()
    };
}

