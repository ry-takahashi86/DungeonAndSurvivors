using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class GoToLevelHub : ButtonActivated, MMEventListener<MMGameEvent>
{
    [MMInspectorGroup("Points of Entry", true, 0)]

    [Tooltip("移動先のシーン名")]
    public string DestinationScene;

    /// <summary>
    /// アクションのトリガー条件
    /// </summary>
    public override void TriggerButtonAction()
    {
        if (!CheckNumberOfUses())
        {
            return;
        }
        base.TriggerButtonAction ();

        GoToNextLevel();
    }

    /// <summary>
    /// 拠点(Hub)シーンに遷移する
    /// </summary>
    public virtual void GoToNextLevel()
    {
        if (DungeonManager.HasInstance)
        {
            Destroy(MMPersistentSingleton<DungeonManager>.Instance.gameObject);
        }

        LevelManager.Instance.GotoLevel(DestinationScene);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<MMGameEvent>();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<MMGameEvent>();
    }
    public void OnMMEvent(MMGameEvent gameEvent)
    {
        if (gameEvent.EventName == "BossDefeat")
        {
            MakeActivable();
        }
    }
}