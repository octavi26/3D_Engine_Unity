using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenGenerator : MonoBehaviour
{
    const int MAX = 100;
    public GameObject[,] Pixels = new GameObject[9 * MAX,16 * MAX];

    public GameObject pixel;

    public int rows = 9;
    public int colums = 16;

    private int lastRows = 9;
    private int lastColums = 16;

    private Vector3 position;
        
    // Start is called before the first frame update
    void Start()
    {
        position = gameObject.transform.position;
        GenerateScreen();
        ChangeScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if(ChangeScreen()){
            GenerateScreen();
        }
    }

    bool ChangeScreen(){
        bool change = false;
        if( lastColums != colums ) change = true;
        if( lastRows != rows ) change = true;
        lastColums = colums;
        lastRows = rows;
        return change;
    }

    void GenerateScreen(){
        Debug.Log("Restarting Screen");
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if(gameObj.name == "Pixel(Clone)")
            {
                Destroy(gameObj);
            }
        }

        if(colums == 1 || rows == 1) return;

        Vector3 pixelSize = new Vector3(16 / (float)colums, 9 / (float)rows, 1f);
        Vector3 firstPointPosition = new Vector3(position.x - 8 + pixelSize.x/2, position.y - 4.5f + pixelSize.y/2, -position.z);
        Vector3 horizontalVector = new Vector3(pixelSize.x,0f,0f);
        Vector3 verticalVector = new Vector3(0f,pixelSize.y,0f);

        for( int i = 0; i < rows; i++ )
            for( int j = 0; j < colums; j++ ){
                Vector3 newPosition = firstPointPosition + i * verticalVector + j * horizontalVector;
                Pixels[i,j] = Instantiate(pixel, newPosition, Quaternion.identity);
                Pixels[i,j].transform.localScale = pixelSize;
                Pixels[i,j].transform.SetParent(gameObject.transform);
            }
    }
}
