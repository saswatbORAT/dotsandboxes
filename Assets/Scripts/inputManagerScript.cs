using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class inputManagerScript : MonoBehaviour 
{
    public soundManagerScript sms;
    public Text p1ScoreText, p2ScoreText;
    public GameObject linePrefab;
    public GameObject line;
    Vector2 initPos, currentPos;
    bool isDown;
    RaycastHit2D hit;
    Ray ray;
    float distance;
    public gameManagerScript gs;
    public aiScript ai;
    public int x, y, i, j;
    public float angle, aiLineLength;
    public bool xLeft, xRight, yUp, yDown;
    public Dictionary<string, bool> lines;
    Dictionary<string, bool> boxes;
    string key, invertKey, boxId;
    public string k1, k2, k3_Pos, k3_Neg, k4_Pos, k4_Neg;
    Vector2 angleUnit;
    bool lineCreated, lineComplete;
    public string[,] checkLines;
    bool gameComplete = false;
    public bool aiTurn;

    public int p1Score, p2Score;
    public bool p1Playing = true;
    bool boxcre;

    void Start () 
    {
        sms = GameObject.Find("soundManager").GetComponent<soundManagerScript>();
        gs.loadData();
        addKeys();
    }

    public void addKeys()
    {
        checkLines = new string[2, 3];
        lines = new Dictionary<string, bool>();
        boxes = new Dictionary<string, bool>();
        for (i = 0; i < gs.h; i++)
        {
            for (j = 0; j < gs.w - 1; j++)
            {
                key = j.ToString() + i.ToString() + (j + 1).ToString() + i.ToString();//horiontal
                lines.Add(key, false);
                ai.setValues(key);
            }
        }
        for (i = 0; i < gs.w; i++)
        {
            for (j = 0; j < gs.h - 1; j++)
            {
                key = i.ToString() + j.ToString() + i.ToString() + (j + 1).ToString();//vertical
                lines.Add(key, false);
                ai.setValues(key);
            }
        }

        ai.limit = ai.easyKeys.Count;
        ai.hardLimit = ai.tempLimit = ai.limit;
        for (i = 0; i < gs.w - 1; i++)
        {
            for (j = 0; j < gs.h - 1; j++)
            {
                boxId = i.ToString() + j.ToString();
                boxes.Add(boxId, false);
            }
        }
    }

    void Update() 
    {
        if (Input.GetMouseButtonDown(0) && !gs.isPaused && !aiTurn)
        {
            initPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(initPos, Vector2.zero);
            if (hit.collider != null)
            {
                x = (int)Mathf.Abs(hit.collider.transform.localPosition.x - gs.pos.x);
                y = (int)Mathf.Abs(hit.collider.transform.localPosition.y - gs.pos.y);
                isDown = true;
                line = Instantiate(linePrefab, new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, 0), Quaternion.identity) as GameObject;
            }
        }
        createLine();
        if (Input.GetMouseButtonUp(0) && !gs.isPaused)
        {
            if (lineComplete && gs.gameMode != 0 && !p1Playing)
            {
                lineComplete = false;
                aiInput();
            }
            else if(!aiTurn)
            {
                Destroy(line);
                isDown = false;
            }
        }
      
	}
    void aiInput()
    {
        switch (gs.gameMode)
        {
            case 1:
                ai.aiEasyMode();
                break;
            case 2:
                ai.aiMediumMode();
                break;
            case 3:
                ai.aiHardMode();
                break;
            default:
                ai.aiEasyMode();
                break;
        }
    }

    void createLine()
    {
        if (isDown && line != null)
        {
            boxcre = false;
            currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            distance = MathScript.getDistance(initPos, currentPos);
            angle = MathScript.getAngle(initPos, currentPos);
            isDown = lineInsideGrid();
            if (isDown)
            {
                isDown = distance < 1;
                distance = isDown ? distance : 1;
                line.transform.localScale = new Vector3(distance * 2, 0.25f, 1);
                line.transform.eulerAngles = new Vector3(0, 0, angle);
                if (distance == 1)
                {
                    lineComplete = true;
                    angleUnit.x = (int)Mathf.Cos(angle * Mathf.Deg2Rad);
                    angleUnit.y = (int)Mathf.Sin(angle * Mathf.Deg2Rad) * -1;
                    createCurrentKey(new Vector2(x, y), angleUnit);
                    checkLineCreation(key);
                    sms.playAudio();
                }
                line = isDown && line != null ? line : null;
            }
        }
    }

    public bool keyAvailable(string checkingKey)
    {
        if (lines.ContainsKey(checkingKey))
        {
            return lines[checkingKey];
        }
        return true;
    }

    string createCurrentKey(Vector2 pos, Vector2 unit)
    {
        k1 = pos.x.ToString() + pos.y.ToString();
        k2 = (pos.x + unit.x).ToString() + (pos.y + unit.y).ToString();
        key = k1 + k2;
        invertKey = k2 + k1;
        lineCreated = lines.ContainsKey(key);
        key = lineCreated ? key : invertKey;
        k1 = key.Substring(0, 2);
        k2 = key.Substring(2, 2);

        return key;
    }

    public void createLineAI(string aiKey)
    {
        key = aiKey;
        Vector2 initPos = new Vector2(gs.pos.x + (int)(key[0] - 48), gs.pos.y - (int)(key[1] - 48));
        Vector2 finalPos = new Vector2(gs.pos.x + (int)(key[2] - 48), gs.pos.y - (int)(key[3] - 48));
        float angle = MathScript.getAIAngle(initPos, finalPos);
        line = Instantiate(linePrefab, initPos, Quaternion.Euler(new Vector3(0, 0, angle))) as GameObject;
        aiTurn = true;
        StartCoroutine(aiDrawLine(key));
    }

    public void checkLineCreation(string key)
    {
        if (lines[key]){
            Destroy(line);
            line = null;
        }else{
            p1Playing = !p1Playing;
            line.GetComponent<SpriteRenderer>().color = isDown ? Color.white : p1Playing ? Color.cyan : Color.green;
            if (key[0] == key[2]){
                createKeys(true);
                if (boxCreated(0)){
                    boxes[k1] = true;
                    addScore(!p1Playing);
                    addIcon(k1);
                }
                if (boxCreated(1)){
                    k1 = ((char)(k1[0] - 1) + "" + (char)k1[1]).ToString();
                    boxes[k1] = true;
                    addScore(!p1Playing);
                    addIcon(k1);
                }
            }else{
                createKeys(false);
                if (boxCreated(0)){
                    boxes[k1] = true;
                    addScore(!p1Playing);
                    addIcon(k1);
                }
                if (boxCreated(1)){
                    k1 = ((char)k1[0] + "" + (char)(k1[1] - 1)).ToString();
                    boxes[k1] = true;
                    addScore(!p1Playing);
                    addIcon(k1);
                }
            }

            if (boxcre)
            {
                p1Playing = !p1Playing;
            }
            

            p1ScoreText.text = p1Score.ToString();
            p2ScoreText.text = p2Score.ToString();

            p1ScoreText.color = p1Playing ? Color.green : Color.black;
            p2ScoreText.color = p1Playing ? Color.black : Color.green;
            
            gameComplete = checkComplete();

            if (gameComplete)
            {
                gameOver();
            }
              

            lines[key] = true;
            line = null;
            if (!gameComplete && boxcre && !p1Playing && gs.gameMode != 0)
            {
                boxcre = false;
                aiInput();
            }
            boxcre = false;
        }       
    }

    void gameOver()
    {
        bool p1Won = p1Score > p2Score;
        int gamesPlayed, gamesWon;
        gs.gameOver_panel.SetActive(true);
        gs.pause_btn.SetActive(false);
        gs.pause_img.SetActive(false);
        gs.isPaused = true;

        switch (gs.gameMode)
        {
            case 1:
                gamesPlayed = PlayerPrefs.GetInt("easyPlayed");
                gamesPlayed++;
                PlayerPrefs.SetInt("easyPlayed", gamesPlayed);
                if (p1Won)
                {
                    gamesWon = PlayerPrefs.GetInt("easyWon");
                    gamesWon++;
                    PlayerPrefs.SetInt("easyWon", gamesWon);
                }
                break;
            case 2:
                gamesPlayed = PlayerPrefs.GetInt("mediumPlayed");
                gamesPlayed++;
                PlayerPrefs.SetInt("mediumPlayed", gamesPlayed);
                if (p1Won)
                {
                    gamesWon = PlayerPrefs.GetInt("mediumWon");
                    gamesWon++;
                    PlayerPrefs.SetInt("mediumWon", gamesWon);
                }
                break;
            case 3:
                gamesPlayed = PlayerPrefs.GetInt("hardPlayed");
                gamesPlayed++;
                PlayerPrefs.SetInt("hardPlayed", gamesPlayed);
                if (p1Won)
                {
                    gamesWon = PlayerPrefs.GetInt("hardWon");
                    gamesWon++;
                    PlayerPrefs.SetInt("hardWon", gamesWon);
                }
                break;
        }
    }

    void addIcon(string key)
    {
        float x = System.Convert.ToInt32(k1[0]) - 48;
        float y = System.Convert.ToInt32(k1[1]) - 48;
        x += gs.pos.x;
        y -= gs.pos.y;
        y *= -1;
        x += 0.5f;
        y -= 0.5f;

        GameObject icon = new GameObject("icon");
        icon.transform.position = new Vector3(x, y, 0);
        icon.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        icon.AddComponent<SpriteRenderer>();
        icon.GetComponent<SpriteRenderer>().sprite = gs.icons[!p1Playing ? gs.p1Index : gs.p2Index];
    }

    void addScore(bool turn)
    {
        boxcre = true;
        p1Score += turn ? 1 : 0;
        p2Score += turn ? 0 : 1;
    }

    bool checkComplete()
    {
        gameComplete = true;
        for (i = 0; i < gs.w - 1; i++)
        {
            for (j = 0; j < gs.h - 1; j++)
            {
                boxId = i.ToString() + j.ToString();
                if (!boxes[boxId])
                {
                    gameComplete = false;
                    break;
                }
            }
        }
        return gameComplete;
    }

    bool boxCreated(int index)
    {
        for (j = 0; j < 3; j++)
        {
            if (lines.ContainsKey(checkLines[index, j]))
            {
                if (!lines[checkLines[index, j]])
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void createKeys(bool isVertical)
    {
        k3_Pos = isVertical ? ((char)(k1[0] + 1) + "" + (char)k1[1]).ToString() :
                              ((char)k1[0] + "" + (char)(k1[1] + 1)).ToString();

        k4_Pos = isVertical ? ((char)(k2[0] + 1) + "" + (char)k2[1]).ToString() :
                              ((char)k2[0] + "" + (char)(k2[1] + 1)).ToString();

        k3_Neg = isVertical ? ((char)(k1[0] - 1) + "" + (char)k1[1]).ToString() :
                              ((char)k1[0] + "" + (char)(k1[1] - 1)).ToString();

        k4_Neg = isVertical ? ((char)(k2[0] - 1) + "" + (char)k2[1]).ToString() :
                              ((char)k2[0] + "" + (char)(k2[1] - 1)).ToString();

        checkLines[0, 0] = k1 + k3_Pos;
        checkLines[0, 1] = k2 + k4_Pos;
        checkLines[0, 2] = k3_Pos + k4_Pos;

        checkLines[1, 0] = k3_Neg + k1;
        checkLines[1, 1] = k4_Neg + k2;
        checkLines[1, 2] = k3_Neg + k4_Neg;
    }

    bool lineInsideGrid()
    {
        xLeft = x == 0 && angle == 180;
        xRight = x == (gs.w - 1) && angle == 0 && distance > 0.1f;
        yUp = y == 0 && angle == 90;
        yDown = y == (gs.h - 1) && angle == 270;
        return !xLeft && !xRight && !yUp && !yDown;
    }

    IEnumerator aiDrawLine(string key)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        drawLine(key);
    }

    void drawLine(string key)
    {
        line.transform.localScale = new Vector3(aiLineLength, 0.25f, 1);
        aiLineLength += Time.deltaTime * 4;
        if (aiLineLength >= 2.0f)
        {
            StopCoroutine("aiDrawLine");
            aiLineLength = 0;
            aiTurn = false;
            k1 = key.Substring(0, 2);
            k2 = key.Substring(2, 2);
            checkLineCreation(key);
            sms.playAudio();
        }
        else
        {
            StartCoroutine(aiDrawLine(key));
        }
    }

}
