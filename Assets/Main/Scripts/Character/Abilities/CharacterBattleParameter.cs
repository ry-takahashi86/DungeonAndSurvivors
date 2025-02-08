using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterBattleParameter : CharacterAbility
{
    public enum CharacterTypes { Player, AI }

    [Header("キャラクタータイプ")]
    public CharacterTypes CharacterType = CharacterTypes.AI;

    [Header("バトルパラメータ")]
    public BattleParameter InitialBattleParameter;
    public BattleParameter BattleParameter;

    protected Weapon _weapon;

    protected override void Start()
    {
        base.Start();

        if (InitialBattleParameter != null)
        {
            // AIの場合は直接渡す
            if (CharacterType == CharacterTypes.AI)
            {
                BattleParameter = InitialBattleParameter;
            }
            // Playerの場合は、複製して初期パラメータが上書きされないようにする
            else if (CharacterType == CharacterTypes.Player)
            {
                BattleParameter = Instantiate(InitialBattleParameter);
            }
        }

        // パラメータを適用
        ApplyBattleParameter();

        // HPを最大値にリセット
        _health.InitialHealth = BattleParameter.MaxHP;
        _health.ResetHealthToMaxHealth();
    }

    /// <summary>
    /// 最大HP,移動速度,攻撃速度を各パラメータに反映する
    /// </summary>
    public void ApplyBattleParameter()
    {
        if (BattleParameter == null) { return; }

        // Debug.Log("バトルパラメータが更新されました------------------------------------");
        // Debug.Log($"Strength: {BattleParameter.EffectiveStrength}");
        // Debug.Log($"Dexterity: {BattleParameter.EffectiveDexterity}");
        // Debug.Log($"Agility: {BattleParameter.EffectiveAgility}");
        // Debug.Log($"Intelligence: {BattleParameter.EffectiveIntelligence}");

        // 最大HPを反映
        if (_health != null)
        {
            _health.MaximumHealth = BattleParameter.MaxHP;
        }

        // 移動速度を反映
        if (_characterMovement != null)
        {
            _characterMovement.WalkSpeed = BattleParameter.MoveSpeed;
            _characterMovement.MovementSpeed = _characterMovement.WalkSpeed;
        }

        // 攻撃速度を反映
        if (_handleWeaponList != null)
        {
            for (int i=0; i<_handleWeaponList.Count; i++)
            {
                Weapon weapon = _handleWeaponList[i].CurrentWeapon;
                if (weapon is MeleeWeaponStats meleeWeapon)
                {
                    meleeWeapon.DelayBeforeUse = Mathf.Max(meleeWeapon.InitialAttackDelay * (1f - BattleParameter.AttackSpeed / 100), 0);
                    meleeWeapon.TimeBetweenUses = Mathf.Max(meleeWeapon.InitialAttackSpeed * (1f - BattleParameter.AttackSpeed / 100), 0);
                }
                else if (weapon is ProjectileWeaponStats projectileWeapon)
                {
                    projectileWeapon.DelayBeforeUse = Mathf.Max(projectileWeapon.InitialAttackDelay * (1f - BattleParameter.AttackSpeed / 100), 0);
                    projectileWeapon.TimeBetweenUses = Mathf.Max(projectileWeapon.InitialAttackSpeed * (1f - BattleParameter.AttackSpeed / 100), 0);
                }
            }
        }
    }
}