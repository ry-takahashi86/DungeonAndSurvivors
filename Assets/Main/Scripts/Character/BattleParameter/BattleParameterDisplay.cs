using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BattleParameterDisplay : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    public BattleParameter BattleParameter;
    public GameObject[] BattleParameterDisplays;

    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        switch (inventoryEvent.InventoryEventType)
        {
            case MMInventoryEventType.InventoryOpens:
                if (BattleParameter == null)
                {
                    // プレイヤーのバトルパラメータを取得する
                    BattleParameter = FindFirstObjectByType<CharacterBattleParameterPlayer>().BattleParameter;
                    BattleParameter.OnParameterChanged += ApplyBattleParameterDisplay;
                }

                if (BattleParameter != null)
                {
                    ApplyBattleParameterDisplay();
                }
                else
                {
                    Debug.LogWarning($"CharacterBattleParameterPlayer が見つかりませんでした");
                }
                break;
        }
    }

    protected void OnEnable()
    {
        this.MMEventStartListening<MMInventoryEvent>();
    }

    protected void OnDestroy()
    {
        this.MMEventStopListening<MMInventoryEvent>();
        if (BattleParameter != null)
        {
            BattleParameter.OnParameterChanged -= ApplyBattleParameterDisplay;
        }
    }

    /// <summary>
    /// 現在のBattleParameterに応じてUIを更新する
    /// </summary>
    public void ApplyBattleParameterDisplay()
    {
        if (BattleParameter is BattleParameterPlayer battleParameterPlayer)
        {
            BattleParameterDisplays[0].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MaxHP.ToString();
            BattleParameterDisplays[1].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.WeaponDamageAvarege.ToString();
            BattleParameterDisplays[2].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.ArmorRating.ToString();
            BattleParameterDisplays[3].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.PhysicalDamageBonus.ToString() + "%";
            BattleParameterDisplays[4].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.PhysicalDamageReductionRate.ToString() + "%";
            BattleParameterDisplays[5].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.CriticalRate.ToString() + "%";
            BattleParameterDisplays[6].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.SkillCooldownRate.ToString() + "%";
            BattleParameterDisplays[7].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MoveSpeed.ToString();
            BattleParameterDisplays[8].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.AttackSpeed.ToString() + "%";
            BattleParameterDisplays[9].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MaxMP.ToString();
            BattleParameterDisplays[10].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MagicDamageBonus.ToString() + "%";
            BattleParameterDisplays[11].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MagicDamageReductionRate.ToString() + "%";
        }
    }
}