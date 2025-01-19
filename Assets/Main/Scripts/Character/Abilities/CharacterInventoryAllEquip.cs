using System;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterInventoryAllEquip : CharacterInventory
{
    [Header("Add Inventories")]
    public String HeadInventoryName;
    public String BodyInventoryName;
    public String FootInventoryName;
    public String AccessoryInventoryName;

    public virtual Inventory HeadInventory { get; set; }
    public virtual Inventory BodyInventory { get; set; }
    public virtual Inventory FootInventory { get; set; }
    public virtual Inventory AccessoryInventory { get; set; }

    protected override void GrabInventories()
    {
        // すべてのインベントリを取得
        Inventory[] inventories = FindObjectsByType<Inventory>(FindObjectsSortMode.None);

        // プレイヤーIDに応じてインベントリを設定
        foreach (Inventory inventory in inventories)
        {
            if (inventory.PlayerID != PlayerID)
            {
                continue;
            }
            if ((MainInventory == null) && (inventory.name == MainInventoryName))
            {
                MainInventory = inventory;
            }
            if ((WeaponInventory == null) && (inventory.name == WeaponInventoryName))
            {
                WeaponInventory = inventory;
            }
            if ((HeadInventory == null) && (inventory.name == HeadInventoryName))
            {
                HeadInventory = inventory;
            }
            if ((BodyInventory == null) && (inventory.name == BodyInventoryName))
            {
                BodyInventory = inventory;
            }
            if ((FootInventory == null) && (inventory.name == FootInventoryName))
            {
                FootInventory = inventory;
            }
            if ((AccessoryInventory == null) && (inventory.name == AccessoryInventoryName))
            {
                AccessoryInventory = inventory;
            }
            if ((HotbarInventory == null) && (inventory.name == HotbarInventoryName))
            {
                HotbarInventory = inventory;
            }
        }

        // インベントリの設定
        if (MainInventory != null) { MainInventory.SetOwner (this.gameObject); MainInventory.TargetTransform = InventoryTransform;}
        if (WeaponInventory != null) { WeaponInventory.SetOwner (this.gameObject); WeaponInventory.TargetTransform = InventoryTransform;}
        if (HotbarInventory != null) { HotbarInventory.SetOwner (this.gameObject); HotbarInventory.TargetTransform = InventoryTransform;}
        if (HeadInventory != null) { HeadInventory.SetOwner (this.gameObject); HeadInventory.TargetTransform = InventoryTransform;}
        if (BodyInventory != null) { BodyInventory.SetOwner (this.gameObject); BodyInventory.TargetTransform = InventoryTransform;}
        if (FootInventory != null) { FootInventory.SetOwner (this.gameObject); FootInventory.TargetTransform = InventoryTransform;}
        if (AccessoryInventory != null) { AccessoryInventory.SetOwner (this.gameObject); AccessoryInventory.TargetTransform = InventoryTransform;}
    }
}