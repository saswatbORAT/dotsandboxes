using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class aiScript : MonoBehaviour 
{
    public inputManagerScript input;
    public List<string> easyKeys, hardKeys, tempKeys;
    public int limit, tempLimit, hardLimit;
    int i, j, num;
    string temp, k1, k2;
    bool b1, b2, b3;
    int totalPos = 0, totalNeg = 0;
    public int pos = 0, neg = 0;
   
    void Start ()
    {
        tempLimit = hardLimit = limit;
        easyKeys = new List<string>();
        hardKeys = new List<string>();
        tempKeys = new List<string>();
	}

    public void setValues(string key)
    {
        easyKeys.Add(key);
        hardKeys.Add(key);
    }
    
    public void aiEasyMode()
    {
        aiMedium(false);
    }

    public void aiMediumMode()
    {
        aiHard(false);
    }

    public void aiHardMode()
    {
        aiHard(true);
    }   

    void aiEasy()
    {
        num = UnityEngine.Random.Range(0, tempLimit);
        while (input.keyAvailable(easyKeys[num]))
        {
            decreaseLimitEasy();
            num = UnityEngine.Random.Range(0, tempLimit);
        }
        input.createLineAI(easyKeys[num]);
        decreaseLimitEasy();
    }

    public void decreaseLimitEasy()
    {
        temp = easyKeys[tempLimit - 1];
        easyKeys[tempLimit - 1] = easyKeys[num];
        easyKeys[num] = temp;
        tempLimit--;
    }

    public void decreaseLimitHard()
    {
        temp = hardKeys[hardLimit - 1];
        hardKeys[hardLimit - 1] = hardKeys[num];
        hardKeys[num] = temp;
        hardLimit--;
    }

    public bool aiMedium(bool isHard)
    {
        for (i = 0; i < input.gs.w; i++)
        {
            for (j = 0; j < input.gs.h; j++)
            {
                if (input.lines.ContainsKey(j.ToString() + i.ToString() + (j + 1).ToString() + i.ToString()))//horizontal
                {
                    if (generateKeys(false, j.ToString() + i.ToString() + (j + 1).ToString() + i.ToString()))
                    {
                        input.createLineAI(j.ToString() + i.ToString() + (j + 1).ToString() + i.ToString());
                        return true;
                    }
                }
                if(input.lines.ContainsKey(i.ToString() + j.ToString() + i.ToString() + (j + 1).ToString()))//vertical
                {
                    if (generateKeys(true, i.ToString() + j.ToString() + i.ToString() + (j + 1).ToString()))
                    {
                        input.createLineAI(i.ToString() + j.ToString() + i.ToString() + (j + 1).ToString());
                        return true;
                    }
                }
            }
        }

        if (!isHard)
        {
            aiEasy();
        }

        return false;
    }

    bool generateKeys(bool isVertical, string key)
    {
        input.k1 = key.Substring(0, 2);
        input.k2 = key.Substring(2, 2);
        input.createKeys(isVertical);
        return (input.lines.ContainsKey(key) && !input.lines[key]) && (checkKeys(0) || checkKeys(1));
    }

    bool checkKeys(int index)
    {
        b1 = input.lines.ContainsKey(input.checkLines[index, 0]);
        b2 = input.lines.ContainsKey(input.checkLines[index, 1]);
        b3 = input.lines.ContainsKey(input.checkLines[index, 2]);

        if (b1 && b2 && b3)
        {
            b1 = input.lines[input.checkLines[index, 0]];
            b2 = input.lines[input.checkLines[index, 1]];
            b3 = input.lines[input.checkLines[index, 2]];

            if (b1 && b2 && b3)
            {
                return true;
            }
        }
        return false;
    }

    public void aiHard(bool isHard)
    {
        if (!aiMedium(true))
        {
            checkHardAvailability(isHard);
        }
    }

    public void checkHardAvailability(bool isHard)
    {
        if (hardLimit == 0)
        {
            if (isHard)
            {
                checkBoxCount();
            }
            else
            {
                aiEasy();
            }
            return;
        }
        num = UnityEngine.Random.Range(0, hardLimit);

        while (input.keyAvailable(hardKeys[num]))
        {
            if (hardLimit == 0)
            {
                if (isHard)
                {
                    checkBoxCount();
                }
                else
                {
                    aiEasy();
                }
                return;
            }
            decreaseLimitHard();
            num = UnityEngine.Random.Range(0, hardLimit);
        }
       
        generateKeys(hardKeys[num][0] == hardKeys[num][2], hardKeys[num]);
       
        checkBoxValues();
        if ((totalPos == 2 || totalNeg == 2))
        {
            decreaseLimitHard();
            checkHardAvailability(isHard);
        }
        else
        {
            input.createLineAI(hardKeys[num]);
        }
    }

    void checkBoxCount()
    {
        int count = 0, total = input.gs.w * input.gs.h;
        string key, k1 = "abcd", k2 = "abcd", currentKey = "abcd";
        List<string> keys = new List<string>();
        for (i = 0; i < input.gs.w; i++)
        {
            for (j = 0; j < input.gs.h; j++)
            {
                if (input.lines.ContainsKey(j.ToString() + i.ToString() + (j + 1).ToString() + i.ToString()))//horizontal
                {
                    key = j.ToString() + i.ToString() + (j + 1).ToString() + i.ToString();
                    if (!input.lines[key])
                    {
                        keys.Add(key);
                    }
                }
                if (input.lines.ContainsKey(i.ToString() + j.ToString() + i.ToString() + (j + 1).ToString()))//vertical
                {
                    key = i.ToString() + j.ToString() + i.ToString() + (j + 1).ToString();
                    if (!input.lines[key])
                    {
                        keys.Add(key);
                    }
                }
            }
        }

        for (i = 0; i < keys.Count; i++)
        {
            count = 0;
            key = keys[i];
            input.lines[key] = true;
            tempKeys.Add(key);
            generateKeys(key[0] == key[2], key);
            checkBoxValues();
            if (totalPos == 2)
            {
                for (j = 0; j < 3; j++)
                {
                    if (!input.lines[input.checkLines[0, j]])
                    {
                        k1 = input.checkLines[0, j];
                        input.lines[k1] = true;
                        tempKeys.Add(k1);
                    }
                }
            }
            if (totalNeg == 2)
            {
                for (j = 0; j < 3; j++)
                {
                    if (!input.lines[input.checkLines[1, j]])
                    {
                        k2 = input.checkLines[1, j];
                        input.lines[k2] = true;
                        tempKeys.Add(k2);
                    }
                }
            }
            do
            {
                checkBoxCreation(ref k1);
                checkBoxCreation(ref k2);
                count++;
            } while (count < keys.Count);
            if (tempKeys.Count - 1 < total)
            {
                total = tempKeys.Count - 1;
                currentKey = key;
            }
            for (int t = 0; t < tempKeys.Count; t++)
            {
                input.lines[tempKeys[t]] = false;
            }
            tempKeys.RemoveRange(0, tempKeys.Count);
        }
        input.createLineAI(currentKey);
    }

    void checkBoxCreation(ref string key)
    {
        generateKeys(key[0] == key[2], key);
        checkBoxValues();
        if (totalPos == 2)
        {
            loop(0, ref key);
        }
        if (totalNeg == 2)
        {
            loop(1, ref key);
        }
    }

    void loop(int index, ref string key)
    {
        for (j = 0; j < 3; j++)
        {
            if (!input.lines[input.checkLines[index, j]])
            {
                key = input.checkLines[index, j];
                input.lines[key] = true;
                tempKeys.Add(key);
            }
        }
    }

    void checkBoxValues()
    {
        totalPos = 0;
        totalNeg = 0;

        b1 = input.lines.ContainsKey(input.checkLines[0, 0]) &&
             input.lines.ContainsKey(input.checkLines[0, 1]) &&
             input.lines.ContainsKey(input.checkLines[0, 2]);
        b2 = input.lines.ContainsKey(input.checkLines[1, 0]) &&
             input.lines.ContainsKey(input.checkLines[1, 1]) &&
             input.lines.ContainsKey(input.checkLines[1, 2]);
        if (b1)
        {
            totalPos = Convert.ToInt32(input.lines[input.checkLines[0, 0]]) +
                        Convert.ToInt32(input.lines[input.checkLines[0, 1]]) +
                        Convert.ToInt32(input.lines[input.checkLines[0, 2]]);
        }
        if (b2)
        {
            totalNeg = Convert.ToInt32(input.lines[input.checkLines[1, 0]]) +
                        Convert.ToInt32(input.lines[input.checkLines[1, 1]]) +
                        Convert.ToInt32(input.lines[input.checkLines[1, 2]]);
        }
    }

}