using UnityEngine;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;

// ----------------------------
// BonusCandidate 定義
// ----------------------------
[System.Serializable]
public struct BonusCandidate
{
    public ParameterType parameterType;
    public int minValue;
    public int maxValue;

    public BonusCandidate(ParameterType type, int min, int max)
    {
        parameterType = type;
        minValue = min;
        maxValue = max;
    }
}

public class ItemPickerDrop : ItemPicker
{
    [Header("Item Database")]
    public InventoryWeaponStatsDataBase InventoryWeaponStatsDataBase;

    [Header("Item Data")]
    public InventoryWeaponStats InitialItemData;

    /// <summary>
    /// レアリティ出現確率表
    /// 例: [60, 85, 95, 99, 100] → 60%:Common, 25%:Uncommon, 10%:Rare, 4%:Epic, 1%:Legendary
    /// </summary>
    private readonly float[] _rarityThresholds = new float[] { 60f, 85f, 95f, 99f, 100f };

    /// <summary>
    /// レアリティ別の追加ボーナス数
    /// </summary>
    private Dictionary<Rarity, int> _rarityBonusCount = new Dictionary<Rarity, int>()
    {
        { Rarity.Common,      0 },
        { Rarity.Uncommon,    1 },
        { Rarity.Rare,        2 },
        { Rarity.Epic,        3 },
        { Rarity.Legendary,   4 }
    };

    /// <summary>
    /// レアリティ別のステータス補正（例）
    /// ダメージと防御力に対して倍率 or 加算を定義
    /// </summary>
    private Dictionary<Rarity, int> _rarityDamageMultiplier = new Dictionary<Rarity, int>()
    {
        { Rarity.Common,    0 },    // 補正なし
        { Rarity.Uncommon,  2 },    // 2アップ
        { Rarity.Rare,      4 },    // 4アップ
        { Rarity.Epic,      6 },    // 6アップ
        { Rarity.Legendary, 10 },   // 10アップ
    };

    private Dictionary<Rarity, int> _rarityArmorMultiplier = new Dictionary<Rarity, int>()
    {
        { Rarity.Common,    0 },    // 補正なし
        { Rarity.Uncommon,  2 },    // 2アップ
        { Rarity.Rare,      4 },    // 4アップ
        { Rarity.Epic,      6 },    // 6アップ
        { Rarity.Legendary, 10 },   // 10アップ
    };

    /// <summary>
    /// 追加ボーナス候補一覧
    /// パラメータ種別と最小・最大値を設定しておき、抽選時に参照
    /// </summary>
    [Header("Bonus Candidates")]
    public List<BonusCandidate> bonusCandidates = new List<BonusCandidate>
    {
        new BonusCandidate(ParameterType.Strength, 1, 5),
        new BonusCandidate(ParameterType.Dexterity, 1, 5),
        new BonusCandidate(ParameterType.Agility, 1, 5),
        new BonusCandidate(ParameterType.Intelligence, 1, 5),

        new BonusCandidate(ParameterType.MaxHP, 5, 30),
        new BonusCandidate(ParameterType.PhysicalDamageBonus, 1, 10),
        new BonusCandidate(ParameterType.CriticalRate, 1, 5),
        new BonusCandidate(ParameterType.SkillCooldownRate, 1, 3),
        new BonusCandidate(ParameterType.MoveSpeedBonus, 1, 3),
        new BonusCandidate(ParameterType.AttackSpeed, 1, 5),
        new BonusCandidate(ParameterType.PhysicalDamageReductionRate, 1, 10),

        new BonusCandidate(ParameterType.MaxMP, 1, 3),
        new BonusCandidate(ParameterType.MagicDamageBonus, 1, 5),
        new BonusCandidate(ParameterType.MagicDamageReductionRate, 1, 10),
    };

    protected override void Initialization()
    {
        if (Item == null)
        {
            RandomizeItem();
        }
        base.Initialization();
    }

