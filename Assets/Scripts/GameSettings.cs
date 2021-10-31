using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Scroll_Input;
    [SerializeField] GameObject CustomArea_Input;
    [SerializeField] TMP_InputField Custom_InputLeft;
    [SerializeField] TMP_InputField Custom_InputRight;

    [SerializeField] Slider Mine_Slider;
    [SerializeField] TextMeshProUGUI MinePercentage;

    DataStorage dataStorage;
    [SerializeField] SceneControl sceneControl;

    bool isCustom = false;
    string validInput = "0123456789";
    int minEdge = 5;
    int maxEdge = 30;


    void Start()
    {
        dataStorage = GameObject.FindWithTag("DataStorage").GetComponent<DataStorage>();
        updateGameArea();
    }
    public void updateGameArea()
    {
        if (Scroll_Input.text == "Choose Custom Area")
        {
            CustomArea_Input.SetActive(true);
            if (Custom_InputLeft.text.Length == 0)
            {
                dataStorage.gameArea[0] = minEdge;
            }
            else
            {
                dataStorage.gameArea[0] = System.Convert.ToInt32(Custom_InputLeft.text);
            }
            if (Custom_InputRight.text.Length == 0)
            {
                dataStorage.gameArea[1] = minEdge;
            }
            else
            {
                dataStorage.gameArea[1] = System.Convert.ToInt32(Custom_InputRight.text);
            }
            isCustom = true;
        }
        else
        {
            CustomArea_Input.SetActive(false);
            dataStorage.gameArea[0] = System.Convert.ToInt32(Scroll_Input.text.Split(char.Parse("x"))[0]);
            dataStorage.gameArea[1] = dataStorage.gameArea[0];
            isCustom = false;
        }
    }

    public void checkInput(TMP_InputField input)
    {
        if (input.text.Length != 0)
        {
            string newInput = "";
            foreach (var cha in input.text)
                {
                    if (validInput.Contains(cha.ToString()))
                    {
                        newInput += cha;
                    }
                }
            input.text = newInput;
            if (input.text.Length > 2)
            {
                input.text = input.text.Substring(0, input.text.Length - 1);
            } 
        }
    }

    public void checkOnDeselect(TMP_InputField input)
    {
        if (input.text.Length == 0 || (input.text.Length == 1 && System.Convert.ToInt32(input.text) < minEdge))
        {
            input.text = minEdge.ToString();
        } 
        if (input.text.Length == 2 && System.Convert.ToInt32(input.text) > maxEdge)
        {
            input.text = maxEdge.ToString();
        }
    }

    public void updateMinePercentage()
    {
        string percent = (Mine_Slider.value * 100).ToString();
        for (int i = 0; i < percent.Length; i++)
        {
            if (percent.Substring(i, 1) == "." || percent.Substring(i, 1) == ",")
            {
                if (percent.Substring(i + 1, 1) == "0")
                {
                    percent = percent.Substring(0, i);
                }
                else
                {
                    percent = percent.Substring(0, i + 2);
                }
                break;
            }

        }
        MinePercentage.text = "%" + percent;
    }

    public void startGame()
    {
        if (isCustom && Custom_InputLeft.text.Length != 0 && Custom_InputRight.text.Length != 0)
        {
            dataStorage.gameArea[0] = System.Convert.ToInt32(Custom_InputLeft.text);
            dataStorage.gameArea[1] = System.Convert.ToInt32(Custom_InputRight.text);
        }
        dataStorage.setMineCount(Mine_Slider.value);
        sceneControl.startGame();
    }

}
