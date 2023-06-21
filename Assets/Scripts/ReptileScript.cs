using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;

public class ReptileScript : MonoBehaviour
{
    private Rigidbody controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 3.5f;
    public bool canMove = true;
    public bool devMode = false; // for some reason, now i can't play the game b/c player moves too slow on unity simulator
    public GameObject tongue;

    private float GRAVITY = 2.0f;
    
    public GameObject level;
    public GameObject UI;
    public GameObject particleSystem;

    public GameObject damageIndicator;
    private float fadeSpeed = 0.35f;

    public Animator animator;

    private Vector2 touchStartPosition;
    private Vector2 touchLastPosition;
    private Touch theTouch;
    private float timeSincePressed;
    private string touchType = ""; // could be TONGUE, MOVEMENT, 
    
    public bool isRetracting = false;
    public bool tongueOut = false;
    public float tongueTimer = 0.0f;

    public float health = GameState.current.MAX_HEALTH;
    public GameObject battleStage;

    internal float HURT_TIME = 0.75f;
    internal float timeSinceHurt = 0.0f;
    private float timeSinceFlash = 0.0f; // time in between flashes

    private float evolveTimer = 0.0f;
    private float EVOLVE_TIME = 2.0f; // half the time of particle system

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<Rigidbody>();

        UI = GameObject.Find("UIDocument");
        level = GameObject.Find("Level");
        damageIndicator = GameObject.Find("Indicator");

        particleSystem = gameObject.transform.Find("ParticleSystem").gameObject;

        animator = gameObject.transform.Find("Model").gameObject.GetComponent<Animator>();

        health = GameState.current.MAX_HEALTH;
        Evolve(GameState.current.currentEvolution);

