using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    System.Random rnd = new System.Random();

    [SerializeField] GameObject tile;
    [SerializeField] GameObject[] backTile;
    [SerializeField] GameObject mineTile;
    [SerializeField] Transform parent;
    public List<GameObject> tiles = new List<GameObject>();
    public List<GameObject> backTiles = new List<GameObject>();
    public List<int> emptyPos = new List<int>();
    public List<int> minesPos = new List<int>();
    DataStorage dataStorage;
    [SerializeField] TextMeshProUGUI flagCounter;
    public int flagCount;
    public int openCount = 0;
    public bool isGameOver = false;
    public bool isWin = false;
    public bool isMultiple = false;
    public bool canPause = true;

    void Start()
    {
        dataStorage = GameObject.FindWithTag("DataStorage").GetComponent<DataStorage>();
        chooseMinesPosition();
        createGameArea(dataStorage.gameArea[0], dataStorage.gameArea[1]);
    }

    void Update()
    {
        checkTiles();
    }

    void createGameArea(int row, int column)
    {
        int remainR = 800 % row;
        int remainC = 800 % column;
        int edge;
        int gapR = 0;
        int gapC = 0;
        if (row > column)
        {
            edge = ((800 - remainR) / row);
            gapC = (row - column) * edge;
        }
        else
        {
            edge = ((800 - remainC) / column);
            gapR = (column - row) * edge;
        }
        tile.GetComponent<RectTransform>().sizeDelta = new Vector2 (edge, edge);
        mineTile.GetComponent<RectTransform>().sizeDelta = new Vector2 (edge, edge);
        foreach (var bt in backTile)
        {
            bt.GetComponent<RectTransform>().sizeDelta = new Vector2 (edge, edge);
        }
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                tiles.Add(Instantiate (tile, new Vector3(((remainC+gapC)/2) + (c * edge),((-remainR-gapR)/2) + (r * -edge), 0) , Quaternion.identity));
                tiles[c + r * column].transform.SetParent(parent, false);
                tiles[c + r * column].GetComponent<TileControl>().row = r;
                tiles[c + r * column].GetComponent<TileControl>().column = c;
                tiles[c + r * column].GetComponent<TileControl>().tileID = c + r * column;
                if (minesPos.Contains(c + r * column))
                {
                    backTiles.Add(Instantiate (mineTile, new Vector3(((remainC+gapC)/2) + (c * edge),((-remainR-gapR)/2) + (r * -edge), 0) , Quaternion.identity));
                    backTiles[c + r * column].transform.SetParent(parent, false);
                }
                else
                {
                    int nbc = NearByMineCount(c, r, column-1, row-1, c + r * column);
                    backTiles.Add(Instantiate (backTile[nbc], new Vector3(((remainC+gapC)/2) + (c * edge),((-remainR-gapR)/2) + (r * -edge), 0) , Quaternion.identity));
                    backTiles[c + r * column].transform.SetParent(parent, false);
                    if (nbc == 0)
                    {
                        emptyPos.Add(c + r * column);
                    }
                }
                backTiles[c + r * column].SetActive(false);
            }
        }
    }

    int NearByMineCount(int column, int row, int c, int r ,  int index)
    {
        int count = 0;
        if (row != 0 && minesPos.Contains(index - dataStorage.gameArea[1])){count++;}
        if (column != 0 && row != 0 && minesPos.Contains(index - dataStorage.gameArea[1] - 1)){count++;}
        if (column != c && row != 0 && minesPos.Contains(index - dataStorage.gameArea[1] + 1)){count++;}
        if (column != 0 && minesPos.Contains(index - 1)){count++;}
        if (column != c && minesPos.Contains(index + 1)){count++;}
        if (row != r && minesPos.Contains(index + dataStorage.gameArea[1])){count++;}
        if (column != 0 && row != r && minesPos.Contains(index + dataStorage.gameArea[1] - 1)){count++;}
        if (column != c && row != r && minesPos.Contains(index + dataStorage.gameArea[1] + 1)){count++;}
        return count;
    }

    void chooseMinesPosition()
    {
        for (int i = 0; i < dataStorage.mineCount; i++)
        {
            minesPos.Add(randomInt());
        }
    }

    int randomInt()
    {
        while (true)
        {
            int random = rnd.Next(dataStorage.gameArea[0] * dataStorage.gameArea[1]);
            if (!minesPos.Contains(random))
            {
                return random;
            }
        }
    }

    public void updateFlagCount()
    {
        bool isNeg = flagCount.ToString().Contains("-");
        string newCount = "";
        string current;

        if (isNeg){current = flagCount.ToString().Substring(1);}
        else {current = flagCount.ToString();}

        for (int i = 0; i < 3 - current.Length; i++)
        {
            newCount += "0";
        }
        if (isNeg){newCount = "-"  + newCount;}
        flagCounter.text = newCount + current;
    }

    public void checkTiles()
    {
        int fCount = 0;
        foreach (var tile in tiles)
        {
            if (tile.GetComponent<TileControl>().state == 0){fCount++;}
        }
        flagCount = dataStorage.mineCount - fCount;
        updateFlagCount();
    }

    public void checkWin()
    {
        isWin = openCount + dataStorage.mineCount == dataStorage.gameArea[0] * dataStorage.gameArea[1];
    }
}
