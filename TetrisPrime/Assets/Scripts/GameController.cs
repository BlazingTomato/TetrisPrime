using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using System.Linq;


public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Spawner spawn;
    [SerializeField] Grid tetrisGrid;
    [SerializeField] GameObject startBTN;

    public GameObject currentGroup, nextGroup, holdGroup, projectedGroup;

    bool[,] grid;
    Transform[] allBlocks;

    [SerializeField] float fallTime, fallTimePased = 0;

    [SerializeField] bool lockStart;
    

    void Start()
    {
        grid = new bool[10,24];
        allBlocks = new Transform[4];
        lockStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentGroup == null)
            return;

        if(Input.GetKeyDown(KeyCode.H))
            holdBlock();

        if(Input.GetKeyDown(KeyCode.RightArrow))
            moveBlock(1,0);
        
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            moveBlock(-1,0);

        if(Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.UpArrow))
            rotate(-90);
        
        if(Input.GetKeyDown(KeyCode.Z))
            rotate(90);

        if(Input.GetKeyDown(KeyCode.Space)){
            lockStart = false;
            while(moveBlock(0,-1));
            lockBlock();
        }

        if(Input.GetKeyDown(KeyCode.DownArrow)){
            moveBlock(0,-1);
        }
            
        
        fallTimePased += Time.deltaTime;

        if(fallTimePased >= fallTime){
            if(lockStart){
                lockStart = false;
                lockBlock();
            }
            fallTimePased = 0;
            if(!moveBlock(0,-1)){
                lockStart = true;
            }
        }

        makeProjectedGroup();
    }

    
    public void startGame(){
        currentGroup = spawn.putBlockOnGrid();
        nextGroup = spawn.putBlockOnNext();

        initializeGroupToGrid();

        startBTN.SetActive(false);
    }

    void holdBlock(){
        if(holdGroup == null){
            holdGroup = currentGroup;
            Destroy(currentGroup);
            holdGroup = spawn.putBlockOnHold(holdGroup);
            spawnBlock();
        }else{
            GameObject tempHoldGroup = holdGroup;
            GameObject tempCurrentGroup = currentGroup;

            Destroy(holdGroup);
            Destroy(currentGroup);
            
            currentGroup = spawn.putBlockOnGrid(tempHoldGroup);
            holdGroup = spawn.putBlockOnHold(tempCurrentGroup);
        }

        initializeGroupToGrid();
        
    }

    void spawnBlock(){
        currentGroup = nextGroup;
        currentGroup = spawn.putBlockOnGrid(currentGroup);
        Destroy(nextGroup);
        Destroy(projectedGroup);
        nextGroup = spawn.putBlockOnNext();
        
        initializeGroupToGrid();
    }

    void initializeGroupToGrid(){
        Transform[] allTransform = currentGroup.GetComponentsInChildren<Transform>();

        for(int i = 1; i < allTransform.Length; i++){
            allBlocks[i-1] = allTransform[i];
            
        }
    }

    Transform[] initializeGroupToGrid(GameObject group){
        Transform[] allTransform = group.GetComponentsInChildren<Transform>();
        Transform[] blocks = new Transform[4];

        for(int i = 1; i < allTransform.Length; i++){
            blocks[i-1] = allTransform[i];
        }

        return blocks;
    }

    void lockBlock(){

        foreach(Transform block in allBlocks){
            Point position = tetrisGrid.localSpacetoGrid(block);
            Debug.Log(transform.position + ": " + position.X + " " + position.Y);
            
            grid[position.X,position.Y] = true;
        }

        spawnBlock();
    }
    //Movement methods
    void rotate(double angle, int xOffset = 0, int yOffset = 0){
        bool canMove = true;
        // Convert the angle from degrees to radians
        double angleRadians = angle * Math.PI / 180;

        // Define the point of rotation
        Point pointOfRotation = tetrisGrid.localSpacetoGrid(currentGroup,xOffset,yOffset);

        // Define the object to rotate (a triangle with vertices at (1,1), (1,3), and (3,1))
        Point[] vertices = new Point[] {
            tetrisGrid.localSpacetoGrid(allBlocks[0],xOffset,yOffset),
            tetrisGrid.localSpacetoGrid(allBlocks[1],xOffset,yOffset),
            tetrisGrid.localSpacetoGrid(allBlocks[2],xOffset,yOffset),
            tetrisGrid.localSpacetoGrid(allBlocks[3],xOffset,yOffset)
        };

        // Construct the rotation matrix
        double[,] rotationMatrix = new double[,] {
            { Math.Cos(angleRadians), -Math.Sin(angleRadians) },
            { Math.Sin(angleRadians),  Math.Cos(angleRadians) }
        };

        // Translate the object so that the point of rotation is at the origin
        Point[] translatedVertices = vertices.Select(vertex => new Point(
            vertex.X - pointOfRotation.X,
            vertex.Y - pointOfRotation.Y
        )).ToArray();

        // Rotate the object
        Point[] rotatedVertices = translatedVertices.Select(vertex => new Point(
            (int)(vertex.X * rotationMatrix[0, 0] + vertex.Y * rotationMatrix[0, 1]),
            (int)(vertex.X * rotationMatrix[1, 0] + vertex.Y * rotationMatrix[1, 1])
        )).ToArray();

        // Translate the object back to its original position

        Point[] finalVerticies = rotatedVertices.Select(vertex => new Point(
            vertex.X + pointOfRotation.X,
            vertex.Y + pointOfRotation.Y
        )).ToArray();

        foreach(Point block in finalVerticies){
            try{
                if(grid[block.X,block.Y]) {
                    if(Math.Abs(xOffset) + Math.Abs(yOffset) > 0) return;

                    rotate(angle, xOffset + 1, yOffset);
                    rotate(angle, xOffset - 1, yOffset);
                    rotate(angle, xOffset, yOffset + 1);
                }
            }catch(Exception){
                canMove = false;

                if(Math.Abs(xOffset) + Math.Abs(yOffset) > 0) return;

                rotate(angle, xOffset + 1, yOffset);
                rotate(angle, xOffset - 1, yOffset);
                rotate(angle, xOffset, yOffset + 1);
            
            }
        }
        
        if(canMove){
            moveBlock(xOffset,yOffset);
            tetrisGrid.rotateBlock(currentGroup, angle);
            moveBlock(-xOffset,-yOffset);
        }

        makeProjectedGroup();
    }

    bool moveBlock(int xOffset, int yOffset){
        bool canMove = true;
        
        foreach(Transform block in allBlocks){

            Point position = tetrisGrid.localSpacetoGrid(block);
            //Debug.Log(transform.position + ": " + position.X + " " + position.Y);
            

            try{
                if(grid[position.X + xOffset,position.Y + yOffset]) canMove = false;
            }catch(Exception){
                canMove = false;
            }
            
        }

        if(canMove) tetrisGrid.moveBlock(currentGroup, xOffset, yOffset);

        makeProjectedGroup();

        return canMove;
    }

    public void makeProjectedGroup(){
        GameObject.Destroy(projectedGroup);

        int i = 100;

        foreach(Transform block in allBlocks){

            Point point = tetrisGrid.localSpacetoGrid(block);

            bool canMove = true;
            int j = 0;

            while(canMove){
                try{
                    if(grid[point.X,point.Y-j]) canMove = false;
                }catch(Exception){
                    canMove = false;
                }
                j++;
            }
            
            i = Math.Min(i,j);
        }

        
        Vector2 position = currentGroup.GetComponent<Transform>().position + new Vector3(0,-.4f*(i-2),0);

        projectedGroup = GameObject.Instantiate(currentGroup,position,currentGroup.GetComponent<Transform>().rotation);

        foreach(SpriteRenderer spriteRenderer in projectedGroup.GetComponentsInChildren<SpriteRenderer>()){
            spriteRenderer.color -= new Color32(0,0,0,110);
        }
    } 

}
