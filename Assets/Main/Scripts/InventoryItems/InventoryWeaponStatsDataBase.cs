using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemDataBase", menuName = "DungeonAndSurvivors/InventoryItemDataBase")]
public class InventoryWeaponStatsDataBase : ScriptableObject
{
    public List<InventoryWeaponStats> InventoryItems = new List<InventoryWeaponStats>();
}
