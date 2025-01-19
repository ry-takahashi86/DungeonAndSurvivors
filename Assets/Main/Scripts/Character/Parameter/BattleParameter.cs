using MoreMountains.InventoryEngine;
using UnityEngine;

[System.Serializable]
public class BattleParameterBase
{
    [Header("Base Parameters")]
    [Min(1)] public int HP;
    [Min(1)] public int MaxHP;
    [Min(1)] public int Strength;       // AttackPower
    [Min(1)] public int Dexterity;      // AttackSpeed
    [Min(1)] public int Agility;        // MoveSpeed, CriticalRate
    [Min(1)] public int Intelligence;   // MP, MagicPower

    [Header("Status")]
    [Min(1)] public int Level;
    [Min(0)] public int Exp;
    [Min(0)] public int Money;

    [Header("Equipment")]
    public InventoryItem AttackWeapon;
    public InventoryItem HeadWeapon;
    public InventoryItem BodyWeapon;
    public InventoryItem FootWeapon;
    public InventoryItem AccessoryWeapon;

    [Header("Calculated Parameters")]
    public int AttackPower { get { return Strength; } }
    public int DefensePower
    {
        get
        {
            int defensePower = Strength + Dexterity;
            defensePower += (HeadWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            defensePower += (BodyWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            defensePower += (FootWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            defensePower += (AccessoryWeapon as InventoryWeaponStats)?.DefensePower ?? 0;
            return defensePower;
        }
    }

    public virtual void CopyTo(BattleParameterBase dest)
    {
        dest.HP = HP;
        dest.MaxHP = HP < MaxHP ? MaxHP : HP;
        dest.Strength = Strength;
        dest.Dexterity = Dexterity;
        dest.Agility = Agility;
        dest.Intelligence = Intelligence;
        dest.Level = Level;
        dest.Exp = Exp;
        dest.Money = Money;
    }
}

[CreateAssetMenu(fileName = "BattleParameter", menuName = "DungeonAndSurvivors/BattleParameter")]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}