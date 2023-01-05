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
    [SerializeField] GameObject textBox;

    public GameObject currentGroup, nextGroup, holdGroup, projectedGroup;

    GameObject[,] grid;
    GameObject[] allBlocks;

    [SerializeField] float fallTime, fallTimePased = 0;

    [SerializeField] bool lockStart;
    int score;
    int level;
    int totalRows;
    

    void Start()
    {
        grid = new GameObject[24,10];
        grid.Equals(null);
        allBlocks = new GameObject[4];
        lockStart = false;
        score = 0;
        level = 0;
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

        if(fallTimePased >= fallTime*Math.Pow(Math.E,.3*level)){
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

    #region Spawn Methods
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
        allBlocks = new GameObject[4];
        for(int i = 0; i < 4; i++){
            allBlocks[i] = currentGroup.transform.GetChild(i).gameObject;
        }
    }

    #endregion

    #region Movement Methods
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
                if(grid[block.Y,block.X] != null) {
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
        
        foreach(GameObject block in allBlocks){

            Point position = tetrisGrid.localSpacetoGrid(block);
            //Debug.Log(transform.position + ": " + position.X + " " + position.Y);
            

            try{
                if(grid[position.Y + yOffset,position.X + xOffset] != null) canMove = false;
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

        foreach(GameObject block in allBlocks){

            Point point = tetrisGrid.localSpacetoGrid(block);

            bool canMove = true;
            int j = 0;

            while(canMove){
                try{
                    if(grid[point.Y-j,point.X] != null) canMove = false;
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

    #endregion
    
    #region Game Logic Methods

    bool isRowFull(int y){
        for(int i = 0; i < grid.GetLength(1); i++){
            if(grid[y,i] == null)
                return false;
        }

        return true;
    }

    void decreaseRowsAbove(int y){
        for(int i = y; i < grid.GetLength(0); i++){
            decreaseRow(i);
        }
    }

    void decreaseRow(int y){
        for(int i = 0; i < grid.GetLength(1); i++){
            if(grid[y,i] != null){
                grid[y-1,i] = grid[y,i];
                grid[y,i] = null;

                tetrisGrid.moveBlock(grid[y-1,i],0,-1);
            }
        }
    }

    void deleteRow(int y){
        for(int i = 0; i < grid.GetLength(1); i++){
            Destroy(grid[y,i].gameObject);
            grid[y,i] = null;
        }
    }

    int deleteFullRows(){
        int rowsDeleted = 0;
        for(int i = 0; i < grid.GetLength(0); i++){
            if(isRowFull(i)){
                deleteRow(i);
                decreaseRowsAbove(i+1);
                rowsDeleted++;
                i--;
            }
        }

        return rowsDeleted;
    }

    void lockBlock(){

        foreach(GameObject block in allBlocks){
            Point position = tetrisGrid.localSpacetoGrid(block);
            //Debug.Log(transform.position + ": " + position.X + " " + position.Y);
            
            grid[position.Y,position.X] = block;
        }

        addScore(deleteFullRows());
        spawnBlock();
    }

    void addScore(int rowsDeleted){
        totalRows += rowsDeleted;

        if(level*10 + 10 > totalRows){
            totalRows = 0;
            level++;
        }

        if(rowsDeleted == 1) score += 40 *(level + 1);

        if(rowsDeleted == 2) score += 100 * (level + 1);

        if(rowsDeleted == 3) score += 300 * (level + 1);

        if(rowsDeleted == 4) score += 1200 * (level + 1);

        if(rowsDeleted > 0)
            updateScore();
    }

    void updateScore(){
        String s = "Score: " + score + "\n" + "Level: " + level;
        textBox.GetComponent<TextMesh>().text = s;
    }
    
    #endregion

}