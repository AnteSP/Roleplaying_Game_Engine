using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

class Deadline : MonoBehaviour
{
    public char ID = 'A';
    public string requirements;
    public int Minutes;
    public GameObject Object;
    TextMeshProUGUI label, time;
    public string Description;
    public CutSceneTalker FailCutScene, SuccessCutScene;

    private void Start()
    {
        foreach (TextMeshProUGUI t in Object.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (t.gameObject.name == "Label") label = t;
            else time = t;
        }
    }

    Deadline(int day, int hour, int min, string Title, string requirements = "")
    {
        this.requirements = requirements;
        this.Description = Title;
        this.Minutes = (day * 24 * 60) + (hour * 60) + min;
    }

    public Deadline(string requirements, int Mins, GameObject Obj, string Titl, CutSceneTalker FailCS, CutSceneTalker SucceedCS)
    {
        this.requirements = requirements;
        Minutes = Mins;
        Object = Obj;
        Description = Titl;

        foreach (TextMeshProUGUI t in Object.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (t.gameObject.name == "Label") label = t;
            else time = t;
        }

        FailCutScene = FailCS;
        SuccessCutScene = SucceedCS;
    }

    public void Refresh()
    {
        bool Lessthan3days = Minutes < 24 * 60 * 3;
        bool Lessthanhour = Minutes < 61;
        time.text = (Lessthan3days ? ((Lessthanhour) ? Minutes : Minutes / 60) : Minutes / (24 * 60)) + "";
        label.text = (Lessthan3days ? ((Lessthanhour) ? "mins" : "hrs") : "days") + " left";
    }

    public bool fulfillRequirement()
    {
        if (this.requirements == "") return true;

        return false;
    }

}