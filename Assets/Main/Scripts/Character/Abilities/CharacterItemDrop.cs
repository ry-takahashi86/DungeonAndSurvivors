using UnityEngine;
using System.Collections;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class CharacterItemDrop : CharacterAbility
{
    [Header("Item Drop Settings")]
    [Tooltip("The item prefab to drop on death")]
    public GameObject ItemPrefab;

    [Tooltip("The number of items to drop on death")]
    public int DropCount = 1;

    [Tooltip("Random position offset for item drops")]
    public Vector3 RandomOffsetRange = new Vector3(0f, 0f, 0f);

    [Header("Drop Delay Settings")]
    [Tooltip("The delay (in seconds) before items are dropped")]
    public float DropDelay = 0f;

    [Tooltip("The chance (in percentage) to drop an item (0 to 100)")]
    [Range(0f, 100f)]
    public float DropChance = 100f;

    /// <summary>
    /// HealthクラスのOnDeathイベントにHandleDeathメソッドを登録する
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        if (_health != null)
        {
            _health.OnDeath += HandleDeath;
        }
    }

    /// <summary>
    /// HealthクラスのOnDeathイベントからHandleDeathメソッドを削除する(メモリリーク防止)
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        if (_health != null)
        {
            _health.OnDeath -= HandleDeath;
        }
    }

    /// <summary>
    /// Characterの死亡時にアイテムをドロップする
    /// </summary>
    protected virtual void HandleDeath()
    {
        if (DropDelay > 0f)
        {
            // Start coroutine to delay the item drop
            StartCoroutine(DropItemsWithDelay());
        }
        else
        {
            // Drop items immediately
            AttemptToDropItems();
        }
    }

    /// <summary>
    /// Drops items after a delay.
    /// </summary>
    protected virtual IEnumerator DropItemsWithDelay()
    {
        yield return new WaitForSeconds(DropDelay);
        AttemptToDropItems();
    }

    /// <summary>
    /// Attempts to drop items based on the drop chance.
    /// </summary>
    protected virtual void AttemptToDropItems()
    {
        float randomValue = Random.Range(0f, 100f);
        if (randomValue <= DropChance)
        {
            DropItems();
        }
        else
        {
            Debug.Log("No items dropped due to drop chance.");
        }
    }

    /// <summary>
    /// Drops items at the character's position.
    /// </summary>
    protected virtual void DropItems()
    {
        if (ItemPrefab == null || DropCount <= 0)
        {
            return;
        }

        for (int i = 0; i < DropCount; i++)
        {
            // Calculate random drop position
            Vector3 dropPosition = transform.position +
                                    new Vector3(
                                        Random.Range(-RandomOffsetRange.x, RandomOffsetRange.x),
                                        Random.Range(-RandomOffsetRange.y, RandomOffsetRange.y),
                                        Random.Range(-RandomOffsetRange.z, RandomOffsetRange.z));

            // Instantiate the item at the calculated position
            Instantiate(ItemPrefab, dropPosition, Quaternion.identity);
        }
    }
}