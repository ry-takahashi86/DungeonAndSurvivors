using System;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryWeaponStats", menuName = "DungeonAndSurvivors/InventoryWeaponStats")]
public class InventoryWeaponStats : InventoryWeapon
{
    [Header("Weapon Stats")]
    public int WeaponMinDamage;
    public int WeaponMaxDamage;

    [Header("Defense Stats")]
    public int DefensePower;

    /// <summary>
    /// CharacterHandleWeaponに装備した武器データを渡す
    /// </summary>
    /// <param name="newWeapon"></param>
    /// <param name="playerID"></param>
    protected override void EquipWeapon(Weapon newWeapon, string playerID)
    {
        if (EquippableWeapon == null)
        {
            return;
        }
        if (TargetInventory(playerID).Owner == null)
        {
            return;
        }

        Character character = TargetInventory(playerID).Owner.GetComponentInParent<Character>();
        if (character == null)
        {
            return;
        }

        // CharacterHandleWeaponを取得して武器を設定
        CharacterHandleWeapon targetHandleWeapon = null;
        CharacterHandleWeapon[] handleWeapons = character.GetComponentsInChildren<CharacterHandleWeapon>();
        foreach (CharacterHandleWeapon handleWeapon in handleWeapons)
        {
            if (handleWeapon.HandleWeaponID == HandleWeaponID)
            {
                targetHandleWeapon = handleWeapon;
            }
        }

        if (targetHandleWeapon != null)
        {
            // 武器のスプライトを設定する
            if (newWeapon != null)
            {
                SpriteRenderer spriteRenderer = newWeapon.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = Icon;
                }
            }
            // MeleeWeapon or ProjectileWeapon の場合はダメージを設定
            if (newWeapon is MeleeWeapon meleeWeapon)
            {
                meleeWeapon.MinDamageCaused = WeaponMinDamage;
                meleeWeapon.MaxDamageCaused = WeaponMaxDamage;
            }
            else if (newWeapon is ProjectileWeaponStats projectileWeapon)
            {
                projectileWeapon.MinDamageCaused = WeaponMinDamage;
                projectileWeapon.MaxDamageCaused = WeaponMaxDamage;
            }
            targetHandleWeapon.ChangeWeapon(newWeapon, this.ItemID);
        }
    }

    public override GameObject SpawnPrefab(string playerID)
    {
        if (TargetInventory(playerID) != null)
        {
            // Prefabとプレイヤー座標が設定されている場合
            if (Prefab != null && TargetInventory(playerID).TargetTransform != null)
            {
                GameObject droppedObject=(GameObject)Instantiate(Prefab);
                // Prefabのスプライトを設定
                SpriteRenderer spriteRenderer = droppedObject.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = Icon;
                }

                ItemPicker droppedObjectItemPicker = droppedObject.GetComponent<ItemPicker>();
                if (droppedObjectItemPicker != null)
                {
                    // アイテムを拾ったときのアイテムデータを設定する
                    droppedObjectItemPicker.Item = this;

                    // ドロップ時の数量設定
                    if (ForcePrefabDropQuantity)
                    {
                        droppedObjectItemPicker.Quantity = PrefabDropQuantity;
                        droppedObjectItemPicker.RemainingQuantity = PrefabDropQuantity;	
                    }
                    else
                    {
                        droppedObjectItemPicker.Quantity = Quantity;
                        droppedObjectItemPicker.RemainingQuantity = Quantity;
                    }
                }

                // ドロップ位置とプロパティの適用
                MMSpawnAround.ApplySpawnAroundProperties(droppedObject, DropProperties,
                    TargetInventory(playerID).TargetTransform.position);

                // 生成したオブジェクトを返す
                return droppedObject;
            }
        }

        // 生成に失敗した場合は null を返す
        return null;
    }
}