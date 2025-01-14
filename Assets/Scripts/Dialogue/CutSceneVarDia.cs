using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

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
        Line = replaceVariables(Line,atStart:false);
        LineIfFalseAndFirstListed = replaceVariables(LineIfFalseAndFirstListed, atStart: false);

        if (FUNMax != FUNMin && !Progress.checkFUN(FUNMin, FUNMax)) return false;

        if (boolVarIds != "") return Progress.getBool(boolVarIds) == expectedTrue;

        if (intVarIds != "") return Progress.getInt(intVarIds) > minIntVals[0] && Progress.getInt(intVarIds) < maxIntVals[0];

        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        Line = replaceVariables(Line);
        LineIfFalseAndFirstListed = replaceVariables(LineIfFalseAndFirstListed);
    }

    string replaceVariables(string OG,bool atStart = true)
    {
        MatchCollection matches = Regex.Matches(OG, @"\[(.*?)\]");

        foreach (Match match in matches)//loop thru each variable
        {
            string wordInBrackets = match.Groups[1].Value;
            string replaceWith = "[" + wordInBrackets + "]";
            print($"Found: {wordInBrackets}");

            switch (wordInBrackets)
            {
                case "RandomUpgrade":
                    replaceWith = Progress.getRandomUpgrade();
                    if (replaceWith == null) replaceWith = "your... little sellin' spot. You should really buy something to make it more appealing";
                    else replaceWith += " it looks nice";
                    break;
                case "FredCost":
                    if (atStart) continue;

                    switch (Progress.getInt("FredF"))
                    {
                        case 0: replaceWith = "20'000p"; break;
                        case 1: replaceWith = "10'000p"; break;//max day 1   dec1
                        case 2: replaceWith = "5'000p"; break;//max day 2    dec1 + dec2
                        case 3: replaceWith = "2'000p"; break;
                        case 4: replaceWith = "1'000p"; break;//max day 3    dec1 + dec2 + task + dec3
                        case 5: replaceWith = "500p"; break;//max day 4    dec1 + dec2 + task + dec3 + dec4
                        case 6: replaceWith = "Free!"; break;
                        default: replaceWith = "Free!"; break;
                    }
                    
                    break;
                case "OutfitComment1":
                    if (atStart) continue;
                    int outfitID = outfit.getActiveOutfit();

                    switch (outfitID)
                    {
                        case 26: replaceWith = "I see you're starting to fit in with a shirt like that"; break; //wife beater
                        case 27: replaceWith = "I like your new shirt"; break;//Polo Shirt
                        case 28: replaceWith = "I like your new outfit. I appreciate a man who puts effort into his appearence"; break;//Polo shirt + tie
                        case 29: replaceWith = "You got that dress shirt for yourself? I'm impressed. You look proper"; break;//Dress shirt
                        case 30: replaceWith = "You smell, you really need a change of clothes"; break;//Dirty
                    }

                    break;

            }

            // Replace the word within brackets with the replacement word
            OG = OG.Replace(match.Value, replaceWith);

        }
        return OG;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
