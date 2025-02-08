using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class ProjectileWeaponStats : ProjectileWeapon
{
    [Header("DamageCaused")] 
    [Tooltip("最小ダメージ")]
    public float MinDamageCaused = 10f;
    [Tooltip("最大ダメージ")]
    public float MaxDamageCaused = 10f;

    [MMInspectorGroup("Use", true, 10)]
    [Header("")]
    [MMReadOnly]
    public float InitialAttackDelay = 0f;   // 攻撃開始までの遅延時間
    [MMReadOnly]
    public float InitialAttackSpeed = 0f;   // 攻撃間隔の時間

    public override void Initialization()
    {
        base.Initialization();

        // 初期攻撃速度を保存する
        InitialAttackDelay = DelayBeforeUse;
        InitialAttackSpeed = TimeBetweenUses;
    }

    public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
    {
        /// 発射するオブジェクトを取得する
        GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

        // 発射オブジェクトの取得確認
        if (nextGameObject == null) { return null; }
        if (nextGameObject.GetComponent<MMPoolableObject>() == null)
        {
            throw new System.Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
        }

        // 発射オブジェクトの座標を設定する
        nextGameObject.transform.position = spawnPosition;
        if (_projectileSpawnTransform != null)
        {
            nextGameObject.transform.position = _projectileSpawnTransform.position;
        }

        // 発射オブジェクトのダメージ判定にオーナーと武器ダメージ量を渡す
        DamageOnTouchStats damageOnTouchStats = nextGameObject.GetComponent<DamageOnTouchStats>();
        if (damageOnTouchStats != null)
        {
            damageOnTouchStats.Owner = Owner.gameObject;
            damageOnTouchStats.MinDamageCaused = MinDamageCaused;
            damageOnTouchStats.MaxDamageCaused = MaxDamageCaused;
        }

        // 発射オブジェクトにオーナーを設定する
        Projectile projectile = nextGameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetWeapon(this);
            if (Owner != null)
            {
                projectile.SetOwner(Owner.gameObject);
            }
        }

        // 発射オブジェクトを有効化する
        nextGameObject.gameObject.SetActive(true);

        // 発射オブジェクトのパラメータを設定する
        if (projectile != null)
        {
            if (RandomSpread)
            {
                _randomSpreadDirection.x = UnityEngine.Random.Range(-Spread.x, Spread.x);
                _randomSpreadDirection.y = UnityEngine.Random.Range(-Spread.y, Spread.y);
                _randomSpreadDirection.z = UnityEngine.Random.Range(-Spread.z, Spread.z);
            }
            else
            {
                if (totalProjectiles > 1)
                {
                    _randomSpreadDirection.x = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.x, Spread.x);
                    _randomSpreadDirection.y = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.y, Spread.y);
                    _randomSpreadDirection.z = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.z, Spread.z);
                }
                else
                {
                    _randomSpreadDirection = Vector3.zero;
                }
            }

            Quaternion spread = Quaternion.Euler(_randomSpreadDirection);

            if (Owner == null)
            {
                projectile.SetDirection(spread * transform.rotation * DefaultProjectileDirection, transform.rotation, true);
            }
            else
            {
                if (Owner.CharacterDimension == Character.CharacterDimensions.Type3D) // if we're in 3D
                {
                    projectile.SetDirection(spread * transform.forward, transform.rotation, true);
                }
                else // if we're in 2D
                {
                    Vector3 newDirection = (spread * transform.right) * (Flipped ? -1 : 1);
                    if (Owner.Orientation2D != null)
                    {
                        projectile.SetDirection(newDirection, spread * transform.rotation, Owner.Orientation2D.IsFacingRight);
                    }
                    else
                    {
                        projectile.SetDirection(newDirection, spread * transform.rotation, true);
                    }
                }
            }

            if (RotateWeaponOnSpread)
            {
                this.transform.rotation = this.transform.rotation * spread;
            }
        }

        if (triggerObjectActivation)
        {
            if (nextGameObject.GetComponent<MMPoolableObject>() != null)
            {
                nextGameObject.GetComponent<MMPoolableObject>().TriggerOnSpawnComplete();
            }
        }
        return (nextGameObject);
    }
}