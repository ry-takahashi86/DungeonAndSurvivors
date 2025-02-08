using System;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

[Serializable]
public class SerializedInventoryWeaponStats
{
    public int NumberOfRows;
    public int NumberOfColumns;
    public string InventoryName = "Inventory";
    public Inventory.InventoryTypes InventoryType ;
    public bool DrawContentInInspector=false;
    public string[] ContentType;
    public int[] ContentQuantity;

    // ScriptableObjectはシリアライズ化できないため値だけ抽出する
    public int[] WeaponMinDamage;
    public int[] WeaponMaxDamage;
    public int[] ArmorRating;
    public string[] Description;
    public Rarity[] ItemRarity;
    public List<ItemBonus>[] AdditionalBonuses;
}

public class InventoryManager : Inventory
{
    /// <summary>
    /// インベントリを保存する(InventoryWeaponStats対応版)
    /// </summary>
    public override void SaveInventory()
    {
        SerializedInventoryWeaponStats serializedInventory = new SerializedInventoryWeaponStats();
        FillSerializedInventory(serializedInventory);
        MMSaveLoadManager.Save(serializedInventory, DetermineSaveName(), _saveFolderName);
    }

    /// <summary>
    /// セーブファイルが存在する場合、インベントリを読み込む(InventoryWeaponStats対応版)
    /// </summary>
    public override void LoadSavedInventory()
    {
        SerializedInventoryWeaponStats serializedInventory = (SerializedInventoryWeaponStats)MMSaveLoadManager.Load(typeof(SerializedInventoryWeaponStats), DetermineSaveName(), _saveFolderName);
        ExtractSerializedInventory(serializedInventory);
        MMInventoryEvent.Trigger(MMInventoryEventType.InventoryLoaded, null, this.name, null, 0, 0, PlayerID);
    }

    /// <summary>
    /// 各アイテムのデータをシリアル化するためにパラメータを保存する
    /// </summary>
    /// <param name="serializedInventory">Serialized inventory.</param>
    protected void FillSerializedInventory(SerializedInventoryWeaponStats serializedInventory)
    {
        serializedInventory.InventoryType = InventoryType;
        serializedInventory.DrawContentInInspector = DrawContentInInspector;
        serializedInventory.ContentType = new string[Content.Length];
        serializedInventory.ContentQuantity = new int[Content.Length];
        serializedInventory.WeaponMinDamage = new int[Content.Length];
        serializedInventory.WeaponMaxDamage = new int[Content.Length];
        serializedInventory.ArmorRating = new int[Content.Length];
        serializedInventory.ItemRarity = new Rarity[Content.Length];
        serializedInventory.AdditionalBonuses = new List<ItemBonus>[Content.Length];
        for (int i = 0; i < Content.Length; i++)
        {
            if (!InventoryItem.IsNull(Content[i]))
            {
                serializedInventory.ContentType[i] = Content[i].ItemID;
                serializedInventory.ContentQuantity[i] = Content[i].Quantity;

                // 武器データを保存
                if (Content[i] is InventoryWeaponStats weaponStats)
                {
                    serializedInventory.WeaponMinDamage[i] = weaponStats.WeaponMinDamage;
                    serializedInventory.WeaponMaxDamage[i] = weaponStats.WeaponMaxDamage;
                    serializedInventory.ArmorRating[i] = weaponStats.ArmorRating;
                    serializedInventory.ItemRarity[i] = weaponStats.ItemRarity;
                    serializedInventory.AdditionalBonuses[i] = weaponStats.AdditionalBonuses;
                }
            }
            else
            {
                serializedInventory.ContentType[i] = null;
                serializedInventory.ContentQuantity[i] = 0;
            }
        }
    }

    /// <summary>
    /// 保存ファイルからシリアル化されているデータを読み込む
    /// </summary>
    /// <param name="serializedInventory">Serialized inventory.</param>
    protected void ExtractSerializedInventory(SerializedInventoryWeaponStats serializedInventory)
    {
        if (serializedInventory == null)
        {
            return;
        }

        InventoryType = serializedInventory.InventoryType;
        DrawContentInInspector = serializedInventory.DrawContentInInspector;
        Content = new InventoryWeaponStats[serializedInventory.ContentType.Length];
        for (int i = 0; i < serializedInventory.ContentType.Length; i++)
        {
            if ((serializedInventory.ContentType[i] != null) && (serializedInventory.ContentType[i] != ""))
            {
                // アイテムのマスターデータを取得
                _loadedInventoryItem = Resources.Load<InventoryItem>(_resourceItemPath + serializedInventory.ContentType[i]);

                if (_loadedInventoryItem == null)
                {
                    Debug.LogError("InventoryEngine : Couldn't find any inventory item to load at Resources/"+_resourceItemPath
                        +" named "+serializedInventory.ContentType[i] + ". Make sure all your items definitions names (the name of the InventoryItem scriptable " +
                        "objects) are exactly the same as their ItemID string in their inspector. Make sure they are in a  Resources/"+_resourceItemPath+" folder. " +
                        "Once that's done, also make sure you reset all saved inventories as the mismatched names and IDs may have " +
                        "corrupted them.");
                }
                else
                {
                    InventoryItem CopyItemData = _loadedInventoryItem.Copy();

                    if (CopyItemData is InventoryWeaponStats weaponStats)
                    {
                        weaponStats.WeaponMinDamage = serializedInventory.WeaponMinDamage[i];
                        weaponStats.WeaponMaxDamage = serializedInventory.WeaponMaxDamage[i];
                        weaponStats.ArmorRating = serializedInventory.ArmorRating[i];
                        weaponStats.ItemRarity = serializedInventory.ItemRarity[i];
                        weaponStats.AdditionalBonuses = serializedInventory.AdditionalBonuses[i];
                        weaponStats.UpdateDescription();

                        Content[i] = weaponStats.Copy();
                        Content[i].Quantity = serializedInventory.ContentQuantity[i];
                    }
                    else
                    {
                        Content[i] = _loadedInventoryItem.Copy();
                        Content[i].Quantity = serializedInventory.ContentQuantity[i];
                    }
                }
            }
            else
            {
                Content[i] = null;
            }
        }
    }
}
