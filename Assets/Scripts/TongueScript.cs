using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TongueScript : MonoBehaviour
{
    public GameObject playerCam;
    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.Find("Main Camera");

    }

    //Detect when there is a collision starting
    void OnCollisionEnter(Collision collision)
    {
        PreyScript prey = collision.gameObject.GetComponent<PreyScript>();
        ReptileScript reptile = gameObject.transform.root.GetComponent<ReptileScript>();
        if (prey != null && reptile.tongueOut)
        {
            GameState.current.addEvoPoints(prey.evoPoints);

            Vector3 reptilePosScreen = playerCam.GetComponent<Camera>().WorldToScreenPoint(reptile.transform.position);
            if(reptilePosScreen.x > 1200)
            {
                reptilePosScreen.x = 1200;
            } else if (reptilePosScreen.x < 15)
            {
                reptilePosScreen.x = 15;
            }

            GameObject.Find("Indicators").transform.position = reptilePosScreen;

            reptile.evoText.GetComponent<TMP_Text>().color = new Color(1, 1, 1, 1);
            reptile.evoText.GetComponent<TMP_Text>().text = prey.evoPoints.ToString();
            reptile.evoImage.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
            switch (prey.name)
            {
                case "Ladybug":
                    reptile.health += 0.1f;
                    reptile.damageIndicator.GetComponent<TMP_Text>().text = "Extra Health";
                    reptile.damageIndicator.GetComponent<TMP_Text>().color = new Color(0.9f, 0.2f, 0, 1);
                    break;
                case "Spider":
                    reptile.damage += 0.25f;
                    reptile.damageIndicator.GetComponent<TMP_Text>().text = "Increased Damage";
                    reptile.damageIndicator.GetComponent<TMP_Text>().color = new Color(0.0f, 0.0f, 0.9f, 1);
                    break;
                case "Butterfly":
                    reptile.attackSpeed /= 1.2f;
                    reptile.damageIndicator.GetComponent<TMP_Text>().text = "Faster attacks";
                    reptile.damageIndicator.GetComponent<TMP_Text>().color = new Color(0.9f, 0.9f, 0, 1);
                    break;
            }

            Destroy(collision.gameObject);
            reptile.isRetracting = true;
            reptile.tongueTimer = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
