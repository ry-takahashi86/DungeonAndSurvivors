using MoreMountains.Tools;
using UnityEngine;

public static class GenericMethods
{
    /// <summary>
    /// ダメージ計算メソッド
    /// </summary>
    /// <param name="minDamage"></param>
    /// <param name="maxDamage"></param>
    /// <param name="battleParameter"></param>
    /// <returns></returns>
    public static int CalculateDamage(float minDamage, float maxDamage, GameObject owner, GameObject target)
    {
        // 関係者のパラメータを取得する
        CharacterBattleParameter _owner = owner.MMGetComponentNoAlloc<CharacterBattleParameter>();
        CharacterBattleParameter _target = target.gameObject.MMGetComponentNoAlloc<CharacterBattleParameter>();

        // バトルパラメーターがない場合は、ベースダメージでランダムに返す
        if (_owner == null || _target == null)
        {
            return (int)UnityEngine.Random.Range(minDamage, Mathf.Max(maxDamage, minDamage));
        }

        // 攻撃倍率: (1 + AttackPower / 100)
        // 防御倍率: (1 - DefensePower / 100)
        float attackFactor  = 1f + (_owner.BattleParameter.AttackPower  / 100f);
        float defenseFactor = 1f - (_target.BattleParameter.DefensePower / 100f);

        // 基本ダメージ * 攻撃倍率 * 防御倍率
        float _minDamage = minDamage * attackFactor * defenseFactor;
        float _maxDamage = maxDamage * attackFactor * defenseFactor;

        Debug.Log($"最小ダメージ: {_minDamage} = 最小武器ダメージ({minDamage}) * ダメージ倍率({_owner.BattleParameter.AttackPower}%) * ダメージ軽減率({_target.BattleParameter.DefensePower}%)");
        Debug.Log($"最大ダメージ: {_maxDamage} = 最大武器ダメージ({maxDamage}) * ダメージ倍率({_owner.BattleParameter.AttackPower}%) * ダメージ軽減率({_target.BattleParameter.DefensePower}%)");

        return (int)UnityEngine.Random.Range(_minDamage, Mathf.Max(_minDamage, _maxDamage));
    }
}