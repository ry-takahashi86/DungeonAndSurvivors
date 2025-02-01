using UnityEngine;

public class BattleParameterDisplay : MonoBehaviour
{
    public BattleParameterBasePlayer BattleParameter;
    public GameObject[] BattleParameterDisplays;

    /// <summary>
    /// 現在のBattleParameterに応じてUIを更新する
    /// </summary>
    public void ApplyBattleParameterDisplay()
    {
        BattleParameterDisplays[0].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.MaxHP.ToString();
        BattleParameterDisplays[1].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.WeaponDamageAvarege.ToString();
        BattleParameterDisplays[2].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.ArmorRating.ToString();
        BattleParameterDisplays[3].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.PhysicalDamageBonus.ToString();
        BattleParameterDisplays[4].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.PhysicalDamageReductionRate.ToString();
        BattleParameterDisplays[5].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.CriticalRate.ToString();
        BattleParameterDisplays[6].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.SkillCooldownRate.ToString();
        BattleParameterDisplays[7].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.MoveSpeedBonus.ToString();
        BattleParameterDisplays[8].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.AttackSpeed.ToString();
        BattleParameterDisplays[9].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.MaxMP.ToString();
        BattleParameterDisplays[10].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.MagicDamageBonus.ToString();
        BattleParameterDisplays[11].GetComponent<TMPro.TextMeshProUGUI>().text = BattleParameter.MagicDamageReductionRate.ToString();
    }
}