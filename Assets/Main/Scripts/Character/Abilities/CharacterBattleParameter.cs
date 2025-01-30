using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterBattleParameter : CharacterAbility
{
    public enum CharacterTypes { Player, AI }

    [Header("キャラクタータイプ")]
    public CharacterTypes CharacterType = CharacterTypes.AI;

    [Header("バトルパラメータ")]
    public BattleParameter InitialBattleParameter;
    public BattleParameterPlayer InitialBattleParameterPlayer;
    public BattleParameterBasePlayer BattleParameter;

    protected Weapon _weapon;

    protected override void Start()
    {
        base.Start();

        // 初期パラメータ を反映
        if (InitialBattleParameter != null)
        {
            InitialBattleParameter.Data.CopyTo(BattleParameter);
        }
        else if (InitialBattleParameterPlayer != null)
        {
            InitialBattleParameterPlayer.Data.CopyTo(BattleParameter);
        }

        // パラメータを適用
        ApplyBattleParameter();

        // HPを最大値にリセット
        _health.InitialHealth = BattleParameter.MaxHP;
        _health.ResetHealthToMaxHealth();
    }

    public void ApplyBattleParameter()
    {
        if (BattleParameter == null) { return; }

        // 他のコンポーネントにパラメータを渡す
        // 例) HP, MP, 移動速度, 攻撃速度, スキルクールダウンなど
        if (_health != null)
        {
            _health.MaximumHealth = BattleParameter.MaxHP;
            Debug.Log($"最大HP: {_health.MaximumHealth}");
        }

        if (_characterMovement != null)
        {
            _characterMovement.MovementSpeed = _characterMovement.WalkSpeed * BattleParameter.MoveSpeedBonus;
            Debug.Log($"移動速度: {_characterMovement.MovementSpeed}");
        }

        // 攻撃速度、スキルクールダウンなども同様に反映
        _weapon = GetComponentInChildren<Weapon>();
        if (_weapon != null)
        {
            _weapon.TimeBetweenUses = 2f - BattleParameter.AttackSpeed;
            Debug.Log($"攻撃速度: {_weapon.TimeBetweenUses}");
        }
    }
}