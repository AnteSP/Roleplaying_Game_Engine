using TMPro;
using UnityEngine;
using System.Collections.Generic;

class Deadline : MonoBehaviour
{
    public class DeadlineData
    {
        public string Description;
        public int hour;
        public int day;

        public static implicit operator DeadlineData(Deadline d)
        {
            return new DeadlineData { Description = d.Description, day=d.day, hour=d.hour};
        }

        public Deadline cloneVariaiblesTo(Deadline d)
        {
            d.Description = this.Description;
            d.hour = this.hour;
            d.day = this.day;
            return d;
        }
    }

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

    static public DeadlineData[] activeDeadlineData = new DeadlineData[4] {null,null,null,null};

    public bool isSameEventAs(DeadlineData a)
    {
        return (a.hour == this.hour && a.day == this.day && a.Description == this.Description);
    }

    private void OnEnable()
    {
        //When Component is generated in-code, it gets enabled by default. If this happens, just disable and keep going
        if (day == 0)
        {
            enabled = false;
            return;
        }
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
        activeDeadlineData[stickyNote] = this;
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
            print("THROWING AWAY DEADLINE " + MinutesLeft + " / " + TriggerAtMinute);

            if (SuccessCutScene == null && FailCutScene == null)
            {
                Stats.current.stickyNotes[stickyNote].gameObject.SetActive(false);
                activeDeadlineData[stickyNote] = null;
                enabled = false;
                print("ERROR WITH DEADLINE. NO CS");
                return;
            }

            Stats.current.CurrentCS = fulfillRequirement() ? SuccessCutScene : FailCutScene;
            Stats.current.CurrentCS.enabled = true;
            Stats.Transition(0);

            Stats.current.stickyNotes[stickyNote].gameObject.SetActive(false);
            activeDeadlineData[stickyNote] = null;
            enabled = false;
            Stats.ChangeTime((uint) (Stats.allTimeInGame - TriggerAtMinute) );

            
        }
    }

    public bool fulfillRequirement()
    {
        if (this.requirements == "") return true;

        return false;
    }

}