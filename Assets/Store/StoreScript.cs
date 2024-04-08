using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StoreScript : MonoBehaviour
{
    /* Define member variables*/
    private const string tabClassName = "tab";
    private const string currentlySelectedTabClassName = "currentlySelectedTab";
    private const string unselectedContentClassName = "unselectedContent";
    // Tab and tab content have the same prefix but different suffix
    // Define the suffix of the tab name
    private const string tabNameSuffix = "Tab";
    // Define the suffix of the tab content name
    private const string contentNameSuffix = "Content";

    private VisualElement root;

    public Button backButton;
    public Label crownsLabel;

    public GameObject loadingScreen;
    public GameObject podium;

    void Awake()
    {
        SaveGameScript.Load();
    }

    // TABS CODE START
    // assigns the callback functions for the tab buttons
    public void RegisterTabCallbacks()
    {
        UQueryBuilder<Button> tabs = GetAllTabs();
        tabs.ForEach((Button tab) => {
            tab.RegisterCallback<ClickEvent>(TabOnClick);
        });
    }

    /* Method for the tab on-click event: 

       - If it is not selected, find other tabs that are selected, unselect them 
       - Then select the tab that was clicked on
    */
    private void TabOnClick(ClickEvent evt)
    {
        Button clickedTab = evt.currentTarget as Button;
        if (!TabIsCurrentlySelected(clickedTab))
        {
            GetAllTabs().Where(
                (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
            ).ForEach(UnselectTab);
            SelectTab(clickedTab);
        }
    }
    //Method that returns a Boolean indicating whether a tab is currently selected
    private static bool TabIsCurrentlySelected(Button tab)
    {
        return tab.ClassListContains(currentlySelectedTabClassName);
    }

    private UQueryBuilder<Button> GetAllTabs()
    {
        return root.Query<Button>(className: tabClassName);
    }

    /* Method for the selected tab: 
       -  Takes a tab as a parameter and adds the currentlySelectedTab class
       -  Then finds the tab content and removes the unselectedContent class */
    private void SelectTab(Button tab)
    {
        tab.AddToClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.RemoveFromClassList(unselectedContentClassName);
    }

    /* Method for the unselected tab: 
       -  Takes a tab as a parameter and removes the currentlySelectedTab class
       -  Then finds the tab content and adds the unselectedContent class */
    private void UnselectTab(Button tab)
    {
        tab.RemoveFromClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.AddToClassList(unselectedContentClassName);
    }

    // Method to generate the associated tab content name by for the given tab name
    private static string GenerateContentName(Button tab) =>
        tab.name.Replace(tabNameSuffix, contentNameSuffix);

    // Method that takes a tab as a parameter and returns the associated content element
    private VisualElement FindContent(Button tab)
    {
        return root.Q(GenerateContentName(tab));
    }
    // TABS CODE END


    void FillInReptileData()
    {
        UQueryBuilder<Button> buttons = root.Query<Button>(className: "reptileButton");
        int CurrentEvolution = GameState.current.currentReptile().currentEvolution;
        print(CurrentEvolution);
        buttons.ForEach((Button button) => {
            string reptile_name = button.hierarchy.parent.name.Substring(0, button.hierarchy.parent.name.IndexOf("Row"));
            int reptile_stage = Int32.Parse(button.name.Substring(5));
            if(reptile_stage > CurrentEvolution)
            {
                // fade it to black
                IStyle picStyle = button.Query<VisualElement>("Pic").AtIndex(0).style;
                print(button.Query<VisualElement>("Pic").AtIndex(0).name);
                picStyle.unityBackgroundImageTintColor = new Color(20.0f / 255, 20.0f / 255, 20.0f / 255);
            } else
            {
                // button.RegisterCallback<ClickEvent>()
            }
        });

        UQueryBuilder<GroupBox> buy_boxes = root.Query<GroupBox>(className: "buy-box");
        buy_boxes.ForEach((GroupBox groupBox) => {
            string reptile_name = groupBox.hierarchy.parent.name.Substring(0, groupBox.hierarchy.parent.name.IndexOf("Row"));
            Button buyButton = groupBox.Q<Button>("BuyButton");
            buyButton.RegisterCallback<ClickEvent, string>(buyEggButton, reptile_name);
            print(reptile_name);
        });
    }

    void buyEggButton(ClickEvent evt, string reptile_name)
    {
        if (GameState.current.addEgg(reptile_name) == false)
        {
            Debug.Log("DID NOT HAVE ENOUGH TO BUY EGG");
            // show some error message
        }
        else
        {
            Debug.Log("BOUGHT EGG");
        }
    }

    void reptileButtonClick(ClickEvent evt)
    {
        // podium.GetComponent<PodiumScript>().ShowModel()
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        backButton = root.Q<Button>("BackButton");
        backButton.RegisterCallback<ClickEvent>(BackToGame);

        crownsLabel = root.Q<Label>("CrownsLabel");

        loadingScreen = GameObject.Find("LoadingScreen");
        loadingScreen.SetActive(false);

        RegisterTabCallbacks();

        FillInReptileData();
    }

    private void BackToGame(ClickEvent evt)
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadSceneAsync(0);
    }

    void Start()
    {
        podium = GameObject.Find("Podium");
    }

    // Update is called once per frame
    void Update()
    {
        crownsLabel.text = GameState.current.crowns.ToString();
    }

    void OnDestroy()
    {
        SaveGameScript.Save();
    }
}