        if (devMode)
            playerSpeed *= 35.0f;
    }

    //Detect when there is a collision starting
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Crown") {
            if (GameState.current.crowns < 1)
            {
                GameState.current.crowns += 1;
            }
            Destroy(collision.gameObject);

            UI.GetComponent<UI>().winScreen.SetActive(true);

            // renabling winscreen button (continue)
            UI.GetComponent<UI>().GetUIVariables();
            level.GetComponent<LevelScript>().isMoving = false;
        }
        else if (collision.gameObject.name != "Floor" && collision.gameObject.GetComponent<PreyScript>() == null)
        {
            Debug.Log("You hit a " + collision.gameObject.name);
            health -= 1.0f;

            timeSinceHurt = HURT_TIME;
        }
    }

    void Evolve(int stage_num) { // stage 1, 2, 3
        GameObject model = transform.Find("Model").gameObject;
        GameObject mesh = transform.Find("Model").Find("Mesh").gameObject;
        if (stage_num == 1) {
            DestroyImmediate(model);
            GameObject loadedModel = Resources.Load("Evolutions/Gecko_Stage1") as GameObject;
            GameObject newModel = Instantiate(loadedModel, gameObject.transform);
            newModel.transform.localPosition = new Vector3(0, 0.676f, 0);
            print(newModel.name);
            newModel.name = "Model";
            animator = gameObject.transform.Find("Model").gameObject.GetComponent<Animator>();
        } else if(stage_num == 2) {
            DestroyImmediate(model);
            GameObject loadedModel = Resources.Load("Evolutions/Gecko_Stage2") as GameObject;
            GameObject newModel = Instantiate(loadedModel, gameObject.transform);
            newModel.transform.localPosition = new Vector3(0, 0.676f, 0);
            print(newModel.name);
            newModel.name = "Model";
            animator = gameObject.transform.Find("Model").gameObject.GetComponent<Animator>();
        }
        else if (stage_num == 3)
        {
            DestroyImmediate(model);
            GameObject loadedModel = Resources.Load("Evolutions/Gecko_Stage3") as GameObject;
            GameObject newModel = Instantiate(loadedModel, gameObject.transform);
            newModel.transform.localPosition = new Vector3(0, 0.676f, 0);
            print(newModel.name);
            newModel.name = "Model";
            animator = gameObject.transform.Find("Model").gameObject.GetComponent<Animator>();
            tongue.transform.localPosition = new Vector3(0, 0.076f, 0.84f);
        }
        
        GameState.current.currentEvolution = stage_num;
        evolveTimer = 0;

        level.GetComponent<LevelScript>().pauseGame = false;
    }

    internal void BattleUpdate() {

        if (battleStage.GetComponent<BattleStageScript>().opponentsOrdering.Count == 0) {
            if (battleStage.GetComponent<BattleStageScript>().crown != null)
            {
                // defeated all enemies so go get the crown
                animator.SetBool("isMoving", true);
                controller.MovePosition(transform.position + (new Vector3(0, 0, 1.0f) * Time.deltaTime * playerSpeed));
            } else
            {
                print("PICKED UP CROWN");
                animator.SetBool("isMoving", false);
            }
        }
        // move close to current opponent
        else if (Vector3.Distance(battleStage.GetComponent<BattleStageScript>().opponentsOrdering[0].transform.position, gameObject.transform.position) > 4.5f) {
            animator.SetBool("isMoving", true);
            controller.MovePosition(transform.position + (new Vector3(0, 0, 1.0f) * Time.deltaTime * playerSpeed));
        } else // no longer moving
        {
            animator.SetBool("isMoving", false);
        }

        // tap to attack (timing)
        if (Input.touchCount > 0)
        { 
            theTouch = Input.GetTouch(0);
            if (theTouch.phase == TouchPhase.Began)
            {
                animator.SetTrigger("attack");

                damageIndicator.transform.position = theTouch.position;
                float timingRatio = UI.GetComponent<UI>().ratioInnerOuterCircle();
                if (timingRatio > 0.95)
                {
                    damageIndicator.GetComponent<TMP_Text>().text = "Perfect";
                    damageIndicator.GetComponent<TMP_Text>().color = new Color(0, 1, 0, 1);
                }
                else if (timingRatio > 0.75)
                {
                    damageIndicator.GetComponent<TMP_Text>().text = "A little early";
                    damageIndicator.GetComponent<TMP_Text>().color = new Color(0.75f, 0.6f, 0, 1);
                }
                else 
                {
                    damageIndicator.GetComponent<TMP_Text>().text = "Too early";
                    damageIndicator.GetComponent<TMP_Text>().color = new Color(1, 0.0f, 0, 1);
                }

                float currentDamage = ( Mathf.Pow(16.0f, timingRatio) / 16.0f ) * GameState.current.damage;
                UI.GetComponent<UI>().circleHit();
                battleStage.GetComponent<BattleStageScript>().DamageOpponent(currentDamage);
            }
        }
    }

    void CheckDamageAnimation()
    {
        GameObject model = gameObject.transform.Find("Model").gameObject;
        GameObject mesh = model.transform.Find("Mesh").gameObject;
        if (timeSinceHurt > 0)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            tongue.transform.Find("Tongue").gameObject.GetComponent<BoxCollider>().enabled = false;

            if (model.activeSelf)
            {
                Material m_Material = mesh.GetComponent<Renderer>().material;
                m_Material.color = Color.red;
            }

            if (timeSinceFlash <= 0)
            {
                mesh.GetComponent<Renderer>().enabled = false;
                timeSinceFlash = 0.225f;
            }
            else if (timeSinceFlash < 0.15f)
            {
                mesh.GetComponent<Renderer>().enabled = true;
            }

            timeSinceHurt -= Time.deltaTime;
            timeSinceFlash -= Time.deltaTime;
        }
        else 
        {
            mesh.GetComponent<Renderer>().enabled = true;
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            tongue.transform.Find("Tongue").gameObject.GetComponent<BoxCollider>().enabled = true;

            Material m_Material = mesh.GetComponent<Renderer>().material;
            m_Material.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* temp 
        if(GameState.current.currentEvolution != 3)
            Evolve(3); 
        GameState.current.addEvoPoints(1);
        */

        // makes damage indicators slowly fade away
        if (damageIndicator.GetComponent<TMP_Text>().color.a > 0)
        {
            Color prevColor = damageIndicator.GetComponent<TMP_Text>().color;
            damageIndicator.GetComponent<TMP_Text>().color = new Color(prevColor.r, prevColor.g, prevColor.b, prevColor.a - (Time.deltaTime * fadeSpeed));
        }

        if (health <= 0)
        {
            return; // don't do anything if dead
        }

        // flashing damage animation
        if (!level.GetComponent<LevelScript>().pauseGame) // if not paused
        {
            CheckDamageAnimation();
        }

        // evolve if total Evo points is enough
        if (GameState.current.totalEvoPoints >= GameState.current.stage1Evolution && GameState.current.currentEvolution == 1)
        {
            level.GetComponent<LevelScript>().pauseGame = true;
            if (!particleSystem.GetComponent<ParticleSystem>().isPlaying)
                particleSystem.GetComponent<ParticleSystem>().Play();
            evolveTimer += Time.deltaTime;
            if(evolveTimer >= EVOLVE_TIME)
                Evolve(2);
        } else if (GameState.current.totalEvoPoints >= GameState.current.stage2Evolution && GameState.current.currentEvolution == 2)
        {
            level.GetComponent<LevelScript>().pauseGame = true;
            if (!particleSystem.GetComponent<ParticleSystem>().isPlaying)
                particleSystem.GetComponent<ParticleSystem>().Play();
            evolveTimer += Time.deltaTime;
            if (evolveTimer >= EVOLVE_TIME)
                Evolve(3);
        }

        if (level.GetComponent<LevelScript>().battleStage != null) {
            tongue.GetComponent<Transform>().localScale = new Vector3(1, 1, 0);
            tongue.SetActive(false);
            BattleUpdate();
            return;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (tongueOut)
        {
            tongue.SetActive(true);
            animator.SetBool("tongueOut", true);
            tongueTimer += Time.deltaTime;
            if (GameState.current.currentEvolution != 3 || tongueTimer > 0.13f) // tongue doesn't start until gecko3 bends his head down
            {
                float tongueLength = tongueTimer * GameState.current.tongueSpeed;
                if ((tongueTimer * GameState.current.tongueSpeed) > GameState.current.tonguePeakLength || isRetracting)
                {
                    if (!isRetracting)
                    { // if not retracting, then reset timer
                        tongueTimer = 0.0f;
                    }
                    isRetracting = true;
                    tongueLength = tongue.GetComponent<Transform>().localScale.z - (Time.deltaTime * GameState.current.tongueRetractionSpeed);
                    if (tongueLength < 0)
                    {
                        isRetracting = false;
                        tongueOut = false;
                        tongueTimer = 0.0f;
                        tongueLength = 0;
                    }
                }
                tongue.GetComponent<Transform>().localScale = new Vector3(1, 1, tongueLength);
            }
        }
        else if (tongue.GetComponent<Transform>().localScale.z > 1) {
            animator.SetBool("tongueOut", false);
            tongue.GetComponent<Transform>().localScale = new Vector3(1, 1, tongue.GetComponent<Transform>().localScale.z - (Time.deltaTime * GameState.current.tongueRetractionSpeed));
        } else
        {
            tongue.SetActive(false);
            if(GameState.current.currentEvolution == 3)
                animator.SetBool("tongueOut", false);
        }

        if (touchType == "SPECIAL_ABILITY")
        {
            // special ability code
            /*
            if (GameState.current.currentEvolution == 2)
            { // jump stage
                float currentHeight = -Mathf.Pow(timeSincePressed - 1, 2) + 2;
                if (currentHeight < 0)
                    touchType = "";
                move = new Vector3(move.x, currentHeight, move.z);
            }
            else if (GameState.current.currentEvolution == 3)
            { // fly stage

            }
            */
        }
        timeSincePressed += Time.deltaTime;

        if (Input.touchCount > 0){ // currently touching
            theTouch = Input.GetTouch(0);
            if(theTouch.phase == TouchPhase.Began){
                touchStartPosition = theTouch.position;
                touchLastPosition = touchStartPosition;
            } else if(theTouch.phase == TouchPhase.Ended){
                isRetracting = false;
                timeSincePressed = 0;
                if(touchType == "MOVEMENT")
                    touchType = "";
            }
            else 
            {
                if (timeSincePressed > 0.1f)
                {
                    if ((theTouch.position.y - touchLastPosition.y) > 30)
                    {
                        // touchType = "TONGUE";
                        tongueOut = true;
                    } else if (Mathf.Abs(theTouch.position.x - touchStartPosition.x) > 20)
                    {
                        touchType = "MOVEMENT";
                    } else {
                        touchType = "SPECIAL_ABILITY";
                        
                    }
                    
                }

                if (touchType == "MOVEMENT")
                {
                    float x = theTouch.position.x - touchLastPosition.x;
                    move = new Vector3(x / 32, 0, 0);
                }
                
                touchLastPosition = theTouch.position;
            }
            
        }

        // move.y -= GRAVITY * Time.deltaTime;

        if (canMove)
        {
            if(move.magnitude > 0 || level.GetComponent<LevelScript>().isMoving)
                animator.SetBool("isMoving", true);
            else
                animator.SetBool("isMoving", false);

        }
        else
        {
            print("STOPPED MOVING");
            move = new Vector3(0, 0, 0);
            animator.SetBool("isMoving", false);
        }

        controller.MovePosition(transform.position + (move * Time.deltaTime * playerSpeed));
    }
}
