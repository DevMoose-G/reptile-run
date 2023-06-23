using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    private Label EVPoints;
    public VisualElement OuterCircle;
    public VisualElement InnerCircle;
    public GroupBox quickTime;
    public float timerTillShown = GameState.current.reptiles[GameState.current.current_reptile_idx].attackSpeed;

    public Label yourHealth;
    public Label theirHealth;

    public GameObject player;
    public GameObject level;
    public GameObject upgradeScreen;
    public Button playGame;
    public Button storeButton;
    public Button upgrade1;
    public Button upgrade2;
    public Button upgrade3;
    public Button upgrade4;

    public Label opponentName;
    public GroupBox opponentInfo;
    public VisualElement opponentHealthBar;

    public GameObject progressScreen;

    public GameObject winScreen;
    public Button winScreenContinue;

    private float MAX_SIZE = 128;
    private float MIN_SIZE = 42;
    private float circleSpeed;

    private GameObject adsManager;
    private int MAX_NUM_OF_AD_UPGRADES = 1;
    public int adUpgradeCounter = 0;

    public List<bool> UIActiveStates = new List<bool>{true, true, true, true};

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.Find("Reptile");
        level = GameObject.Find("Level");

        adsManager = GameObject.Find("Ads Manager");
        adsManager.GetComponent<AdsInitializer>().LoadRewardedAd();

        GetUIDocuments();

        GetUIVariables();
        UpdateUpgrades();
        SetupBattleUI();

        progressScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    public void SetupBattleUI()
    {
        IStyle style = OuterCircle.style;
        style.width = MAX_SIZE;
        style.height = MAX_SIZE;

        IStyle innerStyle = InnerCircle.style;
        innerStyle.width = MIN_SIZE;
        innerStyle.height = MIN_SIZE;

        IStyle quickTimeStyle = quickTime.style;
        quickTimeStyle.display = DisplayStyle.None;

        IStyle opponentInfoStyle = opponentInfo.style;
        opponentInfoStyle.visibility = Visibility.Hidden;
    }

    public void GetUIVariables()
    {
        if (upgradeScreen == null || winScreen == null || progressScreen == null)
            GetUIDocuments();
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        EVPoints = root.Q<Label>("EVPoints");
        quickTime = root.Q<GroupBox>("QuickTime");
        OuterCircle = root.Q<VisualElement>("OuterCircle");
        InnerCircle = root.Q<VisualElement>("InnerCircle");
        yourHealth = root.Q<Label>("Health");

        opponentName = root.Q<Label>("OpponentName");
        opponentInfo = root.Q<GroupBox>("OpponentInfo");
        opponentHealthBar = root.Q<VisualElement>("HealthBar");

        VisualElement upgradeRoot = upgradeScreen.GetComponent<UIDocument>().rootVisualElement;

        playGame = upgradeRoot.Q<Button>("PlayGame");
        playGame.RegisterCallback<ClickEvent>(StartGame);

        storeButton = upgradeRoot.Q<Button>("Store");
        storeButton.RegisterCallback<ClickEvent>(StoreScene);

        upgrade1 = upgradeRoot.Q<Button>("Upgrade1");
        upgrade2 = upgradeRoot.Q<Button>("Upgrade2");
        upgrade3 = upgradeRoot.Q<Button>("Upgrade3");
        upgrade4 = upgradeRoot.Q<Button>("Upgrade4");

        // anything you do hear has to be redone when you renable winScreen
        if (winScreen != null && winScreen.activeSelf)
        {
            VisualElement winRoot = winScreen.GetComponent<UIDocument>().rootVisualElement;
            winScreenContinue = winRoot.Q<Button>("Continue");
            if(level != null)
                winScreenContinue.RegisterCallback<ClickEvent>(level.GetComponent<LevelScript>().EndGame);
        }
    }

    public void GetUIDocuments()
    {
        if(upgradeScreen == null)
            upgradeScreen = GameObject.Find("UpgradeScreen");
        if (winScreen == null)
            winScreen = GameObject.Find("WinScreen");
        if (progressScreen == null)
            progressScreen = GameObject.Find("ProgressScreen");
    }

    private void OnEnable() {
        UpgradeTree.Load();
    }


    public void UISetActive(bool on)
    {
        if (on == false)
        {
            // saves UI states
            UIActiveStates[0] = gameObject.GetComponent<UIDocument>().enabled;
            UIActiveStates[1] = upgradeScreen.activeSelf;
            UIActiveStates[2] = progressScreen.activeSelf;
            UIActiveStates[3] = winScreen.activeSelf;
            // turns all UI stuff off
            winScreen.SetActive(on);
            gameObject.GetComponent<UIDocument>().enabled = on;
            progressScreen.SetActive(on);
            upgradeScreen.SetActive(on);
        } else
        {
            GetUIDocuments();

            gameObject.GetComponent<UIDocument>().enabled = UIActiveStates[0];
            upgradeScreen.SetActive(UIActiveStates[1]);
            progressScreen.SetActive(UIActiveStates[2]);
            winScreen.SetActive(UIActiveStates[3]);

            GetUIVariables();
            IStyle quickTimeStyle = quickTime.style;
            quickTimeStyle.display = DisplayStyle.None;

            IStyle opponentInfoStyle = opponentInfo.style;
            opponentInfoStyle.visibility = Visibility.Hidden;

            UpdateUpgrades();
        }
    }

    private void StoreScene(ClickEvent evt)
    {
        PlayerPrefs.SetInt("NumberOfRuns", 0);
        SceneManager.LoadSceneAsync(1);
    }

    public static string toRomanNumerals(int num)
    {
        string ret = "";
        while (num > 0)
        {
            if (num >= 1000)
            {
                ret += "M";
                num -= 1000;
            }
            else if (num >= 900)
            {
                ret += "CM";
                num -= 900;
            }
            else if (num >= 500)
            {
                ret += "D";
                num -= 500;
            }
            else if (num >= 400)
            {
                ret += "CD";
                num -= 400;
            }
            else if (num >= 100)
            {
                ret += "C";
                num -= 100;
            }
            else if (num >= 50)
            {
                ret += "L";
                num -= 50;
            }
            else if (num >= 40)
            {
                ret += "XL";
                num -= 40;
            }
            else if (num >= 10)
            {
                ret += "X";
                num -= 10;
            }
            else if (num >= 9)
            {
                ret += "IX";
                num -= 9;
            }
            else if (num >= 5)
            {
                ret += "V";
                num -= 5;
            }
            else if (num >= 4)
            {
                ret += "IV";
                num -= 4;
            }
            else if (num >= 1)
            {
                ret += "I";
                num -= 1;
            }
        }
        return ret;
    }

    public float ratioInnerOuterCircle() {
        if(quickTime.style.display == DisplayStyle.None)
        {
            return -1.0f;
        }
        IStyle outerStyle = OuterCircle.style;
        IStyle innerStyle = InnerCircle.style;
        return innerStyle.width.value.value / outerStyle.width.value.value;
    }

    public void circleHit() {
        // resets the outer circle
        IStyle style = OuterCircle.style;
        style.width = MAX_SIZE;
        style.height = MAX_SIZE;

        IStyle outerStyle = OuterCircle.style;
        IStyle innerStyle = InnerCircle.style;
        outerStyle.display = DisplayStyle.None;
        innerStyle.display = DisplayStyle.None;
    }

    public void UpdateUpgrades() {
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
                upgrades[i].UnregisterCallback<ClickEvent, UpgradeNode>(AdUpgrade, TrickleDown.TrickleDown);

                if (GameState.current.currentReptile().evoPoints >= nodes[i].cost || adUpgradeCounter >= MAX_NUM_OF_AD_UPGRADES || adsManager == null) // if you have enough evo points, show the cost
                {
                    upgrades[i].Q<Label>("EvoAmount").text = nodes[i].cost.ToString();
                    upgrades[i].RegisterCallback<ClickEvent, UpgradeNode>(BuyUpgrade, nodes[i]);
                }
                else { // else write, show add
                    upgrades[i].Q<Label>("EvoAmount").text = "Watch Ad";
                    upgrades[i].RegisterCallback<ClickEvent, UpgradeNode>(AdUpgrade, nodes[i]);
                }
            }
            else
            {
                upgradeStyle.visibility = Visibility.Visible;

                upgrades[i].Q<Label>("Category").text = nodes[i].category + " " + toRomanNumerals(nodes[i].level);

                // removes previous callbacks
                upgrades[i].UnregisterCallback<ClickEvent, UpgradeNode>(BuyUpgrade, TrickleDown.TrickleDown);
                upgrades[i].UnregisterCallback<ClickEvent, UpgradeNode>(AdUpgrade, TrickleDown.TrickleDown);

                upgrades[i].Q<Label>("EvoAmount").text = "MAX";
            }
        }
    }

    private void AdUpgrade(ClickEvent evt, UpgradeNode data)
    {
        if(adUpgradeCounter >= MAX_NUM_OF_AD_UPGRADES) {
            return; // stop watching ads after you hit max ads
        }
        adsManager.GetComponent<AdsInitializer>().data_for_BoughtUpgrade = data;
        adsManager.GetComponent<AdsInitializer>().ShowRewardedAd();
        adsManager.GetComponent<AdsInitializer>().LoadRewardedAd();
    }

    public void BuyUpgrade(ClickEvent evt, UpgradeNode node)
    {
        print("Buying UPGRADE");
        if (GameState.current.currentReptile().evoPoints >= node.cost)
        {
            GameState.current.subtractEvoPoints(node.cost);
            ApplyUpgrade(evt, node);
        }
    }

    public void ApplyUpgrade(ClickEvent evt, UpgradeNode node)
    {
        print("BOUGHT UPGRADE");
        
        GameState.current.currentReptile().upgradeTree.nodes_obtained.Add(node.id);

        // apply the upgrade
        print(node.category);
        if (node.category == "Tongue")
        {
            GameState.current.currentReptile().tonguePeakLength += node.amount;
            GameState.current.currentReptile().tongueRetractionSpeed *= node.amount2;
            GameState.current.currentReptile().tongueSpeed *= node.amount2;
        }
        else if (node.category == "Health") { 
            GameState.current.currentReptile().MAX_HEALTH += node.amount;
            player.GetComponent<ReptileScript>().health = GameState.current.currentReptile().MAX_HEALTH;
        }
        else if (node.category == "AttackSpeed")
        {
            GameState.current.currentReptile().attackSpeed /= node.amount;
        }
        else if (node.category == "Damage")
        {
            print("Got here");
            GameState.current.currentReptile().damage += node.amount;
            print(node.amount);
            print(GameState.current.currentReptile().damage);
        }

        UpdateUpgrades();
    }

    public void StartGame(ClickEvent evt)
    {
        GameObject.Find("Level").GetComponent<LevelScript>().isMoving = true;
        upgradeScreen.SetActive(false);
        player.GetComponent<ReptileScript>().animator.SetBool("isMoving", true);
        player.GetComponent<ReptileScript>().canMove = true;
    }

    // Update is called once per frame
    internal void Update()
    {
        EVPoints.text = GameState.current.currentReptile().evoPoints.ToString();
        if (player.GetComponent<ReptileScript>().health < 0)
        {
            yourHealth.text = "0";
        }
        else
        {
            yourHealth.text = (System.Math.Truncate(player.GetComponent<ReptileScript>().health * 100) / 100).ToString();
        }

        if (GameObject.Find("Level").GetComponent<LevelScript>().pauseGame)
            return;

        // making outer circle smaller but still centered
        IStyle quickTimeStyle = quickTime.style;
        IStyle outerStyle = OuterCircle.style;
        IStyle innerStyle = InnerCircle.style;

        if (quickTimeStyle.display != DisplayStyle.None && progressScreen != null && !progressScreen.activeSelf) {
            

            if (outerStyle.width.value.value <= MIN_SIZE) // resets circle once it hits the inner circle
            {
                outerStyle.width = MAX_SIZE;
                outerStyle.height = MAX_SIZE;
                timerTillShown = GameState.current.currentReptile().attackSpeed;
            }

            if (outerStyle.display == DisplayStyle.None) // if outer circle is invisible, wait for timerTillShown to hit 0, then show both circles
            {
                timerTillShown -= Time.deltaTime;
                if (timerTillShown <= 0) {
                    timerTillShown = GameState.current.currentReptile().attackSpeed;
                    outerStyle.display = DisplayStyle.Flex;
                    innerStyle.display = DisplayStyle.Flex;
                }
            }
            else
            {
                circleSpeed = (MAX_SIZE - MIN_SIZE) / GameState.current.currentReptile().attackSpeed;

                timerTillShown -= Time.deltaTime;

                outerStyle.unityBackgroundImageTintColor = new StyleColor(new Color(1 - ratioInnerOuterCircle(), ratioInnerOuterCircle(), 0, 1));
                innerStyle.unityBackgroundImageTintColor = new StyleColor(new Color(1 - ratioInnerOuterCircle(), ratioInnerOuterCircle(), 0, 1));

                outerStyle.width = OuterCircle.style.width.value.value - (Time.deltaTime * circleSpeed);
                outerStyle.height = OuterCircle.style.height.value.value - (Time.deltaTime * circleSpeed);

                outerStyle.top = 32 - (outerStyle.height.value.value / 2) - (innerStyle.height.value.value / 2);
                outerStyle.left = (innerStyle.height.value.value / 2) - (outerStyle.width.value.value / 2) + 32;
            }
        } 

    }
}
