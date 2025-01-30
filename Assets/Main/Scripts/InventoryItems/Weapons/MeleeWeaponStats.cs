using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class MeleeWeaponStats : MeleeWeapon
{
    protected override void CreateDamageArea()
    {
        // 作成済みののダメージエリアを使用する場合
        if ((MeleeDamageAreaMode == MeleeDamageAreaModes.Existing) && (ExistingDamageArea != null))
        {
            _damageArea = ExistingDamageArea.gameObject;
            _damageAreaCollider = _damageArea.gameObject.GetComponent<Collider>();
            _damageAreaCollider2D = _damageArea.gameObject.GetComponent<Collider2D>();
            _damageOnTouch = ExistingDamageArea;
            return;
        }

        // ダメージエリアオブジェクトの生成
        _damageArea = new GameObject();
        _damageArea.name = this.name + "DamageArea";
        _damageArea.transform.position = this.transform.position;
        _damageArea.transform.rotation = this.transform.rotation;
        _damageArea.transform.SetParent(this.transform);
        _damageArea.transform.localScale = Vector3.one;
        _damageArea.layer = this.gameObject.layer;

        if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
        {
            _boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
            _boxCollider2D.offset = AreaOffset;
            _boxCollider2D.size = AreaSize;
            _damageAreaCollider2D = _boxCollider2D;
            _damageAreaCollider2D.isTrigger = true;
        }
        if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
        {
            _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
            _circleCollider2D.transform.position = this.transform.position;
            _circleCollider2D.offset = AreaOffset;
            _circleCollider2D.radius = AreaSize.x / 2;
            _damageAreaCollider2D = _circleCollider2D;
            _damageAreaCollider2D.isTrigger = true;
        }

        if ((DamageAreaShape == MeleeDamageAreaShapes.Rectangle) || (DamageAreaShape == MeleeDamageAreaShapes.Circle))
        {
            Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            rigidBody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }

        if (DamageAreaShape == MeleeDamageAreaShapes.Box)
        {
            _boxCollider = _damageArea.AddComponent<BoxCollider>();
            _boxCollider.center = AreaOffset;
            _boxCollider.size = AreaSize;
            _damageAreaCollider = _boxCollider;
            _damageAreaCollider.isTrigger = true;
        }
        if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
        {
            _sphereCollider = _damageArea.AddComponent<SphereCollider>();
            _sphereCollider.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
            _sphereCollider.radius = AreaSize.x / 2;
            _damageAreaCollider = _sphereCollider;
            _damageAreaCollider.isTrigger = true;
        }

        if ((DamageAreaShape == MeleeDamageAreaShapes.Box) || (DamageAreaShape == MeleeDamageAreaShapes.Sphere))
        {
            Rigidbody rigidBody = _damageArea.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;

            rigidBody.gameObject.AddComponent<MMRagdollerIgnore>();
        }

        // DamageOnTouchStatsコンポーネントを付与してパラメータを渡す
        _damageOnTouch = _damageArea.AddComponent<DamageOnTouchStats>();
        _damageOnTouch.SetGizmoSize(AreaSize);
        _damageOnTouch.SetGizmoOffset(AreaOffset);
        _damageOnTouch.TargetLayerMask = TargetLayerMask;
        _damageOnTouch.MinDamageCaused = MinDamageCaused;
        _damageOnTouch.MaxDamageCaused = MaxDamageCaused;
        _damageOnTouch.DamageDirectionMode = DamageOnTouch.DamageDirections.BasedOnOwnerPosition;
        _damageOnTouch.DamageCausedKnockbackType = Knockback;
        _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
        _damageOnTouch.DamageCausedKnockbackDirection = KnockbackDirection;
        _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
        _damageOnTouch.HitDamageableFeedback = HitDamageableFeedback;
        _damageOnTouch.HitNonDamageableFeedback = HitNonDamageableFeedback;
        _damageOnTouch.TriggerFilter = TriggerFilter;

        if (!CanDamageOwner && (Owner != null))
        {
            _damageOnTouch.IgnoreGameObject(Owner.gameObject);
        }
    }


    /// <summary>
    /// Triggers an attack, turning the damage area on and then off
    /// </summary>
    /// <returns>The weapon attack.</returns>
    protected override IEnumerator MeleeWeaponAttack()
    {
        // 攻撃開始している時は、再実行されないよう中断する
        if (_attackInProgress) { yield break; }

        // 攻撃中フラグを立てて、初期遅延時間を待つ
        _attackInProgress = true;
        yield return new WaitForSeconds(InitialDelay);

        // 攻撃中に移動状態が変わった場合は、攻撃コルーチンを中止する
        if (Owner.MovementState.CurrentState == CharacterStates.MovementStates.Walking)
        {
            _attackInProgress = false;
            yield break;
        }

        EnableDamageArea();
        yield return new WaitForSeconds(ActiveDuration);
        DisableDamageArea();
        _attackInProgress = false;
    }
}