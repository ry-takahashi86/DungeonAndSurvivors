using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class DungeonManager : MMPersistentSingleton<DungeonManager>
{
    [Header("Dungeon Settings")]

    [Tooltip("現在の階層")]
    public int CurrentFloor = 1;

    [Tooltip("最大階層")]
    public int MaxFloor = 5;

    [Tooltip("通常フロア")]
    public string FloorLevelName;

    [Tooltip("ボスフロア")]
    public string BossLevelName;

    public void AdvanceFloor()
    {
        CurrentFloor++;
        if (CurrentFloor <= MaxFloor)
        {
            if (LevelManager.HasInstance)
            {
                LevelManager.Instance.GotoLevel(FloorLevelName);
            }
        }
        else
        {
            // 最大階層に達した場合は、ボスエリアへ遷移
            if (LevelManager.HasInstance)
            {
                LevelManager.Instance.GotoLevel(BossLevelName);
            }
        }
    }
}