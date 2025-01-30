using MoreMountains.TopDownEngine;
using UnityEngine;

public class BattleParameterDisplay : MonoBehaviour
{
    public CharacterBattleParameterPlayer CharacterBattleParameter;
    public GameObject[] BattleParameterDisplays;

    /// <summary>
    /// 現在のBattleParameterに応じてUIを更新する
    /// </summary>
    public void UpdateDisplay()
    {
        if (CharacterBattleParameter != null)
        {
            if (CharacterBattleParameter.BattleParameter is BattleParameterBasePlayer battleParameterPlayer)
            {
                BattleParameterDisplays[0].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MaxHP.ToString();
                BattleParameterDisplays[1].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.WeaponDamageAvarege.ToString();
                BattleParameterDisplays[2].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.ArmorRating.ToString();
                BattleParameterDisplays[3].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.PhysicalDamageBonus.ToString();
                BattleParameterDisplays[4].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.PhysicalDamageReductionRate.ToString();
                BattleParameterDisplays[5].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.CriticalRate.ToString();
                BattleParameterDisplays[6].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.SkillCooldownRate.ToString();
                BattleParameterDisplays[7].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MoveSpeedBonus.ToString();
                BattleParameterDisplays[8].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.AttackSpeed.ToString();
                BattleParameterDisplays[9].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MaxMP.ToString();
                BattleParameterDisplays[10].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MagicDamageBonus.ToString();
                BattleParameterDisplays[11].GetComponent<TMPro.TextMeshProUGUI>().text = battleParameterPlayer.MagicDamageReductionRate.ToString();
            }
        }
    }
}