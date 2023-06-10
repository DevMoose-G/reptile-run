using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleStageScript : MonoBehaviour
{
    private GameObject UI;
    public GameObject level;
    public GameObject player;
    public GameObject crown;
    public List<GameObject> opponentsOrdering;
    private OpponentScript currentOpponent;
    private Animator currentOpponentAnimator;

    private float timeSinceLastAttack = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        UI = GameObject.Find("UIDocument");
        level = GameObject.Find("Level");
        player = GameObject.Find("Reptile");
        crown = GameObject.Find("Crown");

        UI.GetComponent<UI>().quickTime.style.display = DisplayStyle.Flex;

        VisualElement root = UI.GetComponent<UIDocument>().rootVisualElement;
        UI.GetComponent<UI>().opponentInfo = root.Q<GroupBox>("OpponentInfo");
        IStyle opponentInfoStyle = UI.GetComponent<UI>().opponentInfo.style;
        opponentInfoStyle.visibility = Visibility.Visible;
        UI.GetComponent<UI>().opponentName = root.Q<Label>("OpponentName");
        UI.GetComponent<UI>().opponentHealthBar = root.Q<VisualElement>("HealthBar");

        currentOpponent = opponentsOrdering[0].GetComponent<OpponentScript>();
        currentOpponentAnimator = currentOpponent.GetComponent<Animator>();

        UI.GetComponent<UI>().opponentName.text = currentOpponent.Name;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        crown.transform.Rotate(0, 50.0f * Time.deltaTime, 0, Space.World);

        IStyle healthbarStyle = UI.GetComponent<UI>().opponentHealthBar.style;
        healthbarStyle.width = new StyleLength(Length.Percent((currentOpponent.health / currentOpponent.MAX_HEALTH) * 100));

        if (timeSinceLastAttack >= currentOpponent.attackSpeed && player.GetComponent<ReptileScript>().health > 0) {
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
                UI.GetComponent<UI>().opponentName.text = currentOpponent.Name;
            }
        }
    }

    public void DamagePlayer(float amount)
    {
        currentOpponent.GetComponent<Animator>().SetTrigger("attack");
        player.GetComponent<ReptileScript>().health -= amount;
    }
}
