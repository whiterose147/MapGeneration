using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = ("Custom/BlockGenerationConditions"))]
public class BlockConditionsSO : ScriptableObject
{
    public static Action<string> SendBlockInformationEV = delegate { };

    #region Base Definitions
    public GameObject aBlock;
    public GameObject bBlock;
    public GameObject cBlock;
    public GameObject dBlock;
    public GameObject eBlock;
    #endregion


    #region Block Dictionary
    string[] blockValues = new string[15] {"0000", "1000","0100","0010","0001","0101","1010","1100","0110","0011","1001","1110","0111","1011","1101"};
   
    Dictionary<string, string[]> blockCompatabilityDictionary = new Dictionary<string, string[]>()
    {
        {"0000", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000", "0100", "0100", "0010", "0010", "0001", "0001", "0101", "0101", "1010", "1010", "1100", "0110", "0011", "1001", "1110", "0111", "1011", "1101" }},
        {"1000", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0100", "0100", "0010", "0010", "0001", "0001", "0101", "0101", "0110", "0011", "0111"}},
        {"0100", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000", "0010", "0010", "0001", "0001", "1010", "1010", "0011", "1001", "1011"}},
        {"0010", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000", "0100", "0100", "0001", "0001", "0101", "0101", "1100", "1001", "1101"}},
        {"0001", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000", "0100", "0100", "0010", "0010", "1010", "1010", "1100", "0110", "1110"}},
        {"0101", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000", "0010", "0010", "1010","1010" }},
        {"1010", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0100", "0100", "0001", "0001", "0101", "0101"}},
        {"1100", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0010", "0010", "0001", "0001", "0011" }},
        {"0110", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000", "0001", "0001", "1001" }},
        {"0011", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000", "0100", "0100", "1100" }},
        {"1001", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0100", "0100", "0010", "0010", "0110" }},
        {"1110", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0001", "0001"}},
        {"0111", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "1000", "1000"}},
        {"1011", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0100", "0100"}},
        {"1101", new string[] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0010", "0010"}},
       
    };
    #endregion

    #region Local Vars
    List<string> blockNames = new List<string>(4);
    List<Quaternion> blockRotations = new List<Quaternion>(4);
    #endregion
    public void ResetLists()
    {
        blockNames.Clear();
        blockRotations.Clear();
    }

    public void DetermineBlock(List<GameObject> conditions)
    { 
        // the blocks will always come in this order: Below, Right, Above, Left
        for (int i = 0; i < conditions.Count; i++)
        {
            blockNames.Add(conditions[i].name);
            blockRotations.Add(conditions[i].transform.rotation);
        }
        RenameToMatchTileRules(blockNames, blockRotations);
        ResetLists();

    }

    private void RenameToMatchTileRules(List<string> blockNames, List<Quaternion> blockRotation)
    {

        for (int i = 0; i < blockNames.Count; i++)
        {
            if (blockNames[i] == aBlock.name)
                blockNames[i] = blockValues[0];

            if (blockNames[i] == bBlock.name)
            {
                if(blockRotation[i] == bBlock.transform.rotation)
                    blockNames[i] = blockValues[1];
                else if (blockRotation[i] == Quaternion.Euler(0, 0, 0))
                    blockNames[i] = blockValues[2];
                else if (blockRotation[i] == Quaternion.Euler(0, 270, 0))
                    blockNames[i] = blockValues[3];
                else if (blockRotation[i] == Quaternion.Euler(0, 180, 0))
                    blockNames[i] = blockValues[4];
            }

            if (blockNames[i] == cBlock.name)
            {
                if (blockRotation[i] == cBlock.transform.rotation)
                    blockNames[i] = blockValues[5];
                else if (blockRotation[i] == Quaternion.Euler(0, 0, 0))
                    blockNames[i] = blockValues[6];
            }

            if (blockNames[i] == dBlock.name)
            {
                if (blockRotation[i] == dBlock.transform.rotation)
                    blockNames[i] = blockValues[7];
                else if (blockRotation[i] == Quaternion.Euler(0, 0, 0))
                    blockNames[i] = blockValues[8];
                else if (blockRotation[i] == Quaternion.Euler(0, 270, 0))
                    blockNames[i] = blockValues[9];
                else if (blockRotation[i] == Quaternion.Euler(0, 180, 0))
                    blockNames[i] = blockValues[10];
            }

            if (blockNames[i] == eBlock.name)
            {
                if (blockRotation[i] == eBlock.transform.rotation)
                    blockNames[i] = blockValues[11];
                else if (blockRotation[i] == Quaternion.Euler(0, 0, 0))
                    blockNames[i] = blockValues[12];
                else if (blockRotation[i] == Quaternion.Euler(0, 270, 0))
                    blockNames[i] = blockValues[13];
                else if (blockRotation[i] == Quaternion.Euler(0, 180, 0))
                    blockNames[i] = blockValues[14];
            }

        }
        CalculateNewBlockType(blockNames);
    }

    private void CalculateNewBlockType(List<string> blockType) // Order incoming is: Bottom, Right, Top, left
    {
        // leftBlock, rightBlock, topBlock, and bottomBlock names are determined by hunters
        string bottomWall = blockType[0];
        string rightWall = blockType[1];
        string topWall = blockType[2];
        string leftWall = blockType[3];

        string newBlockType = "";
        // Order will be read as: Right, Top, Left, Bottom
        newBlockType += rightWall[2]; // does the newblock have a right wall on its left?                                              
        newBlockType += topWall[3]; // does the newblock have a top wall on its bottom?                                          
        newBlockType += leftWall[0]; // does the newblock have a left wall on its right?                                          
        newBlockType += bottomWall[1]; // does the newblock have a bottom wall on its top? 
  
        DeterminePossibleBlockGenerations(newBlockType);

    }

    private void DeterminePossibleBlockGenerations(string newBlockType)
    {
        string[] compatableBlocks;
        string finalKey;
        
        compatableBlocks = blockCompatabilityDictionary[newBlockType];
        finalKey = compatableBlocks[UnityEngine.Random.Range(0, compatableBlocks.Length)]; // pick index from the dictionary
        SendBlockInformationEV?.Invoke(finalKey);
    }
   
}


