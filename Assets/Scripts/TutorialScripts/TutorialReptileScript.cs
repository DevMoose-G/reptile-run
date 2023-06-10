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
            winScreenContinue.RegisterCallback<ClickEvent>(UI.GetComponent<UI>().EndGame);
            level.GetComponent<LevelScript>().isMoving = false;
        }
        else if (collision.gameObject.name != "Floor" && collision.gameObject.GetComponent<PreyScript>() == null)
        {
            Debug.Log("You hit a " + collision.gameObject.name);
            health -= 1.0f;
            if(health == 1.0f)
            {
                level.GetComponent<TutorialLevelScript>().hitRock = true;
            } else if (health == 0.0f)
            {
                level.GetComponent<TutorialLevelScript>().died = true;
            }
        }
    }
}
