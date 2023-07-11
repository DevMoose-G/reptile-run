using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AdOffersScript : MonoBehaviour
{
    private GameObject adsManager;

    // Start is called before the first frame update
    void Start()
    {
        adsManager = GameObject.Find("Ads Manager");

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(CloseUI);
        root.Q<Button>("AdButton").RegisterCallback<ClickEvent>(CloseUI);
        root.Q<Label>("AdLabel").text = "Watch this ad to unlock <color=#af944cff>Screech</color> for one turn!";
    }

    void CloseUI(ClickEvent evt)
    {
        gameObject.SetActive(false);
    }

    void AdButton(ClickEvent evt)
    {
        // add the move

        adsManager.GetComponent<AdsInitializer>().ShowRewardedAd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
