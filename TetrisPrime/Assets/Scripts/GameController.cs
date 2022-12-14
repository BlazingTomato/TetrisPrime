using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Spawner spawn;

    public GameObject currentGroup, nextGroup, holdGroup;


    int[,] grid;

    //float fallTime = 1, fallTimePased = 0;


    void Start()
    {
        grid = new int[24,10];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
            holdBlock();
    }

    public void startGame(){
        currentGroup = spawn.putBlockOnGrid();
        nextGroup = spawn.putBlockOnNext();
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
    }
    //Tetris Block movement methods

    

}
