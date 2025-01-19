using UnityEngine;

public class BattleParameterDisplay : MonoBehaviour
{
    public CharacterBattleParameter CharacterBattleParameter;
    public GameObject[] BattleParameterDisplays;

    /// <summary>
    /// 現在のBattleParameterに応じてUIを更新する
    /// </summary>
    public void UpdateDisplay()
    {
        if (CharacterBattleParameter != null)
        {
            BattleParameterDisplays[0].GetComponent<TMPro.TextMeshProUGUI>().text = CharacterBattleParameter.BattleParameter.MaxHP.ToString();
            BattleParameterDisplays[1].GetComponent<TMPro.TextMeshProUGUI>().text = CharacterBattleParameter.BattleParameter.AttackPower.ToString();
            BattleParameterDisplays[2].GetComponent<TMPro.TextMeshProUGUI>().text = CharacterBattleParameter.BattleParameter.DefensePower.ToString();
        }
    }
}