using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class CharacterBattleParameterPlayer : CharacterBattleParameter, MMEventListener<MMInventoryEvent>
{
    [Header("パラメータのUI表示")]
    public BattleParameterDisplay ParameterDisplay;

    // コンポーネント取得
    protected CharacterInventoryAllEquip _equipInventoryAllEquip;

    protected override void Start()
    {
        base.Start();

        // ==============================
        // イベント購読の設定
        // ==============================
        if (BattleParameter != null)
        {
            // パラメータが変わったときに呼ばれるメソッドを登録
            BattleParameter.OnParameterChanged += ApplyBattleParameter;
        }

        // BattleParameter Display を取得
        ParameterDisplay = FindFirstObjectByType<BattleParameterDisplay>();
        if (ParameterDisplay != null)
        {
            ParameterDisplay.CharacterBattleParameter = this;
        }

        // 必須コンポーネントを取得
        _equipInventoryAllEquip = GetComponent<CharacterInventoryAllEquip>();
    }

    /// <summary>
    /// イベントアイテムから対象インベントリを特定して格納されているアイテムを装備する
    /// </summary>
    /// <param name="battleParameter"></param>
    public void BattleParameterUpdateEquipments(InventoryItem eventItem)
    {
        // プレイヤーバトルパラメータのときは、装備を設定する
        if (BattleParameter is BattleParameterBasePlayer battleParameterPlayer)
        {
            if (_equipInventoryAllEquip != null)
            {
                switch (eventItem.TargetEquipmentInventoryName)
                {
                    case "WeaponInventory":
                        battleParameterPlayer.AttackWeapon = _equipInventoryAllEquip.WeaponInventory.Content[0];
                        break;
                    case "HeadInventory":
                        battleParameterPlayer.HeadWeapon = _equipInventoryAllEquip.HeadInventory.Content[0];
                        break;
                    case "BodyInventory":
                        battleParameterPlayer.BodyWeapon = _equipInventoryAllEquip.BodyInventory.Content[0];
                        break;
                    case "FootInventory":
                        battleParameterPlayer.FootWeapon = _equipInventoryAllEquip.FootInventory.Content[0];
                        break;
                    case "AccessoryInventory":
                        battleParameterPlayer.AccessoryWeapon = _equipInventoryAllEquip.AccessoryInventory.Content[0];
                        break;
                }
            }
        }
    }

    /// <summary>
    /// イベントアイテムから対象インベントリを特定して装備を外す
    /// </summary>
    /// <param name="battleParameter"></param>
    public void BattleParameterUpdateUnquipments(InventoryItem eventItem)
    {
        // プレイヤーバトルパラメータのときは、装備を解除する
        if (BattleParameter is BattleParameterBasePlayer battleParameterPlayer)
        {
            if (_equipInventoryAllEquip != null)
            {
                switch (eventItem.TargetEquipmentInventoryName)
                {
                    case "WeaponInventory":
                        battleParameterPlayer.AttackWeapon = null;
                        break;
                    case "HeadInventory":
                        battleParameterPlayer.HeadWeapon = null;
                        break;
                    case "BodyInventory":
                        battleParameterPlayer.BodyWeapon = null;
                        break;
                    case "FootInventory":
                        battleParameterPlayer.FootWeapon = null;
                        break;
                    case "AccessoryInventory":
                        battleParameterPlayer.AccessoryWeapon = null;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// MMInventoryEventをキャッチしてそれに応じて処理を行う
    /// </summary>
    /// <param name="inventoryEvent">Inventory event.</param>
    public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        switch (inventoryEvent.InventoryEventType)
        {
            case MMInventoryEventType.InventoryOpens:
                print("Inventory Opens");
                ParameterDisplay.UpdateDisplay();
                break;
            case MMInventoryEventType.ItemEquipped:
                print("Inventory ItemEquipped");
                BattleParameterUpdateEquipments(inventoryEvent.EventItem);
                ApplyBattleParameter();
                ParameterDisplay.UpdateDisplay();
                break;
            case MMInventoryEventType.ItemUnEquipped:
                print("Inventory ItemUnEquipped");
                BattleParameterUpdateUnquipments(inventoryEvent.EventItem);
                ApplyBattleParameter();
                ParameterDisplay.UpdateDisplay();
                break;
        }
    }

    /// <summary>
    /// Enableにすると、MMInventoryEventsのリッスンを開始する。
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<MMInventoryEvent>();
    }

    /// <summary>
    /// Disableの場合、MMInventoryEvents,BattleParameterのリッスンを停止する。
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<MMInventoryEvent>();

        if (BattleParameter != null)
        {
            BattleParameter.OnParameterChanged -= ApplyBattleParameter;
        }
    }
}