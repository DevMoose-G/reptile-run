using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    private Label EVPoints;
    private VisualElement OuterCircle;
    private VisualElement InnerCircle;
    public GroupBox quickTime;
    private float timerTillShown = GameState.current.attackSpeed;

    public Label yourHealth;
    public Label theirHealth;

    public GameObject player;
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

    // Start is called before the first frame update
    void Start()
    {

        GameState.current.Clear();
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

        player = GameObject.Find("Reptile");

        adsManager = GameObject.Find("Ads Manager");
        adsManager.GetComponent<AdsInitializer>().LoadRewardedAd();

        UpdateUpgrades();
    }

    private void OnEnable() {
        upgradeScreen = GameObject.Find("UpgradeScreen");
        UpgradeTree.Load();
        winScreen = GameObject.Find("WinScreen");
        progressScreen = GameObject.Find("ProgressScreen");

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
        UpdateUpgrades();

        progressScreen.SetActive(false);

        // anything you do hear has to be redone when you renable winScreen
        VisualElement winRoot = winScreen.GetComponent<UIDocument>().rootVisualElement;

        winScreenContinue = winRoot.Q<Button>("Continue");
        winScreenContinue.RegisterCallback<ClickEvent>(EndGame);
        winScreen.SetActive(false);
    }

    private void StoreScene(ClickEvent evt)
    {
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

        UpgradeGroup nodeGroup = GameState.current.upgradeTree.GetUpgradeGroup();
        List<UpgradeNode> nodes = nodeGroup.toList();
        List<VisualElement> upgrades = new List<VisualElement> { upgrade1, upgrade2, upgrade3, upgrade4 };
        for (int i = 0; i < nodes.Count; i++)
        {
            IStyle upgradeStyle = upgrades[i].style;
            if (nodes[i] != null)
            {
                upgradeStyle.visibility = Visibility.Visible;
                upgrades[i].Q<Label>("Category").text = nodes[i].category + " " + toRomanNumerals(nodes[i].level);


                if (GameState.current.evoPoints >= nodes[i].cost || adUpgradeCounter >= MAX_NUM_OF_AD_UPGRADES) // if you have enough evo points, show the cost
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
                upgradeStyle.visibility = Visibility.Hidden;
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

    private void BuyUpgrade(ClickEvent evt, UpgradeNode node)
    {
        print("Buying UPGRADE");
        if (GameState.current.evoPoints >= node.cost)
        {
            GameState.current.subtractEvoPoints(node.cost);
            ApplyUpgrade(evt, node);
        }
    }

    public void ApplyUpgrade(ClickEvent evt, UpgradeNode node)
    {
        print("BOUGHT UPGRADE");
        
        GameState.current.upgradeTree.nodes_obtained.Add(node.id);

        // apply the upgrade
        print(node.category);
        if (node.category == "Tongue")
        {
            GameState.current.tonguePeakLength += node.amount;
            GameState.current.tongueRetractionSpeed *= node.amount2;
            GameState.current.tongueSpeed *= node.amount2;
        }
        else if (node.category == "Health") { 
            GameState.current.MAX_HEALTH += node.amount;
            player.GetComponent<ReptileScript>().health = GameState.current.MAX_HEALTH;
        }
        else if (node.category == "AttackSpeed")
        {
            GameState.current.attackSpeed /= node.amount;
        }
        else if (node.category == "Damage")
        {
            print("Got here");
            GameState.current.damage += node.amount;
            print(node.amount);
            print(GameState.current.damage);
        }

        UpdateUpgrades();
    }

    private void StartGame(ClickEvent evt)
    {
        GameObject.Find("Level").GetComponent<LevelScript>().isMoving = true;
        upgradeScreen.SetActive(false);
        player.GetComponent<ReptileScript>().animator.SetBool("isMoving", true);
    }

    public void EndGame(ClickEvent evt) {
        print("ENDING GAME");
        upgradeScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        EVPoints.text = GameState.current.evoPoints.ToString();
        if (player.GetComponent<ReptileScript>().health < 0)
        {
            yourHealth.text = "0";
        }
        else
        {
            yourHealth.text = (System.Math.Truncate(player.GetComponent<ReptileScript>().health * 100) / 100).ToString();
        }

        // making outer circle smaller but still centered
        IStyle quickTimeStyle = quickTime.style;
        IStyle outerStyle = OuterCircle.style;
        IStyle innerStyle = InnerCircle.style;

        if (quickTimeStyle.display != DisplayStyle.None && !progressScreen.activeSelf) {
            

            if (outerStyle.width.value.value <= MIN_SIZE) // resets circle once it hits the inner circle
            {
                outerStyle.width = MAX_SIZE;
                outerStyle.height = MAX_SIZE;
                timerTillShown = GameState.current.attackSpeed;
            }

            if (outerStyle.display == DisplayStyle.None) // if outer circle is invisible, wait for timerTillShown to hit 0, then show both circles
            {
                timerTillShown -= Time.deltaTime;
                if (timerTillShown <= 0) {
                    timerTillShown = GameState.current.attackSpeed;
                    outerStyle.display = DisplayStyle.Flex;
                    innerStyle.display = DisplayStyle.Flex;
                }
            }
            else
            {
                circleSpeed = (MAX_SIZE - MIN_SIZE) / GameState.current.attackSpeed;

                timerTillShown -= Time.deltaTime;
                outerStyle.width = OuterCircle.style.width.value.value - (Time.deltaTime * circleSpeed);
                outerStyle.height = OuterCircle.style.height.value.value - (Time.deltaTime * circleSpeed);

                outerStyle.top = 32 - (outerStyle.height.value.value / 2) - (innerStyle.height.value.value / 2);
                outerStyle.left = (innerStyle.height.value.value / 2) - (outerStyle.width.value.value / 2) + 32;
            }
        } 

    }
}
