using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class EnemySpawner : TimedSpawner
{
    [Header("Random Spawn List")]
    public List<GameObject> SpawnEnemyList;

    [Header("Random Quantity Offset")]
    public int MinQuantity = 0;
    public int MaxQuantity = 1;

    [Header("Random Spawn Offset")]
    public Vector2 SpawnOffsetRange = Vector2.zero;

    private int _spawnCount = 0;

    protected override void Start()
    {
        base.Start();

        InitialSpawn();
    }

    public void InitialSpawn()
    {
        if (ObjectPooler is MMMultipleObjectPooler multipleObjectPooler)
        {
            Debug.LogWarning("未実装");
            return;
        }
        else if (ObjectPooler is MMSimpleObjectPooler simpleObjectPooler)
        {
            // 既存オブジェクト・プーラーを削除
            simpleObjectPooler.DestroyObjectPool();

            // オブジェクト・プーラーの設定
            simpleObjectPooler.GameObjectToPool = SpawnEnemyList[UnityEngine.Random.Range(0, SpawnEnemyList.Count)];
            simpleObjectPooler.PoolSize = UnityEngine.Random.Range(MinQuantity, MaxQuantity);

            // プーラーを再作成する
            simpleObjectPooler.FillObjectPool();

            _spawnCount = simpleObjectPooler.PoolSize;
        }
        else
        {
            return;
        }

        for (int i=0; i < _spawnCount; i++)
        {
            GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

            // エラーハンドリング
            if (nextGameObject==null) { return; }
            if (nextGameObject.GetComponent<MMPoolableObject>()==null)
            {
                throw new Exception(gameObject.name+" is trying to spawn objects that don't have a PoolableObject component.");
            }

            // スポーンオブジェクトをアクティブにする
            nextGameObject.gameObject.SetActive(true);
            nextGameObject.gameObject.MMGetComponentNoAlloc<MMPoolableObject>().TriggerOnSpawnComplete();

            // Healthコンポーネントを持つ場合は、復活させる
            Health objectHealth = nextGameObject.gameObject.MMGetComponentNoAlloc<Health> ();
            if (objectHealth != null)
            {
                objectHealth.Revive ();
            }

            // スポーンオブジェクトの座標設定
            Vector3 spawnPosition = this.transform.position;
            spawnPosition += new Vector3(
                UnityEngine.Random.Range(-SpawnOffsetRange.x, SpawnOffsetRange.x),
                UnityEngine.Random.Range(-SpawnOffsetRange.y, SpawnOffsetRange.y),
                0f);
            nextGameObject.transform.position = spawnPosition;
        }
    }
}