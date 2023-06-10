using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialBattleStageScript : MonoBehaviour
{
    private GameObject UI;
    public GameObject level;
    public GameObject player;
    public GameObject crown;
    public List<GameObject> opponentsOrdering;
    private OpponentScript currentOpponent;

    private float timeSinceLastAttack = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        UI = GameObject.Find("UIDocument");
        level = GameObject.Find("Level");
        player = GameObject.Find("Reptile");
        crown = GameObject.Find("Crown");

        UI.GetComponent<UI>().quickTime.style.display = DisplayStyle.Flex;

        currentOpponent = opponentsOrdering[0].GetComponent<OpponentScript>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        crown.transform.Rotate(0, 50.0f * Time.deltaTime, 0, Space.World);

        UI.GetComponent<UI>().theirHealth.text = "Their: " + (System.Math.Truncate(currentOpponent.health * 100) / 100).ToString();

        if (timeSinceLastAttack >= currentOpponent.attackSpeed) {
            timeSinceLastAttack = 0.0f;
            DamagePlayer(currentOpponent.damage);
        }
    }

    public void DamageOpponent(float amount) {
        currentOpponent.health -= amount;

        if (currentOpponent.health <= 0) {
            timeSinceLastAttack = 0.0f;
            GameState.current.addEvoPoints(currentOpponent.evoPoints);
            Destroy(opponentsOrdering[0]);
            opponentsOrdering.RemoveAt(0);
            if (opponentsOrdering.Count > 0)
            {
                currentOpponent = opponentsOrdering[0].GetComponent<OpponentScript>();
            }  
        }
    }

    public void DamagePlayer(float amount)
    {
        player.GetComponent<ReptileScript>().health -= amount;
    }
}
