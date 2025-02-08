using System.Drawing;
using System.Security.AccessControl;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryDetailsModify : InventoryDetails
{
    public GameObject InventoryButtons;

    protected override void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (HideOnStart)
        {
            _canvasGroup.alpha = 0;
        }

        if (_canvasGroup.alpha == 0)
        {
            Hidden = true;
            _canvasGroup.blocksRaycasts = false;
        }
        else
        {
            Hidden = false;
        }
   }

    public override void DisplayDetails(InventoryItem item)
    {
        // アイテムが未選択のときは、表示しない
        if (InventoryItem.IsNull(item))
        {
            if (HideOnEmptySlot && !Hidden)
            {
                _canvasGroup.blocksRaycasts = false;
                StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup,_fadeDelay,0f));
                Hidden=true;
            }
            if (!HideOnEmptySlot)
            {
                StartCoroutine(FillDetailFieldsWithDefaults(0));
            }
        }
        else
        {
            StartCoroutine(FillDetailFields(item,0f));

            if (HideOnEmptySlot && Hidden)
            {
                _canvasGroup.blocksRaycasts = true;
                StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup,_fadeDelay,1f));
                Hidden=false;
            }
        }
    }

    public override void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        // if this event doesn't concern our inventory display, we do nothing and exit
        // if (!Global && (inventoryEvent.TargetInventoryName != this.TargetInventoryName))
        // {
        //     return;
        // }

        if (inventoryEvent.PlayerID != PlayerID)
        {
            return;
        }

        switch (inventoryEvent.InventoryEventType)
        {
            case MMInventoryEventType.Select:
                DisplayDetails (inventoryEvent.EventItem);
                break;
            case MMInventoryEventType.UseRequest:
                DisplayDetails (null);
                break;
            case MMInventoryEventType.InventoryOpens:
                DisplayDetails (null);
                break;
            case MMInventoryEventType.InventoryCloses:
                DisplayDetails (null);
                break;
            case MMInventoryEventType.Drop:
                DisplayDetails (null);
                break;
            case MMInventoryEventType.EquipRequest:
                DisplayDetails (null);
                break;
            case MMInventoryEventType.UnEquipRequest:
                DisplayDetails (null);
                break;
        }
    }
}