using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleParameterPlayer", menuName = "DungeonAndSurvivors/BattleParameter/PlayerParameter", order = 2)]
public class BattleParameterPlayer : BattleParameter
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

    public override int EffectiveStrength { get { return Strength + (int)GetEquipmentBonus(ParameterType.Strength); } }
    public override int EffectiveDexterity { get { return Dexterity + (int)GetEquipmentBonus(ParameterType.Dexterity); } }
    public override int EffectiveAgility { get { return Agility + (int)GetEquipmentBonus(ParameterType.Agility); } }
    public override int EffectiveIntelligence { get { return Intelligence + (int)GetEquipmentBonus(ParameterType.Intelligence); } }

    [Header("計算パラメータ")]
    public override int MaxHP
    {
        get
        {
            return Mathf.RoundToInt(base.MaxHP * (100 + GetEquipmentBonus(ParameterType.MaxHP)) / 100);
        }
    }
    public override float PhysicalDamageBonus
    {
        get
        {
            return base.PhysicalDamageBonus + GetEquipmentBonus(ParameterType.PhysicalDamageBonus);
        }
    }
    public override float CriticalRate
    {
        get
        {
            return Mathf.Min(base.CriticalRate + GetEquipmentBonus(ParameterType.CriticalRate), MAX_CRIT);
        }
    }
    public override float SkillCooldownRate
    {
        get
        {
            float baseSCD = base.SkillCooldownRate;
            return Mathf.Min(base.SkillCooldownRate + GetEquipmentBonus(ParameterType.SkillCooldownRate), MAX_COOLDOWN);
        }
    }
    public override float MoveSpeed
    {
        get
        {
            float baseMS = Mathf.Min(BASE_MS_PLAYER + MS_AGI_FACTOR * EffectiveAgility, MAX_MS);
            return Mathf.Min(baseMS + baseMS / 100 * GetEquipmentBonus(ParameterType.MoveSpeedBonus), MAX_MS);
        }
    }
    public override float AttackSpeed
    {
        get
        {
            float baseAS = base.AttackSpeed;
            return Mathf.Min(base.AttackSpeed + GetEquipmentBonus(ParameterType.AttackSpeed), MAX_ATTACK_SPEED);
        }
    }
    public override float PhysicalDamageReductionRate
    {
        get
        {
            float armorBonus = PDR_ARMOR_FACTOR * ArmorRating;
            float basePDR = base.PhysicalDamageReductionRate;
            return Mathf.Min(basePDR + armorBonus + (basePDR / 100 * GetEquipmentBonus(ParameterType.PhysicalDamageReductionRate)), MAX_PDR);
        }
    }
    public override int MaxMP
    {
        get
        {
            return Mathf.RoundToInt(base.MaxMP * (100 + GetEquipmentBonus(ParameterType.MaxMP)) / 100);
        }
    }
    public override float MagicDamageBonus
    {
        get
        {
            float baseMDB = base.MagicDamageBonus;
            return base.MagicDamageBonus + GetEquipmentBonus(ParameterType.MagicDamageBonus);
        }
    }
    public override float MagicDamageReductionRate
    {
        get
        {
            float baseMDR = base.MagicDamageReductionRate;
            return Mathf.Min(base.MagicDamageReductionRate + GetEquipmentBonus(ParameterType.MagicDamageReductionRate), MAX_MDR);
        }
    }

    /// <summary>
    /// 指定された ParameterType に対して、全装備からボーナスを合算して返すヘルパー。
    /// 各装備が IParameterBonus を実装していれば、GetBonus を呼び出します。
    /// </summary>
    private float GetEquipmentBonus(ParameterType parameterType)
    {
        float bonus = 0;
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

}