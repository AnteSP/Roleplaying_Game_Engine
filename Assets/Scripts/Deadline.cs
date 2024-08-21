using TMPro;
using UnityEngine;
using System.Collections.Generic;

class Deadline : MonoBehaviour
{
    public char ID = 'A';
    public string Description;
    public string requirements;
    public int hour;
    public int day;
    public CutSceneTalker FailCutScene, SuccessCutScene;

    [Header("==Everything below auto-generates==")]
    public int TriggerAtMinute;
    public int MinutesLeft;
    public int stickyNote;
    TextMeshProUGUI label, time;
    Tooltip tooltip;

    static Deadline[] deadlines = new Deadline[4];

    private void OnEnable()
    {
        TriggerAtMinute = ((day - 1) * 24 * 60) + (((hour) * 60));

        stickyNote = Stats.GetStickyNote();
        if (stickyNote == -1) print("DEADLINE GET STICKY NOTES FUCKED UP ");
        foreach (TextMeshProUGUI t in Stats.current.stickyNotes[stickyNote].GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (t.gameObject.name == "Label") label = t;
            else time = t;
        }
        tooltip = Stats.current.stickyNotes[stickyNote].GetComponent<Tooltip>();

        tooltip.tooltip = Description;
        Refresh();
        deadlines[stickyNote] = this;
    }

    public void Refresh()
    {
        if (MinutesLeft == TriggerAtMinute - Stats.allTimeInGame) return;
        if (time == null) return;
        MinutesLeft = TriggerAtMinute - Stats.allTimeInGame;
        bool Lessthan3days = MinutesLeft < 24 * 60 * 3;
        bool Lessthanhour = MinutesLeft < 61;
        time.text = (Lessthan3days ? ((Lessthanhour) ? MinutesLeft : MinutesLeft / 60) : MinutesLeft / (24 * 60)) + "";
        label.text = (Lessthan3days ? ((Lessthanhour) ? "mins" : "hrs") : "days") + " left";

        if(MinutesLeft <= 0)
        {
            print("THROWING AWAY DEADLINE");

            Stats.current.CurrentCS = fulfillRequirement() ? SuccessCutScene : FailCutScene;
            Stats.current.CurrentCS.enabled = true;
            Stats.Transition(0);

            Stats.current.stickyNotes[stickyNote].gameObject.SetActive(false);
            deadlines[stickyNote] = null;
            enabled = false;
        }
    }

    public bool fulfillRequirement()
    {
        if (this.requirements == "") return true;

        return false;
    }

}