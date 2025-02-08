using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryClose : MonoBehaviour
{
    public InventoryDetailsModify MainInventoryDetailWindow;
    public InventoryInputManager InventoryInputManager;

    protected CanvasGroup _detailWindowAlpha;

    /// <summary>
    /// When that slot gets selected (via a mouse over or a touch), triggers an event for other classes to act on
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void CloseInventory()
    {
        CanvasGroup _detailWindowAlpha = MainInventoryDetailWindow.GetComponent<CanvasGroup>();
        if (_detailWindowAlpha == null) { return; }

        // アイテム詳細が表示されているときは、アイテム詳細をクローズして終了
        if (_detailWindowAlpha.alpha > 0f)
        {
            MainInventoryDetailWindow.DisplayDetails (null);
            return;
        }

        // インベントリ全体をクローズする
        if (InventoryInputManager != null)
        {
            InventoryInputManager.CloseInventory();
        }
    }
}