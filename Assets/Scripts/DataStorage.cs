using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    public int[] gameArea;
    public int mineCount;
    public bool soundON;
    
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        if (GameObject.FindGameObjectsWithTag("DataStorage").Length == 2){Destroy(transform.gameObject);}
    }

    public void setMineCount(float mineRate)
    {
        mineCount = Mathf.RoundToInt(gameArea[0] * gameArea[1] * mineRate);
    }
}
