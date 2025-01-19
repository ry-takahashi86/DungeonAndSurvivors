using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemDataBase", menuName = "DungeonAndSurvivors/InventoryItemDataBase")]
public class InventoryItemDataBase : ScriptableObject
{
    public List<InventoryItem> InventoryItems = new List<InventoryItem>();
}
