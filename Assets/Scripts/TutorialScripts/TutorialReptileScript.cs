using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialReptileScript : ReptileScript
{
    //Detect when there is a collision starting
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Crown")
        {
            if (GameState.current.crowns < 1)
            {
                GameState.current.crowns += 1;
            }
            UI.GetComponent<UI>().winScreen.SetActive(true);

            // renabling winscreen button (continue)
            VisualElement winRoot = UI.GetComponent<UI>().winScreen.GetComponent<UIDocument>().rootVisualElement;
            Button winScreenContinue = winRoot.Q<Button>("Continue");
            winScreenContinue.RegisterCallback<ClickEvent>(level.GetComponent<LevelScript>().EndGame);
            level.GetComponent<LevelScript>().isMoving = false;
        }
        else if (collision.gameObject.name != "Floor" && collision.gameObject.GetComponent<PreyScript>() == null && level.GetComponent<TutorialLevelScript>().isMoving)
        {
            Debug.Log("You hit a " + collision.gameObject.name);
            health -= 1.0f;
            if(level.GetComponent<TutorialLevelScript>().hitRock == TutorialLevelScript.TipStatus.NotSeen)
            {
                level.GetComponent<TutorialLevelScript>().hitRock = TutorialLevelScript.TipStatus.JustSeen;
            } else if (health == 0.0f && level.GetComponent<TutorialLevelScript>().died == TutorialLevelScript.TipStatus.NotSeen)
            {
                level.GetComponent<TutorialLevelScript>().died = TutorialLevelScript.TipStatus.JustSeen;
            }
        }
    }
}
