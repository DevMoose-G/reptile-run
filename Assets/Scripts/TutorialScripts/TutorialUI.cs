using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TutorialUI : UI
{
    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.Find("Reptile");
        level = GameObject.Find("Level");

        // no ads

        GetUIDocuments();

        GetUIVariables();
        UpdateUpgrades();
        SetupBattleUI();

        progressScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    void Update()
    {
        if (level.GetComponent<TutorialLevelScript>().pauseGame == false || upgradeScreen.activeSelf)
        {
            base.Update();
        }
    }

        public void UpdateUpgrades()
    {
        IStyle upgrade1Style = upgrade1.style;
        IStyle upgrade2Style = upgrade2.style;
        IStyle upgrade3Style = upgrade3.style;
        IStyle upgrade4Style = upgrade4.style;

        UpgradeGroup nodeGroup = GameState.current.currentReptile().upgradeTree.GetUpgradeGroup();
        List<UpgradeNode> nodes = nodeGroup.toList();
        List<VisualElement> upgrades = new List<VisualElement> { upgrade1, upgrade2, upgrade3, upgrade4 };
        for (int i = 0; i < nodes.Count; i++)
        {
            IStyle upgradeStyle = upgrades[i].style;
            if (nodes[i] != null)
            {
                upgradeStyle.visibility = Visibility.Visible;
                upgrades[i].Q<Label>("Category").text = nodes[i].category + " " + toRomanNumerals(nodes[i].level);

                // removes previous callbacks
                upgrades[i].UnregisterCallback<ClickEvent, UpgradeNode>(BuyUpgrade, TrickleDown.TrickleDown);

                upgrades[i].Q<Label>("EvoAmount").text = nodes[i].cost.ToString();
                upgrades[i].RegisterCallback<ClickEvent, UpgradeNode>(BuyUpgrade, nodes[i]);
            }
            else
            {
                upgradeStyle.visibility = Visibility.Hidden;
            }
        }
    }
}
