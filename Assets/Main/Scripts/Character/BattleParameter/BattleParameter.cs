using System;
using UnityEngine;

[Serializable]
public class BattleParameterBase
{
    public event Action OnParameterChanged;

    [Header("基礎パラメータ")]
    [Min(0)] [SerializeField] private int _strength;       // 物理面のパワーと耐久力
    public virtual int Strength
    {
        get => _strength;
        set
        {
            if (_strength != value)
            {
                _strength = value;
                OnParameterChanged?.Invoke();   // イベント通知
            }
        }
    }
    [Min(0)] [SerializeField] private int _dexterity;      // 命中率・器用さ・クリティカル関連
    public virtual int Dexterity
    {
        get => _dexterity;
        set
        {
            if (_dexterity != value)
            {
                _dexterity = value;
                OnParameterChanged?.Invoke();   // イベント通知
            }
        }
    }
    [Min(0)] [SerializeField] private int _agility;        // 回避・機動力（移動速度・攻撃速度）
    public virtual int Agility
    {
        get => _agility;
        set
        {
            if (_agility != value)
            {
                _agility = value;
                OnParameterChanged?.Invoke();   // イベント通知
            }
        }
    }
    [Min(0)] [SerializeField] private int _intelligence;   // MaxMP, MagicDamageBonus, MagicDamageReductionRate
    public virtual int Intelligence
    {
        get => _intelligence;
        set
        {
            if (_intelligence != value)
            {
                _intelligence = value;
                OnParameterChanged?.Invoke();   // イベント通知
            }
        }
    }

    # region パラメータ定数

    // MaxHP
    protected const int   BASE_HP         = 10;   // 初期HP
    protected const float HP_PER_STR      = 2f;   // STR 1pt あたりのHP上昇量

    // PhysicalDamageBonus
    protected const float PDMG_STR_FACTOR = 2f;    // STR の寄与度
    protected const float PDMG_DEX_FACTOR = 0.5f;  // DEX の寄与度（少しだけダメージに影響させる例）

    // CriticalRate (％)
    protected const float BASE_CRIT       = 5f;    // 初期クリ率5%
    protected const float CRIT_DEX_FACTOR = 0.3f;  // DEX 1pt あたり +0.3%
    protected const float CRIT_AGI_FACTOR = 0.1f;  // AGI 1pt あたり +0.1%（敏捷さによる微増）
    protected const float MAX_CRIT        = 70f;   // 最大 70% に制限する例

    // SkillCooldownRate（スキルクールダウン短縮率）
    // ※ "スキルクールダウン時間 × SkillCooldownRate" のように扱う想定
    protected const float MIN_COOLDOWN_RATIO = 0.5f;   // 50% (半分) まで短縮するのが限度
    protected const float COOL_DEX_FACTOR    = 0.001f; // DEX 1pt ごとに0.1%短縮する例

    // MoveSpeedBonus（移動速度倍率）
    protected const float MOV_AGI_FACTOR = 0.03f;  // AGI1pt ごとに+3%上昇
    protected const float MAX_MOV_BONUS  = 1.5f;   // 移動速度は最大150%に制限

    // AttackSpeed（攻撃速度：1秒当たり何回攻撃するか、等）
    protected const float BASE_ATTACK_SPEED = 1.0f;   // 初期攻撃速度(1.0 = 1秒1回など)
    protected const float AS_DEX_FACTOR     = 0.005f; // DEX 1pt あたり攻撃速度 +0.005
    protected const float AS_AGI_FACTOR     = 0.01f;  // AGI 1pt あたり攻撃速度 +0.01
    protected const float MAX_ATTACK_SPEED  = 1.8f;   // 1.8 = 1秒1.8回攻撃が上限

    // PhysicalDamageReductionRate（物理ダメージ軽減率％）
    protected const float BASE_DR       = 0f;       // 初期は0%
    protected const float DR_STR_FACTOR = 0.002f;   // STR 1pt ごとに0.2%軽減率上昇
    protected const float DR_ARMOR_FACTOR = 0.003f; // 防具の防御力 1pt ごとに0.26%軽減率上昇
    protected const float MAX_DR        = 0.80f;    // 最大80%軽減

