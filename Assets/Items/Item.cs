using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite icon;
    public string description;
    public int price;
    public int MLevel;
    public int SodaPChange;
    public int SodaTChange;
}
