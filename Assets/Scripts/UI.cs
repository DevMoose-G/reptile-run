using System;
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

    public VisualTreeAsset battleUI;

    public Label yourHealth;
    public VisualElement extraHealth;

    public Slider volumeSlider;

    public GameObject player;
    public GameObject playerCam;
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

    public List<bool> UIActiveStates = new List<bool> { true, true, true, true };

    // Start is called before the first frame update
    void Start()
    {

        playerCam = GameObject.Find("Main Camera");
        player = GameObject.Find("Reptile");
        level = GameObject.Find("Level");

        adsManager = GameObject.Find("Ads Manager");
        if (AdsManager.rewardedAd == null || AdsManager.rewardedAd.CanShowAd() == false)
        {
            adsManager.GetComponent<AdsInitializer>().LoadRewardedAd();
        }

        GetUIDocuments();

        GetUIVariables();
        try
        {
            UpdateUpgrades();
        }
        catch (Exception e)
        {
            Debug.LogError("UPDATE UPGRADES STILL DOES NOT WORK FROM START FUNCTION. MAYBE B/C of INTRO SEQUENCE?");
        }

        progressScreen.SetActive(false);
        winScreen.SetActive(false);

        volumeSlider.value = PlayerPrefs.GetFloat("volume", volumeSlider.value);
    }

    public void SwitchToBattleUI()
    {
        GetComponent<UIDocument>().visualTreeAsset = battleUI;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        opponentInfo = root.Q<GroupBox>("OpponentInfo");
        IStyle opponentInfoStyle = opponentInfo.style;
        opponentInfoStyle.visibility = Visibility.Visible;

        opponentName = opponentInfo.Q<Label>("OpponentName");
        opponentHealthBar = opponentInfo.Q<VisualElement>("OpponentHealthBar");

        root.Q<Label>("YourName").text = GameState.current.currentReptile().name;
        root.Q<Label>("YourSpecies").text = "the " + GameState.current.currentReptile().species;

        for (int i = 1; i <= 4; i++)
        {
            if (i <= GameState.current.currentReptile().moves.Count)
            {
                Button currentButton = root.Q<Button>("Move" + i.ToString());
                MoveData currentMove = GameState.current.currentReptile().moves[i - 1];
                currentButton.Q<Label>("Name").text = currentMove.name;
                currentButton.RegisterCallback<ClickEvent, MoveData>(PressedMove, currentMove);
            }
            else
            {
                // hide the move
                IStyle moveStyle = root.Q<Button>("Move" + i.ToString()).style;
                moveStyle.visibility = Visibility.Hidden;
            }
        }
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
        extraHealth = root.Q<VisualElement>("ExtraHealth");

        VisualElement upgradeRoot = upgradeScreen.GetComponent<UIDocument>().rootVisualElement;

        volumeSlider = upgradeRoot.Q<Slider>("VolumeSlider");

        playGame = upgradeRoot.Q<Button>("PlayGame");
        playGame.UnregisterCallback<ClickEvent>(StartGame);
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
            if (level != null)
                winScreenContinue.RegisterCallback<ClickEvent>(level.GetComponent<LevelScript>().EndGame);
        }
    }

    public void GetUIDocuments()
    {
        if (upgradeScreen == null)
            upgradeScreen = GameObject.Find("UpgradeScreen");
        if (winScreen == null)
            winScreen = GameObject.Find("WinScreen");
        if (progressScreen == null)
            progressScreen = GameObject.Find("ProgressScreen");
    }

    private void OnEnable()
    {
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
            winScreen.SetActive(false);
            gameObject.GetComponent<UIDocument>().enabled = false;
            progressScreen.SetActive(false);
            upgradeScreen.SetActive(false);
        }
        else
        {
            GetUIDocuments();

            gameObject.GetComponent<UIDocument>().enabled = UIActiveStates[0];
            upgradeScreen.SetActive(UIActiveStates[1]);
            progressScreen.SetActive(UIActiveStates[2]);
            // winScreen.SetActive(UIActiveStates[3]); KEEP WIN SCREEN ALWAYS OFF

            GetUIVariables();

            UpdateUpgrades();

            GetUIVariables();
        }
    }

    private void PressedMove(ClickEvent evt, MoveData move)
    {
        player.GetComponent<ReptileScript>().PerformMove(move);
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

    public float ratioInnerOuterCircle()
    {
        if (quickTime.style.display == DisplayStyle.None)
        {
            return -1.0f;
        }
        IStyle outerStyle = OuterCircle.style;
        IStyle innerStyle = InnerCircle.style;
        return innerStyle.width.value.value / outerStyle.width.value.value;
    }

    public void circleHit()
    {
        // resets the outer circle
        IStyle style = OuterCircle.style;
        style.width = MAX_SIZE;
        style.height = MAX_SIZE;

        IStyle outerStyle = OuterCircle.style;
        IStyle innerStyle = InnerCircle.style;
        outerStyle.display = DisplayStyle.None;
        innerStyle.display = DisplayStyle.None;
    }

    public void UpdateUpgrades()
    {
        if (upgrade1 == null)
            GetUIVariables();

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

                if (GameState.current.currentReptile().evoPoints >= nodes[i].cost || adUpgradeCounter >= MAX_NUM_OF_AD_UPGRADES || adsManager == null
                    || AdsManager.rewardedAd.CanShowAd() == false) // if you have enough evo points, show the cost
                {
                    upgrades[i].Q<Label>("EvoAmount").text = nodes[i].cost.ToString();
                    upgrades[i].RegisterCallback<ClickEvent, UpgradeNode>(BuyUpgrade, nodes[i]);
                }
                else
                { // else write, show add
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
        if (adUpgradeCounter >= MAX_NUM_OF_AD_UPGRADES)
        {
            return; // stop watching ads after you hit max ads
        }
        adsManager.GetComponent<AdsInitializer>().data_for_BoughtUpgrade = data;
        adsManager.GetComponent<AdsInitializer>().ShowRewardedAd();
    }

    public void BuyUpgrade(ClickEvent evt, UpgradeNode node)
    {
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
        if (node.category == "Tongue")
        {
            GameState.current.currentReptile().tonguePeakLength += node.amount;
            GameState.current.currentReptile().tongueRetractionSpeed *= node.amount2;
            GameState.current.currentReptile().tongueSpeed *= node.amount2;
        }
        else if (node.category == "Health")
        {
            GameState.current.currentReptile().MAX_HEALTH += node.amount;
            player.GetComponent<ReptileScript>().health = GameState.current.currentReptile().MAX_HEALTH;
        }
        else if (node.category == "AttackSpeed")
        {
            GameState.current.currentReptile().attackSpeed /= node.amount;
            player.GetComponent<ReptileScript>().attackSpeed = GameState.current.currentReptile().attackSpeed;
        }
        else if (node.category == "Damage")
        {
            GameState.current.currentReptile().damage += node.amount;
            player.GetComponent<ReptileScript>().damage = GameState.current.currentReptile().damage;
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
        if (upgradeScreen.activeSelf)
        {
            player.GetComponent<AudioSource>().volume = (volumeSlider.value) / 100;
            playerCam.GetComponent<AudioSource>().volume = (volumeSlider.value) / 100;
        }

        float evoPoints = GameState.current.currentReptile().evoPoints;
        if (evoPoints >= 1000000)
            EVPoints.text = (evoPoints / 1000000.0f).ToString("F2") + "m";
        else if (evoPoints >= 1000)
            EVPoints.text = (evoPoints / 1000.0f).ToString("F2") + "k";
        else
            EVPoints.text = GameState.current.currentReptile().evoPoints.ToString();
        if (player.GetComponent<ReptileScript>().health < 0)
        {
            yourHealth.text = "0";
        }
        else
        {
            yourHealth.text = ((int)player.GetComponent<ReptileScript>().health).ToString();
            IStyle extraHealthStyle = extraHealth.style;
            float extraHealthAmount = player.GetComponent<ReptileScript>().health - (int)player.GetComponent<ReptileScript>().health;
            extraHealthStyle.height = new StyleLength(Length.Percent(extraHealthAmount * 100));
        }

        if (GameObject.Find("Level").GetComponent<LevelScript>().pauseGame)
            return;

        // if in battle mode
        if(GetComponent<UIDocument>().visualTreeAsset == battleUI)
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            // your health
            IStyle yourHealthBarStyle = root.Q<VisualElement>("YourHealthBar").style;
            if (player.GetComponent<ReptileScript>().health < 0)
                yourHealthBarStyle.width = new StyleLength(Length.Percent(0));
            else
                yourHealthBarStyle.width = new StyleLength(Length.Percent( (player.GetComponent<ReptileScript>().health / GameState.current.currentReptile().MAX_HEALTH) * 100.0f));

            for (int i = 0; i < GameState.current.currentReptile().moves.Count; i++)
            {
                MoveData currentMove = GameState.current.currentReptile().moves[i];
                IStyle cooldownStyle = root.Q<Button>("Move" + (i + 1).ToString()).Q<VisualElement>("Cooldown").style;
                cooldownStyle.width = new StyleLength(Length.Percent( (currentMove.timer / currentMove.coolDownTime) * 100.0f) );
            }
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }
}