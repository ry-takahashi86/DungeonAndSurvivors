using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class DamageOnTouchStats : DamageOnTouch
{
    protected override void OnCollideWithDamageable(Health health)
    {
        _collidingHealth = health;

        // 衝突対象がダメージを受けることができるかどうかを判定
        if (health.CanTakeDamageThisFrame())
        {
            // 衝突対象のTopDownControllerを取得
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();
            if (_colliderTopDownController == null)
            {
                _colliderTopDownController = health.gameObject.GetComponentInParent<TopDownController>();
            }

            // 衝突時のフィードバック、イベントを実行
            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
            HitDamageableEvent?.Invoke(_colliderHealth);

            // Ownerが設定されていない場合は、自分自身をOwnerとして設定する
            if (Owner == null)
            {
                Owner = gameObject;
            }

            // ダメージを決定
            float randomDamage = Random.Range(MinDamageCaused, MaxDamageCaused);
            Debug.Log($"攻撃側: {Owner.name}, 防御側: {health.gameObject.name}");
            CharacterBattleParameter _owner = Owner.MMGetComponentNoAlloc<CharacterBattleParameter>();
            CharacterBattleParameter _target = health.gameObject.MMGetComponentNoAlloc<CharacterBattleParameter>();
            randomDamage = GenericMethods.CalculateDamage(MinDamageCaused, MaxDamageCaused, _owner.BattleParameter, _target.BattleParameter);

            // ノックバックを適用
            ApplyKnockback(randomDamage, TypedDamages);

            // ダメージの方向を各コンポーネントに渡す
            DetermineDamageDirection();

            // 継続ダメージの場合
            if (RepeatDamageOverTime)
            {
                _colliderHealth.DamageOverTime(randomDamage, gameObject, InvincibilityDuration,
                    InvincibilityDuration, _damageDirection, TypedDamages, AmountOfRepeats, DurationBetweenRepeats,
                    DamageOverTimeInterruptible, RepeatedDamageType);
            }
            // 単発ダメージの場合
            else
            {
                _colliderHealth.Damage(randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration,
                    _damageDirection, TypedDamages);
            }
        }

        // 自傷ダメージを受ける場合の処理
        if (DamageTakenEveryTime + DamageTakenDamageable > 0 && !_colliderHealth.PreventTakeSelfDamage)
        {
            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
        }
    }
}