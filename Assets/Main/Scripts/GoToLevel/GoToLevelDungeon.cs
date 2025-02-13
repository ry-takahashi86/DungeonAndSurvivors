using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class GoToLevelDungeon : ButtonActivated
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
    /// 次のフロアに移動する
    /// </summary>
    public virtual void GoToNextLevel()
    {
        LevelManager.Instance.GotoLevel(DestinationScene);
    }
}