    #endregion

    # region // 計算パラメータ（getter）

    [Header("計算パラメータ")]
    /// <summary>
    /// 最大HP: STR により上昇
    /// </summary>
    public virtual int MaxHP
    {
        get
        {
            // 例) BaseHP + (Strength × HP_PER_STR)
            return (int)(BASE_HP + HP_PER_STR * Strength);
        }
    }

    /// <summary>
    /// 物理攻撃力ボーナス: STR(主), DEX(副) により上昇
    /// </summary>
    public virtual float PhysicalDamageBonus
    {
        get
        {
            // 例) STRで主に上昇、DEXで少し上昇
            return (PDMG_STR_FACTOR * Strength)
                 + (PDMG_DEX_FACTOR * Dexterity);
        }
    }

    /// <summary>
    /// クリティカル率 (%): DEX, AGI により上昇、最大70%
    /// </summary>
    public virtual float CriticalRate
    {
        get
        {
            // 例) BaseCrit + DEX要素 + AGI要素、最大70%
            float crit = BASE_CRIT
                       + (CRIT_DEX_FACTOR * Dexterity)
                       + (CRIT_AGI_FACTOR * Agility);

            return Mathf.Min(crit, MAX_CRIT);
        }
    }

    /// <summary>
    /// スキルクールダウン倍率（0.5～1.0）: DEX により短縮、最低50%
    /// ※ 実際の計算: スキルのベースクールダウン時間 × SkillCooldownRate
    /// </summary>
    public virtual float SkillCooldownRate
    {
        get
        {
            // 例) DEX に応じて短縮、最低50%まで
            float rate = 1.0f - (COOL_DEX_FACTOR * Dexterity);
            return Mathf.Max(rate, MIN_COOLDOWN_RATIO);
        }
    }

    /// <summary>
    /// 移動速度ボーナス（1.0 = 100%）: AGI により上昇、最大150%
    /// </summary>
    public virtual float MoveSpeedBonus
    {
        get
        {
            // 例) AGI による移動速度上昇、最大150%
            float bonus = 1.0f + (MOV_AGI_FACTOR * Agility);
            return Mathf.Min(bonus, MAX_MOV_BONUS);
        }
    }

    /// <summary>
    /// 攻撃速度 (1.0 = 基本攻撃速度): DEX, AGI により上昇、最大1.8
    /// </summary>
    public virtual float AttackSpeed
    {
        get
        {
            // 例) Dex, Agi 両方で増加し、最大1.8まで
            float speed = BASE_ATTACK_SPEED
                        + (AS_DEX_FACTOR * Dexterity)
                        + (AS_AGI_FACTOR * Agility);

            return Mathf.Min(speed, MAX_ATTACK_SPEED);
        }
    }

    /// <summary>
    /// 物理ダメージ軽減率 (%): STR により上昇、最大80%
    /// </summary>
    public virtual int PhysicalDamageReductionRate
    {
        get
        {
            // 例) STRに応じて 0～80% 間で軽減率を決定
            float dr = BASE_DR + (DR_STR_FACTOR * Strength);
            float clamped = Mathf.Min(dr, MAX_DR);
            // 0.8 であれば 80% として返す
            return Mathf.RoundToInt(clamped * 100f);
        }
    }
    public virtual int MaxMP { get { return Intelligence; } }
    public virtual int MagicDamageBonus { get { return Intelligence; } }
    public virtual int MagicDamageReductionRate { get { return Intelligence; } }

    #endregion

    public virtual void CopyTo(BattleParameterBase dest)
    {
        if (dest == null) { return; }
        dest.Strength = Strength;
        dest.Dexterity = Dexterity;
        dest.Agility = Agility;
        dest.Intelligence = Intelligence;
    }
}

[CreateAssetMenu(fileName = "BattleParameterBase", menuName = "DungeonAndSurvivors/BattleParameter/BaseParameter", order = 1)]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}