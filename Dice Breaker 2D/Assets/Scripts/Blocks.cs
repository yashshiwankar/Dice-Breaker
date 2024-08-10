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
    
    [SerializeField] private TextMeshProUGUI hpText;
    Rigidbody2D rb;

    public ObjectPool<Blocks> objectPool;

    private Vector3 pos;
    public int GetHP() { return hitPoints; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pos = transform.position;
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
        hpText.text = hitPoints.ToString();
        int i = 1;
        float newScaleX = scaleStartX;
        while (i < hitPoints)
        {
            newScaleX += maxSizeIncrementer;
            i += 1;
        }
        transform.localScale = new Vector3(1f,1f,0f) * newScaleX;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
    }
    public void OnDisable()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = pos;
        gameObject.SetActive(false);
    }
    public void Damage(int damage)
    {
        hitPoints -= damage;
        hpText.text = hitPoints.ToString();
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
