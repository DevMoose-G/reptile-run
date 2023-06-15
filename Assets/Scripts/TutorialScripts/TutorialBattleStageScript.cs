using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialBattleStageScript : BattleStageScript
{
    // Update is called once per frame
    void Update()
    {
        crown.transform.Rotate(0, 50.0f * Time.deltaTime, 0, Space.World);

        IStyle healthbarStyle = UI.GetComponent<UI>().opponentHealthBar.style;
        healthbarStyle.width = new StyleLength(Length.Percent((currentOpponent.health / currentOpponent.MAX_HEALTH) * 100));

        if (defeatedOpponents >= 1)
        {
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack >= currentOpponent.attackSpeed && player.GetComponent<ReptileScript>().health > 0)
            {
                timeSinceLastAttack = 0.0f;
                DamagePlayer(currentOpponent.damage);
            }
        }
    }
}
