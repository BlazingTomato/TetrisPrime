using UnityEngine;
using System.Linq;
using System;

public class Spawner : MonoBehaviour
{
    [SerializeField] 
    GameObject[] blockPrefabs;

    [SerializeField] GameObject gridLocation;
    [SerializeField] GameObject nextLocation;
    [SerializeField] GameObject holdLocation;
    

    public GameObject getBlockPrefab(){
        return blockPrefabs[UnityEngine.Random.Range(0,blockPrefabs.Length)];
    }

    GameObject InstantiateBlock(GameObject newBlock, GameObject location, bool offSet = true){
        newBlock.transform.parent = location.transform;
        newBlock.GetComponent<Transform>().position = location.GetComponent<Transform>().position;

        if(offSet)
            newBlock.GetComponent<Transform>().localPosition = getOffset(newBlock);
        else
            newBlock.GetComponent<Transform>().localPosition = new Vector3(0,0,0);

        return newBlock;
    }

    public GameObject putBlockOnGrid(){
        GameObject newBlock = Instantiate(getBlockPrefab(), new Vector3(0,0,0),Quaternion.identity);
        return InstantiateBlock(newBlock, gridLocation, false);
    }

    public GameObject putBlockOnGrid(GameObject blockPrefab){
        GameObject newBlock = Instantiate(blockPrefab,new Vector3(0,0,0),Quaternion.identity);
        return InstantiateBlock(newBlock, gridLocation, false);
    }


    public GameObject putBlockOnNext(){
        GameObject newBlock = Instantiate(getBlockPrefab(), new Vector3(0,0,0),Quaternion.identity);
        return InstantiateBlock(newBlock, nextLocation);
    }

    public GameObject putBlockOnHold(GameObject holdBlock){
        GameObject newBlock = Instantiate(holdBlock, new Vector3(0,0,0),Quaternion.identity);
        return InstantiateBlock(newBlock, holdLocation);
    }

    Vector3 getOffset(GameObject block){
        Transform[] blocks = block.GetComponentsInChildren<Transform>();


        float[] xValues = new float[blocks.Length];
        float[] yValues = new float[blocks.Length];
        
        for(int i = 0; i < blocks.Length; i++){
            xValues[i] = blocks[i].localPosition.x;
            yValues[i] = blocks[i].localPosition.y;
        }

        float y = (Math.Abs(yValues.Max()) - Math.Abs(yValues.Min()))/2;
        float x = (Math.Abs(xValues.Max()) - Math.Abs(xValues.Min()))/2;

        //Debug.Log(block.name + " " + x + " " + y);

        return new Vector3(-x,-y,0);
    }

    
}
