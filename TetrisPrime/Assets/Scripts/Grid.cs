using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject borderPrefab;
    [SerializeField] new Camera camera;
    [SerializeField] GameObject borderParent;
    [SerializeField] GameObject whiteBorderPrefab;
    
    int gridLength = 20; 
    int gridWidth = 10; 
    int holdLength = 5; 
    int gapLength = 1;



    [ContextMenu("Make Border")]
    // start at -8,4
    void makeBorder(){

        foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Border")){
            DestroyImmediate(gameObject);
        }

        int size = (int)camera.orthographicSize;
        float scale = size * .8f * .1f; 

        float xStart = size * -1f;
        float yStart = size * .8f;

        float xStartForHold = xStart - (holdLength + gapLength) * scale;
        float xStartForNext = xStart + ((gridWidth + gapLength) * scale);
    
        //horizontal borders
        for(int i = 0; i < gridWidth; i++){
            GameObject border = Instantiate(borderPrefab,new Vector3(xStart + i*scale, yStart, 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab,new Vector3(xStart + i*scale, -yStart, 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
        }

        //vertical borders
        for(int i = 0; i < gridLength; i++){
            GameObject border = Instantiate(borderPrefab, new Vector3(xStart, yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab, new Vector3(xStart + (gridWidth * scale), yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
        }


        //horizontal white borders
        for(int j = 0; j < gridLength - 1; j++){
            for(int i = 0; i < gridWidth; i++){
                GameObject border = Instantiate(whiteBorderPrefab,new Vector3(xStart + i*scale, yStart - (scale * (j + 1)), 0), Quaternion.Euler(0,0,90));
                initializeBorder(border,scale);            
            }
        }

        //vertical white borders
        for(int j = 0; j < gridWidth - 1; j++){
            for(int i = 0; i < gridLength; i++){
                GameObject border = Instantiate(whiteBorderPrefab, new Vector3(xStart + (scale * (j+1)), yStart - i*scale, 0), Quaternion.identity);
                initializeBorder(border,scale);            
            }
        }

        //horizontal hold Borders
        for(int i = 0; i < holdLength; i++){
            GameObject border = Instantiate(borderPrefab,new Vector3(xStartForHold + i*scale, yStart, 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab,new Vector3(xStartForHold + i*scale, yStart - (holdLength * scale), 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
        }

        //vertical hold borders
        for(int i = 0; i < holdLength; i++){
            GameObject border = Instantiate(borderPrefab, new Vector3(xStartForHold, yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab, new Vector3(xStartForHold + (holdLength * scale), yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
        }

        //horizontal next Borders
        for(int i = 0; i < holdLength; i++){
            GameObject border = Instantiate(borderPrefab,new Vector3(xStartForNext + i*scale, yStart, 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab,new Vector3(xStartForNext + i*scale, yStart - (holdLength * scale), 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
        }

        //vertical next borders
        for(int i = 0; i < holdLength; i++){
            GameObject border = Instantiate(borderPrefab, new Vector3(xStartForNext, yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab, new Vector3(xStartForNext + (holdLength * scale), yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
        }
        
        
    }



    void initializeBorder(GameObject border, float scale){
        border.transform.parent = borderParent.transform;
        border.GetComponent<Transform>().localScale = new Vector3(1,borderPrefab.GetComponent<Transform>().localScale.y * scale,1);
    }
}
