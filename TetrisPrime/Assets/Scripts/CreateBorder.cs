using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBorder : MonoBehaviour
{
    [SerializeField] GameObject borderPrefab;
    [SerializeField] Camera camera;
    [SerializeField] GameObject borderParent;
    
    [ContextMenu("Make Border")]

    // start at -8,4
    void makeBorder(){
        int size = (int)camera.orthographicSize;
        Debug.Log(size);
        float scale = size * .8f * .1f; 

        float xStart = size * -1.6f;
        float yStart = size * .8f;

        borderPrefab.GetComponent<Transform>().localScale = new Vector3(1,borderPrefab.GetComponent<Transform>().localScale.y * scale,1);

        //horizontal borders
        for(int i = 0; i < 10; i++){
            GameObject border = Instantiate(borderPrefab,new Vector3(xStart + i*scale, yStart, 0), Quaternion.Euler(0,0,90));
            border.transform.parent = borderParent.transform;
            border = Instantiate(borderPrefab,new Vector3(xStart + i*scale, -yStart, 0), Quaternion.Euler(0,0,90));
            border.transform.parent = borderParent.transform;
        }

        //vertical borders
        for(int i = 0; i < 20; i++){
            GameObject border = Instantiate(borderPrefab, new Vector3(xStart, yStart - i*scale, 0), Quaternion.identity);
            border.transform.parent = borderParent.transform;
            border = Instantiate(borderPrefab, new Vector3(xStart + (10 * scale), yStart - i*scale, 0), Quaternion.identity);
            border.transform.parent = borderParent.transform;
        }
    }
}
