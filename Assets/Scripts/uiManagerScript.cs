using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class uiManagerScript : MonoBehaviour
{
    bool isDown = false;
    int width, height, p1Index, p2Index, easyPlayed, mediumPlayed, hardPlayed, easyWon, mediumWon, hardWon;
    float distance, angle;
    public GameObject menu, settingsMenu, playMenu, statisticsMenu, helpMenu, Line, circles;
    public Text wGrid, hGrid, easyStat, mediumStat, hardStat;
    public Sprite[] icons;
    public Image p1Icon, p2Icon;
    Vector2 initPos, currentPos;
    RaycastHit2D hit;

    void Start()
    {
        stats();

        p1Index = PlayerPrefs.GetInt("p1Icon", 0);
        p2Index = PlayerPrefs.GetInt("p2Icon", 1);
        p1Icon.sprite = icons[p1Index];
        p2Icon.sprite = icons[p2Index];

        width = PlayerPrefs.GetInt("wGrid", 6);
        height = PlayerPrefs.GetInt("hGrid", 6);
        wGrid.text = width.ToString();
        hGrid.text = height.ToString();
    }
    
    void stats()
    {
        easyPlayed = PlayerPrefs.GetInt("easyPlayed", 0);
        mediumPlayed = PlayerPrefs.GetInt("mediumPlayed", 0);
        hardPlayed = PlayerPrefs.GetInt("hardPlayed", 0);

        easyWon = PlayerPrefs.GetInt("easyWon", 0);
        mediumWon = PlayerPrefs.GetInt("mediumWon", 0);
        hardWon = PlayerPrefs.GetInt("hardWon", 0);
 
        float easyPer = easyPlayed == 0 ? 0 : ((float)easyWon / (float)easyPlayed) * 100;
        float mediumPer = mediumPlayed == 0 ? 0 : ((float)mediumWon / (float)mediumPlayed) * 100;
        float hardPer = hardPlayed == 0 ? 0 : ((float)hardWon / (float)hardPlayed) * 100;

        easyStat.text = "AI Difficulty : Easy    Games Played : " + easyPlayed +
                        "    Games Won : " + easyWon + " (" + easyPer.ToString("F2") + "%)";
        mediumStat.text = "AI Difficulty : Medium    Games Played : " + mediumPlayed +
                          "    Games Won : " + mediumWon + " (" + mediumPer.ToString("F2") + "%)";
        hardStat.text = "AI Difficulty : Hard    Games Played : " + hardPlayed +
                        "    Games Won : " + hardWon + " (" + hardPer.ToString("F2") + "%)";

    }

    public void play()
    {
        playMenu.SetActive(true);
        menu.SetActive(false);
        circles.SetActive(true);
        Line.SetActive(true);
    }

    public void settings()
    {
        menu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void statistics()
    {
        statisticsMenu.SetActive(true);
        menu.SetActive(false);
    }

    public void help()
    {
        helpMenu.SetActive(true);
        menu.SetActive(false);
    }

    public void backToMenu()
    {
        playMenu.SetActive(false);
        menu.SetActive(true);
        settingsMenu.SetActive(false);
        statisticsMenu.SetActive(false);
        circles.SetActive(false);
        Line.SetActive(false);
        helpMenu.SetActive(false);
    }

    public void changeWidth(int value)
    {
        width += value;
        if (width >= 4 && width <= 6)
        {
            wGrid.text = width.ToString();
            PlayerPrefs.SetInt("wGrid", width);
        }
        else
        {
            width -= value;
        }
    }

    public void changeHeight(int value)
    {
        height += value;
        if (height >= 4 && height <= 6)
        {
            hGrid.text = height.ToString();
            PlayerPrefs.SetInt("hGrid", height);
        }
        else
        {
            height -= value;
        }
    }

    public void changeP1Icon(int value)
    {
        p1Index += value;
        p1Index = p1Index < 0 ? icons.Length - 1 : p1Index;
        p1Index = p1Index > icons.Length - 1 ? 0 : p1Index;
        p1Index = p1Index == p2Index ? p1Index + value : p1Index;
        PlayerPrefs.SetInt("p1Icon", p1Index);
        p1Icon.sprite = icons[p1Index];
    }

    public void changeP2Icon(int value)
    {
        p2Index += value;
        p2Index = p2Index < 0 ? icons.Length - 1 : p2Index;
        p2Index = p2Index > icons.Length - 1 ? 0 : p2Index;
        p2Index = p1Index == p2Index ? p2Index + value : p2Index;
        PlayerPrefs.SetInt("p2Icon", p2Index);
        p2Icon.sprite = icons[p2Index];
    }

    public void quitGame()
    {
        Application.Quit();
    }

	void Update ()
    {
	    if (Input.GetMouseButtonDown(0))
        {
            initPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(initPos, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.transform.position == Vector3.zero)
                {
                    isDown = true;
                    Line.transform.position = Vector3.zero;
                }
            }
        }
        if (isDown)
        {
            createLine();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDown = false;
            if (distance < 1)
            {
                Line.transform.localScale = new Vector3(0.25f, 0.25f, 1);
            }
        }
	}

    void createLine()
    {
        currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = MathScript.getDistance(initPos, currentPos);
        angle = MathScript.getAngle(initPos, currentPos);
        distance = distance > 1 ? 1 : distance;
        Line.transform.localScale = new Vector3(distance * 2, 0.25f, 1);
        Line.transform.eulerAngles = new Vector3(0, 0, angle);
        if (distance == 1)
        {
            angle = (int)angle / 90;
            switch ((int)angle)
            {
                case 0:
                    PlayerPrefs.SetInt("gameMode", 3);
                    SceneManager.LoadScene("Game");
                    break;
                case 1:
                    PlayerPrefs.SetInt("gameMode", 0);
                    SceneManager.LoadScene("Game");
                    break;
                case 2:
                    PlayerPrefs.SetInt("gameMode", 1);
                    SceneManager.LoadScene("Game");
                    break;
                case 3:
                    PlayerPrefs.SetInt("gameMode", 2);
                    SceneManager.LoadScene("Game");
                    break;
            }
        }
    }
}
