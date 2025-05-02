using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SellSpot : Resource
{
    [SerializeField] GameObject SellSpotPrompt;
    [Tooltip ("0 means no people. 1 means max people")]
    public float MLevel = 1f;//0 means no people. 1 means max people
    [Tooltip("0 means no stealing. 1 means literally everything gets stolen")]
    public float SLevel = 0.1f;//1-10
    public int MinsToPrep = 30;
    public int CostToPrep = 0;
    public string triggerOnSetup = "";
    float ogY;
    [SerializeField] GameObject SodaMachine;

    public static SellSpot current = null;

    public static List<string> sellSpots { get; } = new List<string>();
    public static List<SellSpot> possibleSellSpots { get; } = new List<SellSpot>();

    private void Awake()
    {
        possibleSellSpots.Add(this);
    }

    void Start()
    {
        ogY = transform.position.y;
    }

    public static void resetSellSpotsList()
    {
        sellSpots.Clear();
        possibleSellSpots.Clear();
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, ogY + (Stats.secSin*0.3f), transform.position.z);
    }

    public override void Use(float Amount)
    {
        Stats.StartStopTime(false,"SellSpot");
        current = this;
        
        SellSpotPrompt.SetActive(true);
        Transform t = SellSpotPrompt.transform.GetChild(0);

        t.Find("Time").GetComponent<Text>().text = "Setup Time: " + MinsToPrep + " mins";
        t.Find("Cost").GetComponent<Text>().text = "Setup Cost: " + CostToPrep.ToString("#,##0") + "¢";
        t.Find("Foot Traffic").GetComponent<Text>().text = "Foot Traffic: " + (int)(MLevel * 100) + "%";
        t.Find("Security").GetComponent<Text>().text = "Thief chance: " + (int)(SLevel * 100) + "%";
    }

    public void turnIntoMarket()
    {
        Stats.ChangeTimeAnim(MinsToPrep);

        setAsMarket();
        if (triggerOnSetup != "") Progress.switchInPlay(triggerOnSetup, true);
    }

    public void setAsMarket()
    {
        sellSpots.Add(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "/" + gameObject.name);
        GameObject s = Instantiate(ChooseSellSoda.example.gameObject, transform);
        s.transform.SetParent(transform.parent);
        s.transform.localPosition = Vector3.zero;
        //print("TURN INTO " + s.name + " " + s.transform.childCount);

        for (int i = 0; i < s.transform.childCount; i++)
        {
            SellUpgrade.addToAllInstances(s.transform.GetChild(i).GetComponent<SellUpgrade>());
        }

        SellUpgrade.addToAllInstances(s.GetComponentsInChildren<SellUpgrade>());

        ChooseSellSoda css = s.GetComponent<ChooseSellSoda>();
        css.MLevel = MLevel;
        css.SLevel = SLevel;
        Destroy(gameObject);
        if (SodaMachine != null) SodaMachine.SetActive(true);
    }

    public static void closeSetUpScreen()
    {
        Stats.StartStopTime(true, "SellSpot");
    }

}
