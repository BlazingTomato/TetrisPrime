using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBorder : MonoBehaviour
{
    [SerializeField] GameObject borderPrefab;
    [SerializeField] Camera camera;
    [SerializeField] GameObject borderParent;
    [SerializeField] GameObject whiteBorderPrefab;
    
    [ContextMenu("Make Border")]

    // start at -8,4
    void makeBorder(){

        Destroy(GameObject.FindGameObjectWithTag("Border"));

        int size = (int)camera.orthographicSize;
        Debug.Log(size);
        float scale = size * .8f * .1f; 

        float xStart = size * -1.6f;
        float yStart = size * .8f;
    
        //horizontal borders
        for(int i = 0; i < 10; i++){
            GameObject border = Instantiate(borderPrefab,new Vector3(xStart + i*scale, yStart, 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab,new Vector3(xStart + i*scale, -yStart, 0), Quaternion.Euler(0,0,90));
            initializeBorder(border,scale);
        }

        //vertical borders
        for(int i = 0; i < 20; i++){
            GameObject border = Instantiate(borderPrefab, new Vector3(xStart, yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
            border = Instantiate(borderPrefab, new Vector3(xStart + (10 * scale), yStart - i*scale, 0), Quaternion.identity);
            initializeBorder(border,scale);
        }


        //horizontal white borders
        for(int j = 0; j < 19; j++){
            for(int i = 0; i < 10; i++){
                GameObject border = Instantiate(whiteBorderPrefab,new Vector3(xStart + i*scale, yStart - (scale * (j + 1)), 0), Quaternion.Euler(0,0,90));
                initializeBorder(border,scale);            
            }
        }

        //vertical white borders
        for(int j = 0; j < 9; j++){
            for(int i = 0; i < 20; i++){
                GameObject border = Instantiate(whiteBorderPrefab, new Vector3(xStart + (scale * (j+1)), yStart - i*scale, 0), Quaternion.identity);
                initializeBorder(border,scale);            
            }
        }
    }

    void initializeBorder(GameObject border, float scale){
        border.transform.parent = borderParent.transform;
        border.GetComponent<Transform>().localScale = new Vector3(1,borderPrefab.GetComponent<Transform>().localScale.y * scale,1);
    }
}
