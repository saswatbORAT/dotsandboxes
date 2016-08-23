using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class gameManagerScript : MonoBehaviour {
    public GameObject circlePrefab;
    public GameObject circles;
    GameObject circleTemp;
    [System.NonSerialized]public int w, h, i, j, p1Index, p2Index;
    [System.NonSerialized]public Vector2 pos, tempPos;
    public bool isPaused;
    public Sprite[] icons;
    public Image p1, p2;
    public GameObject pause_btn;
    public GameObject pause_img;
    public GameObject pause_panel, gameOver_panel;
    public int gameMode;
    inputManagerScript input;

    public void Start()
    {
        input = GameObject.FindObjectOfType<inputManagerScript>();
        gameMode = PlayerPrefs.GetInt("gameMode");
        p1Index = PlayerPrefs.GetInt("p1Icon", 0);
        p2Index = PlayerPrefs.GetInt("p2Icon", 1);
        p1.sprite = icons[p1Index];
        p2.sprite = icons[p2Index];
    }

    public void loadData()
    {
        w = PlayerPrefs.GetInt("wGrid", 6);
        h = PlayerPrefs.GetInt("hGrid", 6);
        pos.x = -((w - 1) * 0.5f);
        pos.y = ((h - 1) * 0.5f);
        tempPos = pos;
        for (i = 0; i < w; i++)
        {
            for (j = 0; j < h; j++)
            {
                circleTemp = Instantiate(circlePrefab, new Vector3(tempPos.x, tempPos.y, 0), Quaternion.identity) as GameObject;
                circleTemp.transform.parent = circles.transform;
                tempPos.y--;
            }
            tempPos.x++;
            tempPos.y = pos.y;
        }
    }

    public void pause()
    {
        Time.timeScale = 0;
        pause_btn.SetActive(false);
        pause_img.SetActive(false);
        pause_panel.SetActive(true);
        isPaused = true;
    }

    public void resume()
    {
        Time.timeScale = 1;
        pause_btn.SetActive(true);
        pause_img.SetActive(true);
        pause_panel.SetActive(false);
        isPaused = false;
    }

    public void restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public void menu()
    {
        Time.timeScale = 1;
        Destroy(GameObject.Find("soundManager"));
        SceneManager.LoadScene("Menu");
    }
}
