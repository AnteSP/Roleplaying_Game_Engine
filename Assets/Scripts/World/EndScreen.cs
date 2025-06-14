using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{

    public TextMeshProUGUI Complete, Freeplay, Respect, BSecret, Time;
    [SerializeField] AudioSource impact;
    bool firstFinish = false;
    [SerializeField] string friendshipID;

    [SerializeField] List<string> decisionIDs, decisionDescs;
    [SerializeField] List<TextMeshProUGUI> decisions;

    // Start is called before the first frame update
    void Start()
    {

    }
    int chNum;

    private void OnEnable()
    {
        //Progress.readData();
        chNum = int.Parse(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Substring(2));

        if (Progress.getInt("Chapter") < chNum)//
        {
            Progress.setInt("Chapter", chNum);
            firstFinish = true;
        } else firstFinish = false;

        Dialogue.d.showDisplay(false);
        StartCoroutine(EndChapter());
    }

    IEnumerator EndChapter()
    {
        float t = 0, temp = 0, secs = 0;
        float Len = 2;
        float TSc = Stats.allTime;
        Stats.allTime = 0;
        if(firstFinish) Progress.saveData(toFile: "saveArchiveCh" + chNum);

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
        Respect.text += Progress.getInt(friendshipID);
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
            Time.text = "Time: " + ((int)(secs / 60)).ToString("D2") + ":" + ((int)(secs % 60)).ToString("D2");

            yield return new WaitForSeconds(temp);
        }
        Time.text = "Time: " + ((int)(TSc / 60)).ToString("D2") + ":" + Mathf.FloorToInt(TSc % 60).ToString("D2");
        impact.loop = false;
        yield return new WaitForSeconds(Len);
        BSecret.gameObject.SetActive(true);
        BSecret.text = "Big Secret: " + BigSecretText();
        impact.Play();
        for (int i = 0; i < decisionIDs.Count; i++)
        {
            if (!Progress.doesFieldExist(decisionIDs[i]))
            {
                yield return new WaitForSeconds(Len / 3);
                decisions[i].transform.parent.gameObject.SetActive(true);
                decisions[i].transform.parent.parent.gameObject.SetActive(true);
                decisions[i].text = "[!] You missed a cutscene";
                decisions[i].gameObject.SetActive(true);
                continue;
            }
            yield return new WaitForSeconds(Len);
            impact.Play();
            decisions[i].transform.parent.gameObject.SetActive(true);
            decisions[i].transform.parent.parent.gameObject.SetActive(true);
            decisions[i].text = decisionDescs[i] + " [" + (Progress.getInt(decisionIDs[i]) == 1 ? "no" : "yes") + "]";//2 is good, 1 is bad
            decisions[i].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1);
    }

    string BigSecretText()
    {
        return BIG_SECRET_STRINGS[BigSecretStatus(chNum)];
    }

    public static readonly string[] BIG_SECRET_STRINGS = new string[] {"NOT FOUND","FOUND","WAITING..."};
    public static int BigSecretStatus(int ChNum)//0 = NOT FOUND. 1 = FOUND. 2 = WAITING
    {
        switch (ChNum)
        {
            case 1:
                if (Progress.getBool("WeekAngelAnim")) return 1;
                else if (Progress.getBool("WeekAngels")) return 2;
                else return 0;
            case 2:
                if (Progress.getBool("Ch2HatmanVisited")) return 1;
                else return 0;
            default:
                return -1;
        }
    }

}
