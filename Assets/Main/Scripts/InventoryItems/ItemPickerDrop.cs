using MoreMountains.InventoryEngine;
using UnityEngine;

public class ItemPickerDrop : ItemPicker
{
    [Header("Item Database")]
    public InventoryItemDataBase inventoryItemDataBase;

    protected override void Initialization()
    {
        if (Item == null)
        {
            RandomizeItem();
        }

        base.Initialization();
    }

    public void RandomizeItem()
    {
        int randomIndex = Random.Range(0, inventoryItemDataBase.InventoryItems.Count);
        Item = inventoryItemDataBase.InventoryItems[randomIndex];

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Item.Icon;
    }
}