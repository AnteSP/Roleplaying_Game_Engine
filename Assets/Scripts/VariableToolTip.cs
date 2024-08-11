using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableToolTip : Tooltip
{
    public float[] nums;
    public string[] strings;

    private void Start()
    {
        SetToolTip();
    }

    public void ChangeValues(float number,int IndexN,string words,int IndexS)
    {
        nums[IndexN] = number;
        strings[IndexS] = words;
        SetToolTip();

    }

    private void SetToolTip()
    {
        tooltip += strings[0];
        for (int i = 0; i < nums.Length; i++)
        {
            tooltip += nums[i] + "";
            tooltip += strings[i + 1];
        }

    }
}
