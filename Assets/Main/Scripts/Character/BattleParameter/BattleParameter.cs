using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleParameterBase", menuName = "DungeonAndSurvivors/BattleParameter/BaseParameter", order = 1)]
public class BattleParameter: ScriptableObject
{
    # region パラメータ定数

    // MaxHP(最大HP)
    protected const int   BASE_HP           = 10;       // 基準HP
    protected const float HP_PER_STR        = 2f;       // 1STR あたりのHP上昇量

    // PhysicalDamageBonus(物理ダメージボーナス)
    protected const float PDMG_STR_FACTOR   = 2f;       // 1STR あたりの物理ダメージボーナス上昇量
    protected const float PDMG_DEX_FACTOR   = 0.5f;     // 1DEX あたりの物理ダメージボーナス上昇量

    // CriticalRate(クリティカル率)
    protected const float BASE_CRIT         = 5f;       // 基準クリティカル率
    protected const float CRIT_DEX_FACTOR   = 0.3f;     // 1DEX あたりのクリティカル率上昇量
    protected const float CRIT_AGI_FACTOR   = 0.1f;     // 1AGI あたりのクリティカル率上昇量
    protected const float MAX_CRIT          = 70f;      // 最大クリティカル率

    // SkillCooldownReductionRate(スキルクールダウン減少率)
    protected const float COOL_DEX_FACTOR   = 0.3f;     // 1DEX あたりのクールダウン減少率上昇量
    protected const float MAX_COOLDOWN      = 50f;      // 最大クールダウン減少率(50%)

    // MoveSpeed(移動速度)
    protected const float BASE_MS_ENEMY     = 1.0f;     // 基準移動速度(Enemy用)
    protected const float BASE_MS_PLAYER    = 4.0f;     // 基準移動速度(Player用)
    protected const float MS_AGI_FACTOR     = 0.05f;    // 1AGI あたりの移動速度上昇量
    protected const float MAX_MS            = 5.5f;     // 最大移動速度

    // AttackSpeed(攻撃速度)
    protected const float BASE_AS           = 0f;       // 基準攻撃速度
    protected const float AS_DEX_FACTOR     = 0.3f;     // 1DEX あたり攻撃速度上昇量
    protected const float AS_AGI_FACTOR     = 0.1f;     // 1AGI あたり攻撃速度上昇量
    protected const float MAX_ATTACK_SPEED  = 80f;      // 最大攻撃速度

    // PhysicalDamageReductionRate(物理ダメージ軽減率)
    protected const float BASE_PDR          = 0f;       // 基準物理ダメージ軽減率
    protected const float PDR_STR_FACTOR    = 0.2f;     // 1STR あたりの物理ダメージ軽減率上昇量
    protected const float PDR_ARMOR_FACTOR  = 0.3f;     // 1AR(防御等級) あたりの物理ダメージ軽減率上昇量
    protected const float MAX_PDR           = 80f;      // 最大物理ダメージ軽減率(80%)

    // MaxHP(最大MP)
    protected const int   BASE_MP           = 0;        // 基準最大MP
    protected const float MP_PER_INT        = 2f;       // 1INT あたりの最大MP上昇量

    // MagicDamageBonus(魔法ダメージボーナス)
    protected const float MDMG_INT_FACTOR   = 2f;       // 1INT あたりの魔法ダメージボーナス上昇量

    // MagicDamageReductionRate(魔法ダメージ軽減率)
    protected const float BASE_MDR          = 0f;       // 基準魔法ダメージ軽減率
    protected const float MDR_INT_FACTOR    = 0.3f;     // 1INT あたりの魔法ダメージ軽減率上昇量
    protected const float MAX_MDR           = 80f;      // 最大魔法ダメージ軽減率(80%)

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

    // 基礎パラメータとして利用する最終的な値
    public virtual int EffectiveStrength { get { return Strength; }}
    public virtual int EffectiveDexterity { get { return Dexterity; }}
    public virtual int EffectiveAgility { get { return Agility; }}
    public virtual int EffectiveIntelligence { get { return Intelligence; }}

    [Header("計算パラメータ")]
    // 最大HP: Strength により上昇
    public virtual int MaxHP { get { return Mathf.RoundToInt(BASE_HP + HP_PER_STR * EffectiveStrength); } }

    // 物理攻撃力ボーナス(%): Strength(主), Dexterity(副) により上昇
    public virtual float PhysicalDamageBonus { get { return PDMG_STR_FACTOR * EffectiveStrength + PDMG_DEX_FACTOR * EffectiveDexterity; } }

    // クリティカル率(%): Dexterity(主), Agility(副) により上昇(上限あり)
    public virtual float CriticalRate { get { return Mathf.Min(BASE_CRIT + CRIT_DEX_FACTOR * EffectiveDexterity + CRIT_AGI_FACTOR * EffectiveAgility, MAX_CRIT); } }

    // スキルクールダウン減少率(%): Dexterity により上昇(上限あり)
    public virtual float SkillCooldownRate { get { return Mathf.Min(COOL_DEX_FACTOR * EffectiveDexterity, MAX_COOLDOWN); } }

    // 移動速度: Agility により上昇(上限あり)
    public virtual float MoveSpeed { get { return Mathf.Min(BASE_MS_ENEMY + MS_AGI_FACTOR * EffectiveAgility, MAX_MS); } }

    // 攻撃速度: Dexterity(主), Agility(副) により上昇(上限あり)
    public virtual float AttackSpeed { get { return Mathf.Min(BASE_AS + AS_DEX_FACTOR * EffectiveDexterity + AS_AGI_FACTOR * EffectiveAgility, MAX_ATTACK_SPEED); } }

    // 物理ダメージ軽減率(%): Strength により上昇(上限あり)
    public virtual float PhysicalDamageReductionRate { get { return Mathf.Min(BASE_PDR + PDR_STR_FACTOR * EffectiveStrength, MAX_PDR); } }

    // 最大MP: Intelligence により上昇
    public virtual int MaxMP { get { return Mathf.RoundToInt(BASE_MP + MP_PER_INT * EffectiveIntelligence); } }

    // 魔法ダメージボーナス(%): Intelligence により上昇
    public virtual float MagicDamageBonus { get { return MDMG_INT_FACTOR * EffectiveIntelligence; } }

    // 魔法ダメージ減少率(%): Intelligence により上昇
    public virtual float MagicDamageReductionRate { get { return Mathf.Min(BASE_MDR + MDR_INT_FACTOR * EffectiveIntelligence, MAX_MDR); } }
}