    /// <summary>
    /// ランダムなアイテムデータを定義する
    /// </summary>
    public void RandomizeItem()
    {
        // 1. アイテムデータベースからアイテムデータをランダムに取得
        int randomIndex = Random.Range(0, InventoryWeaponStatsDataBase.InventoryItems.Count);
        InitialItemData = InventoryWeaponStatsDataBase.InventoryItems[randomIndex];

        Item = Instantiate(InitialItemData);

        // 2. レアリティをランダム決定
        Rarity newRarity = GetRandomRarity();

        // 3. 抽選されたアイテムが InventoryWeaponStats なら、レアリティ & ボーナス反映
        if (Item is InventoryWeaponStats weaponStats)
        {
            // 3-1. レアリティ設定
            weaponStats.ItemRarity = newRarity;

            // 3-2. レアリティに応じたダメージ・防御力の補正
            int dmgMultiplier = _rarityDamageMultiplier[newRarity];
            int armorMultiplier = _rarityArmorMultiplier[newRarity];

            if (ItemClasses.Weapon == weaponStats.ItemClass)
            {
            weaponStats.WeaponMinDamage = weaponStats.WeaponMinDamage + dmgMultiplier;
            weaponStats.WeaponMaxDamage = weaponStats.WeaponMaxDamage + dmgMultiplier;
            }
            else if (ItemClasses.Armor == weaponStats.ItemClass)
            {
                weaponStats.ArmorRating = weaponStats.ArmorRating + armorMultiplier;
            }

            // 3-3. 既存ボーナスリストをクリア or リセット
            weaponStats.AdditionalBonuses.Clear();

            // 3-4. レアリティに応じたボーナス数を取得
            int bonusCount = _rarityBonusCount[newRarity];

            // 重複しないようにパラメータTypeを管理するためのセット
            HashSet<ParameterType> usedParameters = new HashSet<ParameterType>();

            // 3-5. ボーナスを抽選 & 追加
            int loopCount = 0; // 無限ループ防止用
            for (int i = 0; i < bonusCount; i++)
            {
                if (loopCount > 1000)
                {
                    // 安全策: ある程度回しても抽選できなければ打ち切り
                    break;
                }
                loopCount++;

                ItemBonus bonus = GetRandomBonusFromCandidates();
                // 重複があるかチェック
                if (!usedParameters.Contains(bonus.ParameterType))
                {
                    // 重複なし → 追加
                    weaponStats.AdditionalBonuses.Add(bonus);
                    usedParameters.Add(bonus.ParameterType);
                }
                else
                {
                    // 重複 → この i の抽選をやり直したいので、カウンタをデクリメント
                    i--;
                    continue;
                }
            }

            // 最終結果をDescriptionに反映させる
            weaponStats.UpdateDescription();

            // 4. スプライト設定
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && weaponStats != null)
            {
                spriteRenderer.sprite = Item.Icon;
            }
        }
    }

    /// <summary>
    /// レアリティを確率的に決定する
    /// </summary>
    /// <returns></returns>
    private Rarity GetRandomRarity()
    {
        // 例: _rarityThresholds = [60, 85, 95, 99, 100]
        //  0～60 -> Common,
        //  60～85 -> Uncommon,
        //  85～95 -> Rare,
        //  95～99 -> Epic,
        //  99～100 -> Legendary
        float roll = Random.Range(0f, 100f);
        if (roll < _rarityThresholds[0]) return Rarity.Common;
        else if (roll < _rarityThresholds[1]) return Rarity.Uncommon;
        else if (roll < _rarityThresholds[2]) return Rarity.Rare;
        else if (roll < _rarityThresholds[3]) return Rarity.Epic;
        else return Rarity.Legendary;
    }

    /// <summary>
    /// ボーナス候補からランダムに1つボーナスを生成して返す
    /// </summary>
    private ItemBonus GetRandomBonusFromCandidates()
    {
        if (bonusCandidates == null || bonusCandidates.Count == 0)
        {
            // 候補リストがない場合はとりあえずデフォルトを返す
            return new ItemBonus(ParameterType.Strength, 1f);
        }

        // 候補の中からランダムに1つ選ぶ
        int index = Random.Range(0, bonusCandidates.Count);
        BonusCandidate candidate = bonusCandidates[index];

        float randomValue = Random.Range(candidate.minValue, candidate.maxValue);
        return new ItemBonus(candidate.parameterType, randomValue);
    }
}