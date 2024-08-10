using System;
using TMPro;
using UnityEngine;
using DiceBreakerUtility;
using System.Collections;
using UnityEngine.Pool;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch= UnityEngine.InputSystem.EnhancedTouch;

public class DiceScript : MonoBehaviour
{
    //---Refs---
    Camera cam;

    [SerializeField]
    GameObject spawnPoint;
    private Rigidbody2D diceRb;
    public ObjectPool<DiceScript> objectPool;

    [SerializeField]
    TextMeshProUGUI diceNumberText;
    
    public DiceState diceState;

    const string IGNORE_COLLISION_LAYER = "Ignore Collisions", PLAYER_LAYER = "Player";
    [SerializeField]
    private float
        collisionColliderRadius = 0.5f,
        detectionColliderRadius = 1f,
        shootForce = 15f,
        minRadiusOffset = 0.75f,
        multiplierLimit,
        camShakeAmp = 1f,
        camShakeDuration = 0.5f;

    private float distanceMultiplier, minRadius;

    [SerializeField]
    private int maxDiceNum = 6;
    private int diceNumber;

    private Vector3 dir;
    Vector3 worldPoints;
    Vector2 startScreenPos;
    CircleCollider2D diceCollider;

    private void Awake()
    {
        cam = Camera.main;
        diceRb = GetComponent<Rigidbody2D>();
        diceCollider = GetComponent<CircleCollider2D>();
        diceCollider.radius = detectionColliderRadius;
        DiceRoll();
        if (spawnPoint == null)
        {
            spawnPoint = GameObject.FindWithTag("Spawn Point");
        }
        minRadius = transform.localScale.x / 2 + minRadiusOffset;
    }

    private void OnEnable()
    {
        diceCollider.radius = detectionColliderRadius;
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_onFingerDown;
        ETouch.Touch.onFingerUp += ShootDiceOnRelease;
        ETouch.Touch.onFingerMove += CalculateDirection;
        DiceRoll();
    }

    private void Touch_onFingerDown(Finger touchedFinger)
    {
        startScreenPos = touchedFinger.currentTouch.startScreenPosition;
        Ray ray = cam.ScreenPointToRay(startScreenPos);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        if (hit2D.collider != null && diceState == DiceState.readyState && hit2D.collider.gameObject.CompareTag(GameManager.instance.DICE_TAG))
        {
            diceState = DiceState.selectedState;
        }
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= Touch_onFingerDown;
        ETouch.Touch.onFingerUp -= ShootDiceOnRelease;
        ETouch.Touch.onFingerMove -= CalculateDirection;
        EnhancedTouchSupport.Disable();
        ResetDice();
    }
    private void ShootDiceOnRelease(Finger touchedFinger)
    {
        if (diceState == DiceState.selectedState)
        {
            StartCoroutine("Move");
        }
    }
    private void CalculateDirection(Finger touchedFinger)
    {
        if (diceState == DiceState.selectedState)
        {
            Vector3 screenPoints = new Vector3(touchedFinger.screenPosition.x, touchedFinger.screenPosition.y, 0f);
            worldPoints = cam.ScreenToWorldPoint(screenPoints);
            worldPoints.z = 0f;
            dir = worldPoints - transform.position;
            Vector2 rotvec = worldPoints - transform.position;
            transform.up = -rotvec.normalized;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && diceState == DiceState.readyState)
        {
            diceNumber = UnityEngine.Random.Range(1, maxDiceNum + 1);
            diceNumberText.text = Convert.ToString(diceNumber);
        }
        if (transform.position.y > Utilty.GetScreenHalfHeight() + (gameObject.transform.localScale.x/2))
        {
            objectPool.Release(this);
        }
    }

    IEnumerator Move()
    {
        WaitForSeconds wait = new WaitForSeconds(0.001f);

        diceState = DiceState.throwState;
        diceCollider.radius = collisionColliderRadius;
        distanceMultiplier = dir.magnitude;
        distanceMultiplier = Mathf.Clamp(distanceMultiplier, 0f, multiplierLimit);
        dir = transform.up;
        ChangeCollisionLayer(PLAYER_LAYER);
        while (diceState == DiceState.throwState)
        {
            transform.Translate(dir.normalized * shootForce * distanceMultiplier * Time.deltaTime, Space.World);   
            if (diceState == DiceState.destroyState)
                yield break;
            else
                yield return wait;
        }
    }

    void ChangeCollisionLayer(string layerMask)
    {
        this.gameObject.layer = LayerMask.NameToLayer(layerMask);
    }

    public void DiceRoll()
    {
        diceState = DiceState.readyState;
        if(gameObject.activeInHierarchy == false)
            gameObject.SetActive(true);

        diceNumber = UnityEngine.Random.Range(1, maxDiceNum + 1);
        diceNumberText.text = Convert.ToString(diceNumber);
        ChangeCollisionLayer(IGNORE_COLLISION_LAYER);
    }

    public void ResetDice()
    {
        this.gameObject.SetActive(false);
        diceRb.velocity = Vector2.zero;
        //Null ref error here
        if (gameObject.GetComponent<DiceScript>() != null)
            gameObject.transform.position = spawnPoint.transform.position;

        diceRb.transform.rotation = Quaternion.identity;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block") && diceState == DiceState.throwState)
        {
            CamShake.instance.ShakeCamera(camShakeAmp, camShakeDuration);
            Blocks block = collision.gameObject.GetComponent<Blocks>();
            int temp = diceNumber;
            diceNumber -= block.GetHP();
            block.Damage(temp);
            diceNumberText.text = Convert.ToString(diceNumber);
        }
        if (collision.gameObject.CompareTag("Bounds"))
        {
            //print($"Before Transform : {transform.up}");
            dir = Utilty.ReflectVector(dir, collision);
            transform.up = dir.normalized;
            //print($"After Transform : {transform.up}");

            CamShake.instance.ShakeCamera(camShakeAmp, camShakeDuration);
        }
        if (diceNumber <= 0)
        {
            diceState = DiceState.destroyState;
            objectPool.Release(this);
            ResetDice();
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.up, Color.green);
        Debug.DrawRay(transform.position, -dir, Color.green);
    }
}
