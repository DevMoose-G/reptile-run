using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class BattleStageScript : MonoBehaviour
{
    internal GameObject UI;
    public GameObject level;
    public GameObject player;
    public GameObject playerCam;
    public GameObject crown;
    public List<GameObject> opponentsOrdering;
    internal OpponentScript currentOpponent;
    private Animator currentOpponentAnimator;

    internal float timeSinceLastAttack = 0.0f;
    internal float DEFEAT_TIMER = 0.25f;
    internal float timeAfterDefeat = 0.0f;

    internal int defeatedOpponents = 0;

    // Start is called before the first frame update
    void Start()
    {
        UI = GameObject.Find("UIDocument");
        level = GameObject.Find("Level");
        player = GameObject.Find("Reptile");
        playerCam = GameObject.Find("Main Camera");
        crown = GameObject.Find("Crown");

        currentOpponent = opponentsOrdering[0].GetComponent<OpponentScript>();
        currentOpponentAnimator = currentOpponent.GetComponent<Animator>();

        UI.GetComponent<UI>().opponentName.text = currentOpponent.Name;
    }

    // Update is called once per frame
    void Update()
    {
        if (level.GetComponent<LevelScript>().pauseGame)
            return;

        timeSinceLastAttack += Time.deltaTime;

        if(crown != null)
            crown.transform.Rotate(0, 50.0f * Time.deltaTime, 0, Space.World);

        IStyle healthbarStyle = UI.GetComponent<UI>().opponentHealthBar.style;
        healthbarStyle.width = new StyleLength(Length.Percent((currentOpponent.health / currentOpponent.MAX_HEALTH) * 100));

        if (currentOpponent != null && timeSinceLastAttack >= currentOpponent.attackSpeed && player.GetComponent<ReptileScript>().health > 0) {
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

                Vector3 reptilePosScreen = playerCam.GetComponent<Camera>().WorldToScreenPoint(player.transform.position);
                if (reptilePosScreen.x > 1200)
                {
                    reptilePosScreen.x = 1200;
                }
                else if (reptilePosScreen.x < 15)
                {
                    reptilePosScreen.x = 15;
                }
                reptilePosScreen.y += 55;
                GameObject.Find("Indicators").transform.position = reptilePosScreen;

                player.GetComponent<ReptileScript>().evoText.GetComponent<TMP_Text>().color = new Color(1, 1, 1, 1);
                player.GetComponent<ReptileScript>().evoText.GetComponent<TMP_Text>().text = currentOpponent.evoPoints.ToString();
                player.GetComponent<ReptileScript>().evoImage.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);

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

    public void DamageOpponent(float amount) {
        currentOpponent.health -= amount;
        
    }

    public void DamagePlayer(float amount)
    {
        currentOpponent.GetComponent<Animator>().SetTrigger("attack");
        player.GetComponent<ReptileScript>().health -= amount;
    }
}
