using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterHandleWeaponPlayer : CharacterHandleWeapon
{
    public override void ChangeWeapon(Weapon newWeapon, string weaponID, bool combo = false)
    {
        // 装備がないときは初期武器をセットする
        if (newWeapon == null)
        {
            newWeapon = InitialWeapon;
        }

        base.ChangeWeapon(newWeapon, weaponID, combo);
    }
}