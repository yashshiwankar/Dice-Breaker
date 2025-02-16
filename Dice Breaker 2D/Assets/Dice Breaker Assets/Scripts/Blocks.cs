using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using DiceBreakerUtility;

public class Blocks : MonoBehaviour
{
    [SerializeField] private float blockForce = 125f, offset = 1f;
    [SerializeField] private int hitPoints, minHP = 1, maxHP = 12; // default

    [SerializeField]
    private float scaleStartX = 0.5f, maxSizeIncrementer = 0.075f;
    
    Rigidbody2D rb;

    public ObjectPool<Blocks> objectPool;

    private Vector3 pos;
    public int GetHP() { return hitPoints; }

    private GameObject[] blockNumArray = new GameObject[12];
    private const string BLOCK_NUM_TAG = "Enemy Dice Number";
    PolygonCollider2D polygonCollider2D;
    BoxCollider2D boxCollider2D;

    private void Awake()
    {      

        rb = GetComponent<Rigidbody2D>();

        polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        polygonCollider2D.enabled = false;

        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;

        pos = transform.position;
        for (int i = 0; i < transform.childCount; i++) 
        {
            if (transform.GetChild(i).CompareTag(BLOCK_NUM_TAG))
            {
                blockNumArray[i] = transform.GetChild(i).gameObject;
                blockNumArray[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        Move();

        if(transform.position.y < (-GameManager.instance.GetLinePosY() + (gameObject.transform.localScale.y/2)))
        {
            HealthManager.Instance.SubtractCurrentHealth(1);
            objectPool.Release(this);
        }
    }
    public void Move()
    {
        transform.Translate(-gameObject.transform.up * blockForce/10 * Time.deltaTime, Space.Self);
    }
    private void OnEnable()
    {
        hitPoints = Random.Range(minHP, maxHP);
        // pick ui
        foreach (var block in blockNumArray)
        {
            if (block.name == hitPoints.ToString())
            {
                block.SetActive(true);
                if (hitPoints > 9)
                {
                    polygonCollider2D.enabled = true;
                }
                else
                {
                    boxCollider2D.enabled = true;
                }
            }
        }
        //hpText.text = hitPoints.ToString();
        int i = 1;
        float newScaleX = scaleStartX;
        while (i < hitPoints)
        {
            newScaleX += maxSizeIncrementer;
            i += 1;
        }
        transform.localScale = new Vector3(1f, 1f, 0f) * newScaleX;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
    }

    private void ChangeBlocKVisual(int prevHitPt)
    {        
        blockNumArray[prevHitPt - 1].SetActive(false);
        if(hitPoints < 10)
        {
            polygonCollider2D.enabled = false;
            boxCollider2D.enabled = true;
        }
        if(hitPoints > 0)
        {
            blockNumArray[hitPoints - 1].SetActive(true);
        }
    }

    public void OnDisable()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = pos;
        foreach(var block in blockNumArray)
        {
            block.SetActive(false);
        }
        boxCollider2D.enabled = false;
        polygonCollider2D.enabled = false;

        gameObject.SetActive(false);
    }
    public void Damage(int damage)
    {
        int temp = hitPoints;
        hitPoints -= damage;
        ChangeBlocKVisual(temp);
        //hpText.text = hitPoints.ToString();
        if(hitPoints <= 0)
        {
            ScoreScript.Instance.SetScore(1);
            objectPool.Release(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bounds"))
        {
            transform.up = Utilty.ReflectVector(-transform.up, collision);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(this.transform.position, -transform.up, Color.blue);
    }
}
