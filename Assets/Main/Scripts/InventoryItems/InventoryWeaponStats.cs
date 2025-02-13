using System;
using System.Collections.Generic;
using System.Text;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
/// アイテムのレアリティ
/// </summary>
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

/// <summary>
/// 追加ボーナスで上昇させたいパラメータ種別を定義します。
/// （Strength, Agility, CriticalRate, MaxHP 等）
/// </summary>
public enum ParameterType
{
    Strength,
    Dexterity,
    Agility,
    Intelligence,
    MaxHP,
    PhysicalDamageBonus,
    CriticalRate,
    SkillCooldownRate,
    MoveSpeedBonus,
    AttackSpeed,
    PhysicalDamageReductionRate,
    MaxMP,
    MagicDamageBonus,
    MagicDamageReductionRate
}

/// <summary>
/// 追加ボーナス1つ分の情報
/// </summary>
[Serializable]
public class ItemBonus
{
    public ParameterType ParameterType;
    public float Value;

    public ItemBonus(ParameterType parameterType, float value)
    {
        this.ParameterType = parameterType;
        this.Value = value;
    }
}

public interface IParameterBonus
{
    /// <summary>
    /// 指定したParameterTypeに対する追加ボーナスを返す(なければ0を返す)
    /// </summary>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    int GetBonus(ParameterType parameterType);
}

[CreateAssetMenu(fileName = "InventoryWeaponStats", menuName = "DungeonAndSurvivors/InventoryWeaponStats")]
[Serializable]
public class InventoryWeaponStats : InventoryWeapon, IParameterBonus
{
    [Header("装備アイテムデータ")]
    [Header("Weapon Stats")]
    public int WeaponMinDamage;
    public int WeaponMaxDamage;

    [Header("Defense Stats")]
    public int ArmorRating;

    [Header("Rarity & Bonus")]
    public Rarity ItemRarity;

    [Tooltip("レアリティに応じて付与される追加ボーナス")]
    public List<ItemBonus> AdditionalBonuses = new List<ItemBonus>();

    // パラメータ種別ごとに割り当てる文字色（HTML カラーコード）
    private static readonly Dictionary<Rarity, string> _rarityColors = new Dictionary<Rarity, string>()
    {
        { Rarity.Common, "#FFFFFF" },       // 白
        { Rarity.Uncommon, "#00FF00" },     // 緑
        { Rarity.Rare, "#0000FF" },         // 青
        { Rarity.Epic, "#800080" },         // 紫
        { Rarity.Legendary, "#FFA500" },    // オレンジ
    };

    // ここでは、AdditionalBonuses リストから該当する ParameterType のボーナスを合算して返します
    public int GetBonus(ParameterType parameterType)
    {
        int bonus = 0;
        if (AdditionalBonuses != null)
        {
            foreach (var bonusItem in AdditionalBonuses)
            {
                if (bonusItem.ParameterType == parameterType)
                {
                    bonus += Mathf.RoundToInt(bonusItem.Value);
                }
            }
        }
        return bonus;
    }

    public void UpdateDescription()
    {
        StringBuilder sb = new StringBuilder();

        // レアリティと基本性能の表示
        string colorCode;
        if (!_rarityColors.TryGetValue(ItemRarity, out colorCode))
        {
            colorCode = "#FFFFFF";
        }
        string coloredLine = $"<color={colorCode}>{ItemRarity}</color>";
        sb.AppendLine($"レアリティ: {coloredLine}");

        float _avarageDamage = (WeaponMinDamage + WeaponMaxDamage) / 2;
        if (_avarageDamage > 0f)
        {
            sb.AppendLine($"武器ダメージ: {WeaponMinDamage} - {WeaponMaxDamage}");
        }
        if (ArmorRating != 0)
        {
            sb.AppendLine($"防御力: {ArmorRating}");
        }

        // 追加ボーナスの内容を文字列化
        if (AdditionalBonuses != null && AdditionalBonuses.Count > 0)
        {
            foreach (ItemBonus bonus in AdditionalBonuses)
            {
                string paramStr = ParameterTypeToString(bonus.ParameterType);

                string valueStr = "";
                switch (bonus.ParameterType)
                {
                    case ParameterType.Strength:
                    case ParameterType.Dexterity:
                    case ParameterType.Agility:
                    case ParameterType.Intelligence:
                        valueStr = bonus.Value.ToString();
                    break;

                    default:
                        valueStr = bonus.Value.ToString("N2") + "%";
                    break;
                }

                colorCode = "#00bfff";
                coloredLine = $"<color={colorCode}>{paramStr} +{valueStr}</color>";
                sb.AppendLine($"- {coloredLine}");
            }
        }

        // まとめた文字列を Description に反映
        Description = sb.ToString();
    }

    public string ParameterTypeToString(ParameterType param)
    {
        switch (param)
        {
            case ParameterType.Strength:
                return "力";
            case ParameterType.Dexterity:
                return "器用さ";
            case ParameterType.Agility:
                return "敏捷性";
            case ParameterType.Intelligence:
                return "知性";
            case ParameterType.MaxHP:
                return "最大HP";
            case ParameterType.PhysicalDamageBonus:
                return "物理ダメージボーナス";
            case ParameterType.CriticalRate:
                return "クリティカル率";
            case ParameterType.SkillCooldownRate:
                return "スキルクールダウン減少率";
            case ParameterType.MoveSpeedBonus:
                return "移動速度ボーナス";
            case ParameterType.AttackSpeed:
                return "攻撃速度";
            case ParameterType.PhysicalDamageReductionRate:
                return "物理ダメージ軽減率";
            case ParameterType.MaxMP:
                return "最大MP";
            case ParameterType.MagicDamageBonus:
                return "魔法ダメージボーナス";
            case ParameterType.MagicDamageReductionRate:
                return "魔法ダメージ軽減率";
            default:
                Debug.LogWarning($"{this.name}: パラメータタイプの文字列変換に失敗しました");
                return "";
        }
    }

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

    /// <summary>
    /// アイテムを新しいアイテムにコピーする
    /// </summary>
    public override InventoryItem Copy()
    {
        string name = this.name;
        InventoryWeaponStats clone = Instantiate(this) as InventoryWeaponStats;
        clone.name = name;
        clone.WeaponMinDamage = WeaponMinDamage;
        clone.WeaponMaxDamage = WeaponMaxDamage;
        clone.ArmorRating = ArmorRating;
        clone.ItemRarity = ItemRarity;
        clone.AdditionalBonuses = AdditionalBonuses;
        clone.UpdateDescription();
        return clone;
    }
}