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

        Debug.Log("バトルパラメータが更新されました-------------------");

        Debug.Log("基礎パラメータ------------------------------------");
        Debug.Log($"Strength: {BattleParameter.EffectiveStrength}");
        Debug.Log($"Dexterity: {BattleParameter.EffectiveDexterity}");
        Debug.Log($"Agility: {BattleParameter.EffectiveAgility}");
        Debug.Log($"Intelligence: {BattleParameter.EffectiveIntelligence}");

        Debug.Log("計算パラメータ------------------------------------");
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
            Debug.Log($"移動速度ボーナス: { BattleParameter.MoveSpeedBonus}");
        }
    }
}