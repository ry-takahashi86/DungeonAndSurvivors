using System;
using UnityEngine;

[Serializable]
public class BattleParameterBase
{
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

    // MaxHP
    protected const int   BASE_MP         = 0;   // 初期HP
    protected const float MP_PER_INT      = 2f;   // INT 1pt あたりのMP上昇量

    // PhysicalDamageBonus
    protected const float MDMG_INT_FACTOR = 2f;    // INT の寄与度

    // PhysicalDamageReductionRate（魔法ダメージ軽減率％）
    protected const float BASE_MR       = 0f;       // 初期は0%
    protected const float MR_INT_FACTOR = 0.002f;   // INT 1pt ごとに0.2%軽減率上昇
    protected const float MAX_MR        = 0.80f;    // 最大80%軽減
    #endregion

    [Header("基礎パラメータ")]

    // 基礎パラメータの初期値
    [Min(0)] [SerializeField] private int _strength;       // 物理面のパワーと耐久力
    [Min(0)] [SerializeField] private int _dexterity;      // 命中率・器用さ・クリティカル関連
    [Min(0)] [SerializeField] private int _agility;        // 回避・機動力（移動速度・攻撃速度）
    [Min(0)] [SerializeField] private int _intelligence;   // 魔法・スキル関連

    // パラメータ更新時の発火イベントを定義
    public event Action OnParameterChanged;
    protected void RaiseOnParameterChanged()
    {
        OnParameterChanged?.Invoke();
    }
    // 基礎パラメータ更新用
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

    // 基礎パラメータに対する「有効な」値（ベース値 + 装備アイテム値)
    public virtual int EffectiveStrength { get { return Strength; }}
    public virtual int EffectiveDexterity { get { return Dexterity; }}
    public virtual int EffectiveAgility { get { return Agility; }}
    public virtual int EffectiveIntelligence { get { return Intelligence; }}

    [Header("計算パラメータ")]
    /// <summary>
    /// 最大HP: STR により上昇
    /// </summary>
    public virtual int MaxHP
    {
        get
        {
            // 例) BaseHP + (Strength × HP_PER_STR)
            return (int)(BASE_HP + HP_PER_STR * EffectiveStrength);
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
            return (PDMG_STR_FACTOR * EffectiveStrength)
                 + (PDMG_DEX_FACTOR * EffectiveDexterity);
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
                       + (CRIT_DEX_FACTOR * EffectiveDexterity)
                       + (CRIT_AGI_FACTOR * EffectiveAgility);

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
            float rate = 1.0f - (COOL_DEX_FACTOR * EffectiveDexterity);
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
            float bonus = 1.0f + (MOV_AGI_FACTOR * EffectiveAgility);
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
                        + (AS_DEX_FACTOR * EffectiveDexterity)
                        + (AS_AGI_FACTOR * EffectiveAgility);

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
            float dr = BASE_DR + (DR_STR_FACTOR * EffectiveStrength);
            float clamped = Mathf.Min(dr, MAX_DR);
            // 0.8 であれば 80% として返す
            return Mathf.RoundToInt(clamped * 100f);
        }
    }

    // 魔法パラメータ仮実装
    public virtual int MaxMP
    {
        get
        {
            return (int)(BASE_MP + MP_PER_INT * EffectiveIntelligence);
        }
    }
    public virtual float MagicDamageBonus
    {
        get
        {
            return MDMG_INT_FACTOR * EffectiveIntelligence;
        }
    }
    public virtual int MagicDamageReductionRate
    {
        get
        {
            float mr = BASE_MR + (MR_INT_FACTOR * EffectiveIntelligence);
            float clamped = Mathf.Min(mr, MAX_MR);
            return Mathf.RoundToInt(clamped * 100f);
        }
    }

    public virtual void CopyTo(BattleParameterBase dest)
    {
        if (dest == null) { return; }
        dest._strength = _strength;
        dest._dexterity = _dexterity;
        dest._agility = _agility;
        dest._intelligence = _intelligence;
    }
}

[CreateAssetMenu(fileName = "BattleParameterBase", menuName = "DungeonAndSurvivors/BattleParameter/BaseParameter", order = 1)]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}