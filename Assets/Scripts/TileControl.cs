using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileControl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject[] TileImage;

    GameController gameController;
    DataStorage dataStorage;
    SceneControl sceneControl;
    AudioControl audioControl;

    public int state = -1;
    public int row;
    public int column;
    public int tileID;
    public bool isOpen = false;
    bool isClicked = false;

    void Start()
    {
        gameController = GameObject.FindWithTag("EventSys").GetComponent<GameController>();
        dataStorage = GameObject.FindWithTag("DataStorage").GetComponent<DataStorage>();
        sceneControl = GameObject.FindWithTag("EventSys").GetComponent<SceneControl>();
        audioControl = GameObject.FindWithTag("AudioSys").GetComponent<AudioControl>();
        gameController.flagCount = dataStorage.mineCount;
        gameController.updateFlagCount();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (state == -1 && eventData.button == PointerEventData.InputButton.Left)
        {
            isClicked = true;
            openTile();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            state++;
            updateTileState();
        }
    }

    void updateTileState()
    {
        if (state == TileImage.Length)
        {
            state = -1;
        }
        for (int i = 0; i < TileImage.Length; i++)
        {
            if (i == state)
            {
                TileImage[i].SetActive(true);
            }
            else
            {
                TileImage[i].SetActive(false);
            }
        }
        if (dataStorage.soundON)
        {
            if (state == 0)
            {
                audioControl.FlaggedSound.Play();
            }
            else
            {
                audioControl.UnFlaggedSound.Play();
            }
        }
    }

    public void openTile()
    {
        if (!isOpen)
        {
            gameController.openCount++;
            gameController.backTiles[tileID].SetActive(true);
            gameController.tiles[tileID].GetComponent<TileControl>().isOpen = true;
            state = 2;
            if (gameController.minesPos.Contains(tileID))
            {
                Time.timeScale = 0;
                gameController.canPause = false;
                gameController.openCount--;
                sceneControl.enableEventBlocker();
                if (dataStorage.soundON){audioControl.MineExplosionSound.Play();}
                sceneControl.revealMines();
            }
            else
            {
                gameController.checkWin();
                if (gameController.isWin){sceneControl.enterWin();}
                else if (gameController.emptyPos.Contains(tileID))
                {
                    gameController.emptyPos.Remove(tileID);
                    int r = dataStorage.gameArea[0] - 1;
                    int c = dataStorage.gameArea[1] - 1;
                    if (row != 0 && column != 0)
                    {gameController.isMultiple = true; gameController.tiles[tileID - dataStorage.gameArea[1] - 1].GetComponent<TileControl>().openTile();}

                    if (row != 0)
                    {gameController.isMultiple = true; gameController.tiles[tileID - dataStorage.gameArea[1]].GetComponent<TileControl>().openTile();}

                    if (row != 0 && column != c)
                    {gameController.isMultiple = true; gameController.tiles[tileID - dataStorage.gameArea[1] + 1].GetComponent<TileControl>().openTile();}

                    if (column != 0)
                    {gameController.isMultiple = true; gameController.tiles[tileID - 1].GetComponent<TileControl>().openTile();}

                    if (column != c)
                    {gameController.isMultiple = true; gameController.tiles[tileID + 1].GetComponent<TileControl>().openTile();}

                    if (row != r && column != 0)
                    {gameController.isMultiple = true; gameController.tiles[tileID + dataStorage.gameArea[1] - 1].GetComponent<TileControl>().openTile();}

                    if (row != r)
                    {gameController.isMultiple = true; gameController.tiles[tileID + dataStorage.gameArea[1]].GetComponent<TileControl>().openTile();}

                    if (row != r && column != c)
                    {gameController.isMultiple = true; gameController.tiles[tileID + dataStorage.gameArea[1] + 1].GetComponent<TileControl>().openTile();}
                }
                if (dataStorage.soundON)
                {
                    if (isClicked && gameController.isMultiple){gameController.isMultiple = false; audioControl.MultipleDigSound.Play();}
                    else {audioControl.SingleDigSound.Play();}
                }
            }
        }
    }
}
