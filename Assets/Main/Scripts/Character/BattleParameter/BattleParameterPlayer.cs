using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

[Serializable]
public class BattleParameterBasePlayer : BattleParameterBase
{
    [Header("装備アイテム")]
    [SerializeField] private InventoryItem _attackWeapon;
    public InventoryItem AttackWeapon
    {
        get { return _attackWeapon; }
        set
        {
            _attackWeapon = value;
            RaiseOnParameterChanged();
        }
    }
    [SerializeField] private InventoryItem _headWeapon;
    public InventoryItem HeadWeapon
    {
        get { return _headWeapon; }
        set
        {
            _headWeapon = value;
            RaiseOnParameterChanged();
        }
    }
    [SerializeField] private InventoryItem _bodyWeapon;
    public InventoryItem BodyWeapon
    {
        get { return _bodyWeapon; }
        set
        {
            _bodyWeapon = value;
            RaiseOnParameterChanged();
        }
    }
    [SerializeField] private InventoryItem _footWeapon;
    public InventoryItem FootWeapon
    {
        get { return _footWeapon; }
        set
        {
            _footWeapon = value;
            RaiseOnParameterChanged();
        }
    }
    [SerializeField] private InventoryItem _accessoryWeapon;
    public InventoryItem AccessoryWeapon
    {
        get { return _accessoryWeapon; }
        set
        {
            _accessoryWeapon = value;
            RaiseOnParameterChanged();
        }
    }

    public override int EffectiveStrength { get { return Strength + GetEquipmentBonus(ParameterType.Strength); } }
    public override int EffectiveDexterity { get { return Dexterity + GetEquipmentBonus(ParameterType.Dexterity); } }
    public override int EffectiveAgility { get { return Agility + GetEquipmentBonus(ParameterType.Agility); } }
    public override int EffectiveIntelligence { get { return Intelligence + GetEquipmentBonus(ParameterType.Intelligence); } }

    [Header("計算パラメータ")]
    public override int MaxHP
    {
        get
        {
            // 例) BaseHP + (Strength × HP_PER_STR)
            return (int)(BASE_HP + HP_PER_STR * EffectiveStrength) + GetEquipmentBonus(ParameterType.MaxHP);
        }
    }
    /// <summary>
    /// 物理攻撃力ボーナス: STR(主), DEX(副) により上昇
    /// </summary>
    public override float PhysicalDamageBonus
    {
        get
        {
            // 例) STRで主に上昇、DEXで少し上昇
            return (PDMG_STR_FACTOR * EffectiveStrength)
                + (PDMG_DEX_FACTOR * EffectiveDexterity)
                + GetEquipmentBonus(ParameterType.PhysicalDamageBonus);
        }
    }

    /// <summary>
    /// クリティカル率 (%): DEX, AGI により上昇、最大70%
    /// </summary>
    public override float CriticalRate
    {
        get
        {
            // 例) BaseCrit + DEX要素 + AGI要素、最大70%
            float crit = BASE_CRIT
                       + (CRIT_DEX_FACTOR * EffectiveDexterity)
                       + (CRIT_AGI_FACTOR * EffectiveAgility)
                       + GetEquipmentBonus(ParameterType.CriticalRate);

            return Mathf.Min(crit, MAX_CRIT);
        }
    }

    /// <summary>
    /// スキルクールダウン倍率（0.5～1.0）: DEX により短縮、最低50%
    /// ※ 実際の計算: スキルのベースクールダウン時間 × SkillCooldownRate
    /// </summary>
    public override float SkillCooldownRate
    {
        get
        {
            // 例) DEX に応じて短縮、最低50%まで
            float rate = 1.0f - (COOL_DEX_FACTOR * EffectiveDexterity) + GetEquipmentBonus(ParameterType.SkillCooldownRate);
            return Mathf.Max(rate, MIN_COOLDOWN_RATIO);
        }
    }

