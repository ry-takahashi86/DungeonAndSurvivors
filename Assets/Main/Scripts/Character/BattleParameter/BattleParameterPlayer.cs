using MoreMountains.InventoryEngine;
using UnityEngine;

[System.Serializable]
public class BattleParameterBasePlayer : BattleParameterBase
{
    [Header("装備アイテム")]
    public InventoryItem AttackWeapon;
    public InventoryItem HeadWeapon;
    public InventoryItem BodyWeapon;
    public InventoryItem FootWeapon;
    public InventoryItem AccessoryWeapon;

    [Header("その他")]
    [Min(1)] public int Level;
    [Min(0)] public int Exp;
    [Min(0)] public int Money;

    [Header("装備パラメータ")]
    public int WeaponDamageAvarege
    {
        get
        {
            int min = (AttackWeapon as InventoryWeaponStats)?.WeaponMinDamage ?? 0;
            int max = (AttackWeapon as InventoryWeaponStats)?.WeaponMaxDamage ?? 0;
            return (min + max) / 2;
        }
    }
    public int ArmorRating
    {
        get
        {
            int armorRating = 0;
            armorRating += (AttackWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            armorRating += (HeadWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            armorRating += (BodyWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            armorRating += (FootWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            armorRating += (AccessoryWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            return armorRating;
        }
    }

    public int FinalStrength
    {
        get
        {
            int sum = Strength;
            sum += (int)(AttackWeapon as InventoryWeaponStats)?.EquipmentBonus.StrengthBonus;
            sum += (int)(HeadWeapon as InventoryWeaponStats)?.EquipmentBonus.StrengthBonus;
            sum += (int)(BodyWeapon as InventoryWeaponStats)?.EquipmentBonus.StrengthBonus;
            sum += (int)(FootWeapon as InventoryWeaponStats)?.EquipmentBonus.StrengthBonus;
            sum += (int)(AccessoryWeapon as InventoryWeaponStats)?.EquipmentBonus.StrengthBonus;
            return sum;
        }
    }
    public int FinalDexterity
    {
        get
        {
            int sum = Dexterity;
            sum += (int)(AttackWeapon as InventoryWeaponStats)?.EquipmentBonus.DexterityBonus;
            sum += (int)(HeadWeapon as InventoryWeaponStats)?.EquipmentBonus.DexterityBonus;
            sum += (int)(BodyWeapon as InventoryWeaponStats)?.EquipmentBonus.DexterityBonus;
            sum += (int)(FootWeapon as InventoryWeaponStats)?.EquipmentBonus.DexterityBonus;
            sum += (int)(AccessoryWeapon as InventoryWeaponStats)?.EquipmentBonus.DexterityBonus;
            return sum;
        }
    }
    public int FinalAgility
    {
        get
        {
            int sum = Agility;
            sum += (int)(AttackWeapon as InventoryWeaponStats)?.EquipmentBonus.AgilityBonus;
            sum += (int)(HeadWeapon as InventoryWeaponStats)?.EquipmentBonus.AgilityBonus;
            sum += (int)(BodyWeapon as InventoryWeaponStats)?.EquipmentBonus.AgilityBonus;
            sum += (int)(FootWeapon as InventoryWeaponStats)?.EquipmentBonus.AgilityBonus;
            sum += (int)(AccessoryWeapon as InventoryWeaponStats)?.EquipmentBonus.AgilityBonus;
            return sum;
        }
    }
    public int FinalIntelligence
    {
        get
        {
            int sum = Intelligence;
            sum += (int)(AttackWeapon as InventoryWeaponStats)?.EquipmentBonus.IntelligenceBonus;
            sum += (int)(HeadWeapon as InventoryWeaponStats)?.EquipmentBonus.IntelligenceBonus;
            sum += (int)(BodyWeapon as InventoryWeaponStats)?.EquipmentBonus.IntelligenceBonus;
            sum += (int)(FootWeapon as InventoryWeaponStats)?.EquipmentBonus.IntelligenceBonus;
            sum += (int)(AccessoryWeapon as InventoryWeaponStats)?.EquipmentBonus.IntelligenceBonus;
            return sum;
        }
    }

    // [Header("計算パラメータ")]
    // /// <summary>
    // /// 最大HP: STR により上昇
    // /// </summary>
    // public override int MaxHP
    // {
    //     get
    //     {
    //         // 例) BaseHP + (Strength × HP_PER_STR)
    //         return (int)(BASE_HP + HP_PER_STR * FinalStrength);
    //     }
    // }

    // /// <summary>
    // /// 物理攻撃力ボーナス: STR(主), DEX(副) により上昇
    // /// </summary>
    // public override float PhysicalDamageBonus
    // {
    //     get
    //     {
    //         // 例) STRで主に上昇、DEXで少し上昇
    //         return (PDMG_STR_FACTOR * FinalStrength)
    //              + (PDMG_DEX_FACTOR * FinalDexterity);
    //     }
    // }

    // /// <summary>
    // /// クリティカル率 (%): DEX, AGI により上昇、最大70%
    // /// </summary>
    // public override float CriticalRate
    // {
    //     get
    //     {
    //         // 例) BaseCrit + DEX要素 + AGI要素、最大70%
    //         float crit = BASE_CRIT
    //                    + (CRIT_DEX_FACTOR * FinalDexterity)
    //                    + (CRIT_AGI_FACTOR * FinalAgility);

    //         return Mathf.Min(crit, MAX_CRIT);
    //     }
    // }

    // /// <summary>
    // /// スキルクールダウン倍率（0.5～1.0）: DEX により短縮、最低50%
    // /// ※ 実際の計算: スキルのベースクールダウン時間 × SkillCooldownRate
    // /// </summary>
    // public override float SkillCooldownRate
    // {
    //     get
    //     {
    //         // 例) DEX に応じて短縮、最低50%まで
    //         float rate = 1.0f - (COOL_DEX_FACTOR * FinalDexterity);
    //         return Mathf.Max(rate, MIN_COOLDOWN_RATIO);
    //     }
    // }

    // /// <summary>
    // /// 移動速度ボーナス（1.0 = 100%）: AGI により上昇、最大150%
    // /// </summary>
    // public override float MoveSpeedBonus
    // {
    //     get
    //     {
    //         // 例) AGI による移動速度上昇、最大150%
    //         float bonus = 1.0f + (MOV_AGI_FACTOR * FinalAgility);
    //         return Mathf.Min(bonus, MAX_MOV_BONUS);
    //     }
    // }

    // /// <summary>
    // /// 攻撃速度 (1.0 = 基本攻撃速度): DEX, AGI により上昇、最大1.8
    // /// </summary>
    // public override float AttackSpeed
    // {
    //     get
    //     {
    //         // 例) Dex, Agi 両方で増加し、最大1.8まで
    //         float speed = BASE_ATTACK_SPEED
    //                     + (AS_DEX_FACTOR * FinalDexterity)
    //                     + (AS_AGI_FACTOR * FinalAgility);

    //         return Mathf.Min(speed, MAX_ATTACK_SPEED);
    //     }
    // }
    // /// <summary>
    // /// 物理ダメージ軽減率 (%): STR, ArmorRating により上昇する.最大80%
    // /// </summary>
    // public override int PhysicalDamageReductionRate
    // {
    //     get
    //     {
    //         // 例) STRに応じて 0～80% 間で軽減率を決定
    //         float dr = BASE_DR + (DR_STR_FACTOR * FinalStrength + DR_ARMOR_FACTOR * ArmorRating);
    //         float clamped = Mathf.Min(dr, MAX_DR);
    //         // 0.8 であれば 80% として返す
    //         return Mathf.RoundToInt(clamped * 100f);
    //     }
    // }

    public override void CopyTo(BattleParameterBase dest)
    {
        if (dest == null) { return; }
        dest.Strength = Strength;
        dest.Dexterity = Dexterity;
        dest.Agility = Agility;
        dest.Intelligence = Intelligence;

        if (dest is BattleParameterBasePlayer destPlayer)
        {
            destPlayer.AttackWeapon = AttackWeapon;
            destPlayer.HeadWeapon = HeadWeapon;
            destPlayer.BodyWeapon = BodyWeapon;
            destPlayer.FootWeapon = FootWeapon;
            destPlayer.AccessoryWeapon = AccessoryWeapon;

            destPlayer.Level = Level;
            destPlayer.Exp = Exp;
            destPlayer.Money = Money;
        }
    }
}

[CreateAssetMenu(fileName = "BattleParameterPlayer", menuName = "DungeonAndSurvivors/BattleParameter/PlayerParameter", order = 2)]
public class BattleParameterPlayer : ScriptableObject
{
    public BattleParameterBasePlayer Data;
}