using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneVarDia : MonoBehaviour
{
    /// <summary>
    /// MUST BE A UNIQUE ID TO ALL OTHERS ON GAMEOBJECT
    /// </summary>
    public char ID;
    public string Line;
    public string LineIfFalseAndFirstListed;

    [Range(1, 100)] public int FUNMin = 0, FUNMax = 100;

    /// <summary>
    /// If doing multiple, separate their string IDs with ;
    /// </summary>
    public string boolVarIds = "";
    public bool expectedTrue = true;
    public string floatVarIds = "";
    public string intVarIds = "";
    public string switchVarIds = "";
    public string itemsNeeded = "";
    public string upgradesNeeded = "";

    public List<float> minFloatVals = new List<float>();
    public List<float> maxFloatVals = new List<float>();

    public List<int> minIntVals = new List<int>();
    public List<int> maxIntVals = new List<int>();


    /// <summary>
    /// Check if this line of Dialogue can be triggered
    /// </summary>
    /// <returns></returns>
    public bool isValid()
    {
        if (FUNMax != FUNMin && !Progress.checkFUN(FUNMin, FUNMax)) return false;

        if (boolVarIds != "") return Progress.getBool(boolVarIds) == expectedTrue;

        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
