using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{

    public TextMeshProUGUI Complete, Freeplay, Respect, BSecret, Time, Decisions;
    [SerializeField] AudioSource impact;
    bool firstFinish = false;
    [SerializeField] string DecisionID;
    [SerializeField] string D1;
    [SerializeField] string D2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //Progress.readData();
        
        if(Progress.getInt("Chapter") < 1)
        {
            Progress.setInt("Chapter", 1);
            firstFinish = true;
        }
        else
        {
            firstFinish = false;
        }
        Progress.saveData();
        StartCoroutine(EndChapter());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EndChapter()
    {
        float t = 0, temp = 0, secs = 0;
        float Len = 2;
        float TSc = Stats.allTime;

        yield return new WaitForSeconds(Len);
        Complete.gameObject.SetActive(true);
        impact.Play();
        if (firstFinish)
        {
            yield return new WaitForSeconds(Len);
            Freeplay.gameObject.SetActive(true);
            impact.Play();
        }
        yield return new WaitForSeconds(Len);
        Respect.text = "Mafia Respect: " + Progress.getInt("MRespect");
        Respect.gameObject.SetActive(true);
        impact.Play();
        yield return new WaitForSeconds(Len);
        impact.Play();

        Time.gameObject.SetActive(true);
        impact.loop = true;
        while (t < Len)
        {
            temp = UnityEngine.Time.deltaTime;
            t += temp;
            secs = (t / Len) * TSc;
            Time.text = "Time: " + ((int)(secs/60)).ToString("D2") + ":" + ((int)(secs%60)).ToString("D2");

            yield return new WaitForSeconds(temp);
        }
        Time.text = "Time: " + ((int)(TSc / 60)).ToString("D2") + ":" + Mathf.FloorToInt(TSc % 60).ToString("D2");
        impact.loop = false;
        yield return new WaitForSeconds(Len);
        BSecret.gameObject.SetActive(true); //NEED TO SET THIS LATER
        if (Progress.getBool("WeekAngelAnim")) BSecret.text = "Big Secret: FOUND";
        else if(Progress.getBool("WeekAngels")) BSecret.text = "Big Secret: WAITING...";
        else BSecret.text = "Big Secret: NOT FOUND";
        impact.Play();
        yield return new WaitForSeconds(Len);
        Decisions.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DecisionID + " [" + (Progress.getInt(DecisionID) == 1 ? D2 : D1) + "]";
        Decisions.gameObject.SetActive(true);
        impact.Play();

        yield return new WaitForSeconds(1);
    }


}
