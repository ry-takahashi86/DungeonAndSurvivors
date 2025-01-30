using UnityEngine;

public static class GenericMethods
{
    /// <summary>
    /// ダメージ計算メソッド
    /// </summary>
    /// <param name="minDamage"></param>
    /// <param name="maxDamage"></param>
    /// <returns></returns>
    public static int CalculateDamage(float minDamage, float maxDamage, BattleParameterBase owner, BattleParameterBase target)
    {
        // クリティカルダメージ倍率
        const float CRITICAL_DAMAGE_MULTIPLIER = 1.5f;

        // 1) 攻撃倍率: (1 + (PhysicalDamageBonus / 100))
        float attackFactor  = 1f + (owner.PhysicalDamageBonus / 100f);

        // 2) 防御倍率: (1 - (PhysicalDamageReductionRate / 100))
        float defenseFactor = 1f - (target.PhysicalDamageReductionRate / 100f);

        // 3) 最小・最大ダメージに攻撃＆防御倍率を適用
        float _minDamage = minDamage * attackFactor * defenseFactor;
        float _maxDamage = maxDamage * attackFactor * defenseFactor;

        Debug.Log($"最小ダメージ: {_minDamage} = 最小武器ダメージ({minDamage}) * ダメージ倍率({owner.PhysicalDamageBonus}%) * ダメージ軽減率({target.PhysicalDamageReductionRate}%)");
        Debug.Log($"最大ダメージ: {_maxDamage} = 最大武器ダメージ({maxDamage}) * ダメージ倍率({owner.PhysicalDamageBonus}%) * ダメージ軽減率({target.PhysicalDamageReductionRate}%)");

        // 4) 実際のダメージ(乱数)を決定
        //    最小と最大が逆転しないよう Mathf.Maxで安全策（最小 > 最大になり得る場合を考慮）
        float baseDamage = Random.Range(_minDamage, Mathf.Max(_minDamage, _maxDamage));

        // 5) クリティカル判定
        float criticalRoll = Random.Range(0f, 100f);
        bool isCriticalHit = criticalRoll < owner.CriticalRate;

        if (isCriticalHit)
        {
            baseDamage *= CRITICAL_DAMAGE_MULTIPLIER;
            Debug.Log($"[Critical Hit!] クリティカル率:{owner.CriticalRate}%, 乱数:{criticalRoll}");
        }

        // 6) ダメージの最終値を整数化して返す
        int finalDamage = Mathf.FloorToInt(baseDamage);
        return finalDamage;
    }
}