    /// <summary>
    /// 移動速度ボーナス（1.0 = 100%）: AGI により上昇、最大150%
    /// </summary>
    public override float MoveSpeedBonus
    {
        get
        {
            // 例) AGI による移動速度上昇、最大150%
            float bonus = 1.0f + (MOV_AGI_FACTOR * EffectiveAgility) + GetEquipmentBonus(ParameterType.MoveSpeedBonus);
            return Mathf.Min(bonus, MAX_MOV_BONUS);
        }
    }

    /// <summary>
    /// 攻撃速度 (1.0 = 基本攻撃速度): DEX, AGI により上昇、最大1.8
    /// </summary>
    public override float AttackSpeed
    {
        get
        {
            // 例) Dex, Agi 両方で増加し、最大1.8まで
            float speed = BASE_ATTACK_SPEED
                        + (AS_DEX_FACTOR * EffectiveDexterity)
                        + (AS_AGI_FACTOR * EffectiveAgility)
                        + GetEquipmentBonus(ParameterType.AttackSpeed);

            return Mathf.Min(speed, MAX_ATTACK_SPEED);
        }
    }

    /// <summary>
    /// 物理ダメージ軽減率 (%): STR により上昇、最大80%
    /// </summary>
    public override int PhysicalDamageReductionRate
    {
        get
        {
            // 例) STRに応じて 0～80% 間で軽減率を決定
            float dr = BASE_DR + (DR_STR_FACTOR * EffectiveStrength);
            float clamped = Mathf.Min(dr, MAX_DR) + GetEquipmentBonus(ParameterType.PhysicalDamageReductionRate);
            // 0.8 であれば 80% として返す
            return Mathf.RoundToInt(clamped * 100f);
        }
    }

    // 魔法パラメータ仮実装
    public override int MaxMP
    {
        get
        {
            return (int)(BASE_MP + MP_PER_INT * EffectiveIntelligence) + GetEquipmentBonus(ParameterType.MaxMP);
        }
    }
    public override float MagicDamageBonus
    {
        get
        {
            return MDMG_INT_FACTOR * EffectiveIntelligence + GetEquipmentBonus(ParameterType.MagicDamageBonus);
        }
    }
    public override int MagicDamageReductionRate
    {
        get
        {
            float mr = BASE_MR + (MR_INT_FACTOR * EffectiveIntelligence);
            float clamped = Mathf.Min(mr, MAX_MR) + GetEquipmentBonus(ParameterType.MagicDamageReductionRate);
            return Mathf.RoundToInt(clamped * 100f);
        }
    }

    /// <summary>
    /// 指定された ParameterType に対して、全装備からボーナスを合算して返すヘルパー。
    /// 各装備が IParameterBonus を実装していれば、GetBonus を呼び出します。
    /// </summary>
    private int GetEquipmentBonus(ParameterType parameterType)
    {
        int bonus = 0;
        bonus += (AttackWeapon as IParameterBonus)?.GetBonus(parameterType) ?? 0;
        bonus += (HeadWeapon as IParameterBonus)?.GetBonus(parameterType) ?? 0;
        bonus += (BodyWeapon as IParameterBonus)?.GetBonus(parameterType) ?? 0;
        bonus += (FootWeapon as IParameterBonus)?.GetBonus(parameterType) ?? 0;
        bonus += (AccessoryWeapon as IParameterBonus)?.GetBonus(parameterType) ?? 0;
        return bonus;
    }

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
            armorRating += (AttackWeapon as InventoryWeaponStats)?.ArmorRating ?? 0;
            armorRating += (HeadWeapon as InventoryWeaponStats)?.ArmorRating ?? 0;
            armorRating += (BodyWeapon as InventoryWeaponStats)?.ArmorRating ?? 0;
            armorRating += (FootWeapon as InventoryWeaponStats)?.ArmorRating ?? 0;
            armorRating += (AccessoryWeapon as InventoryWeaponStats)?.ArmorRating ?? 0;
            return armorRating;
        }
    }


    public override void CopyTo(BattleParameterBase dest)
    {
        base.CopyTo(dest);

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