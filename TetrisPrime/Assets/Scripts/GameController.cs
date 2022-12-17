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

    public GameObject currentGroup, nextGroup, holdGroup;

    bool[,] grid;
    Transform[] allBlocks;

    [SerializeField] float fallTime, fallTimePased = 0;

    bool lockStart;
    

    void Start()
    {
        grid = new bool[10,24];
        allBlocks = new Transform[4];
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

        
        fallTimePased += Time.deltaTime;

        if(fallTimePased >= fallTime){
            fallTimePased = 0;
            moveBlock(0,-1);
        }
    }

    
    public void startGame(){
        currentGroup = spawn.putBlockOnGrid();
        nextGroup = spawn.putBlockOnNext();

        initializeGroupToGrid();
    }

    void holdBlock(){
        if(holdGroup == null){
            holdGroup = currentGroup;
            spawnBlock();
        }else{
            GameObject tempHoldGroup = holdGroup;
            GameObject tempCurrentGroup = currentGroup;

            Destroy(holdGroup);
            Destroy(currentGroup);
            
            currentGroup = spawn.putBlockOnGrid(tempHoldGroup);
            holdGroup = tempCurrentGroup;
        }
        holdGroup = spawn.putBlockOnHold(holdGroup);
    }

    void spawnBlock(){
        Destroy(currentGroup);
        currentGroup = nextGroup;
        currentGroup = spawn.putBlockOnGrid(currentGroup);
        Destroy(nextGroup);
        nextGroup = spawn.putBlockOnNext();
        
        initializeGroupToGrid();
    }

    void initializeGroupToGrid(){
        Transform[] allTransform = currentGroup.GetComponentsInChildren<Transform>();

        for(int i = 1; i < allTransform.Length; i++){
            allBlocks[i-1] = allTransform[i];
            //Debug.Log(groupPosition[i-1].X + " " + groupPosition[i-1].sY);
        }
    }

    //Movement methods
    //Figure out edge case shit
    //If angle is positive, try moving left then up
    //If angle is negative, try moving right then up
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
    }

    
    void moveBlock(int xOffset, int yOffset){
        bool canMove = true;
        
        foreach(Transform block in allBlocks){

            Point position = tetrisGrid.localSpacetoGrid(block);

            try{
                if(grid[position.X + xOffset,position.Y + yOffset]) canMove = false;
            }catch(Exception){
                canMove = false;
            }
            
        }

        if(canMove) tetrisGrid.moveBlock(currentGroup, xOffset, yOffset);
        
    }
    

}
