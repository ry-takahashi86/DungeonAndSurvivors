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