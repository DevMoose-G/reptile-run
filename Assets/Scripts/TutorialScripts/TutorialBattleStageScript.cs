using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialBattleStageScript : BattleStageScript
{
    // Update is called once per frame
    void Update()
    {
        if (crown != null)
            crown.transform.Rotate(0, 50.0f * Time.deltaTime, 0, Space.World);

        if (level.GetComponent<LevelScript>().pauseGame)
            return;

        timeSinceLastAttack += Time.deltaTime;

        IStyle healthbarStyle = UI.GetComponent<UI>().opponentHealthBar.style;
        healthbarStyle.width = new StyleLength(Length.Percent((currentOpponent.health / currentOpponent.MAX_HEALTH) * 100));

        // for tutorial, first opponent doesn't hit you back
        if (defeatedOpponents < 1)
            timeSinceLastAttack = 0.0f;

        if (timeSinceLastAttack >= currentOpponent.attackSpeed && player.GetComponent<ReptileScript>().health > 0 && currentOpponent != null)
        {
            timeSinceLastAttack = 0.0f;
            DamagePlayer(currentOpponent.damage);
        }

        // check if opponent is dead
        if (currentOpponent != null && currentOpponent.health <= 0)
        {
            timeSinceLastAttack = 0.0f;

            // wait a few seconds
            if (timeAfterDefeat < DEFEAT_TIMER)
            {
                timeAfterDefeat += Time.deltaTime;
            }
            else
            {
                timeAfterDefeat = 0.0f;

                GameState.current.addEvoPoints(currentOpponent.evoPoints);

                Destroy(opponentsOrdering[0]);
                defeatedOpponents++;

                currentOpponent = null;

                opponentsOrdering.RemoveAt(0);
                if (opponentsOrdering.Count > 0)
                {
                    currentOpponent = opponentsOrdering[0].GetComponent<OpponentScript>();
                    UI.GetComponent<UI>().opponentName.text = currentOpponent.Name;

                    // pause while player is moving
                    IStyle outerStyle = UI.GetComponent<UI>().OuterCircle.style;
                    IStyle innerStyle = UI.GetComponent<UI>().InnerCircle.style;
                    outerStyle.display = DisplayStyle.None;
                    innerStyle.display = DisplayStyle.None;
                    UI.GetComponent<UI>().timerTillShown = 1.0f;

                    timeSinceLastAttack = -1.0f;
                }
            }
        }
    }
}
