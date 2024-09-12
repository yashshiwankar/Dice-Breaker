using UnityEngine;
using UnityEngine.Pool;
using DiceBreakerUtility;


public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    bool gameOver = false;

    private float screenHalfWidth, screenHalfHeight,linePosX, linePosY;

    [SerializeField]
    PhysicsMaterial2D bounce;

//---Blocks---
    [SerializeField] Blocks block;
    GameObject blockSpawnPoint;

    [Tooltip("Spawn Block Parameters")]
    [SerializeField] private float startSpawnTime = 3f, spawnRate = 1.5f, spawnOffset = 0.75f; //default values

    [SerializeField] private int defaultSize = 25, maxSize = 100;
    [SerializeField] private int defaultSize_DicePool = 10, maxSize_DicePool = 15;

//---Object Pools---
    private ObjectPool<Blocks> _pool;
    public ObjectPool<Blocks> _Objpool 
    {
        get
        {
            if(_pool == null)
            {
                _pool = new ObjectPool<Blocks>(CreateBlockPool, _block =>
                {
                    _block.gameObject.SetActive(true);
                }, _block =>
                {
                    _block.gameObject.SetActive(false);
                }, _block =>
                {
                    Destroy(_block.gameObject);
                }, false, defaultSize, maxSize);
            }
            return _pool;
        }
    }

    private ObjectPool<DiceScript> _dicePool;
    public ObjectPool<DiceScript> GetDiceObjPool
    {
        get
        {
            if (_dicePool == null)
            {
                //functoCreate, setactive, setdeactive, destory, collectionCheck,defaultsize,maxsize
                _dicePool = new ObjectPool<DiceScript>(CreateDicePool,
                    _dicePool =>
                    {
                        _dicePool.gameObject.SetActive(true);
                    },
                    _dicePool =>
                    {
                        _dicePool.gameObject.SetActive(false);
                    },
                    _dicePool =>
                    {
                        Destroy(_dicePool.gameObject);
                    },
                    false, defaultSize_DicePool, maxSize_DicePool);
            }
            return _dicePool;
        }
    }

//---Line renderer
    Vector3[] linePosArray;
    [SerializeField] GameObject lineObj;
    [SerializeField] float lineY_Ratio, lineX_Offset;

    //---DICE---
    [SerializeField] DiceScript dice;
    DiceScript currentDice;
    GameObject spawnPoint;
    [SerializeField] float DiceSpawnOffset, diceSpawnRate = 0.75f;

    GameObject[] bounds;

    public readonly string
        DICE_TAG = "Dice",
        DICE_DETECTION_TAG = "DiceDetection",
        BOUND_TAG = "Bounds",
        UPPER_BOUND = "Upper Bound",
        LOWER_BOUND = "Lower Bound",
        LEFT_BOUND = "Left Bound",
        RIGHT_BOUND = "Right Bound";

    [SerializeField]
    private GameObject gameOverUI;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        screenHalfWidth = Utilty.GetScreenHalfWidth();
        screenHalfHeight = Utilty.GetScreenHalfHeight();

        SpawnBlockPoint();
        SpawnDicePoint();
        CreateLine();
        CreateBoundary();

        if (dice == null)
        {
            dice = GameObject.FindGameObjectWithTag(DICE_TAG).GetComponent<DiceScript>();
        }
        else
        {
            SpawnDice();
        }

        HealthManager.Instance.OnGameOver += GameOverHandler;
    }

    private void GameOverHandler()
    {
        gameOver = true;
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    private void Start()
    {
        InvokeRepeating("SpawnBlock", startSpawnTime, spawnRate);
    }
    private void Update()
    {
        if (currentDice != null)
        {
            if (currentDice.diceState == DiceState.throwState || currentDice.diceState == DiceState.destroyState)
            {
                currentDice = null;
                Invoke("SpawnDice", diceSpawnRate);
            }
        }
    }
    void SpawnBlockPoint()
    {
        blockSpawnPoint = new GameObject("Block Spawn Point");
        blockSpawnPoint.transform.position = new Vector3(0f, screenHalfHeight + spawnOffset, 0f);
    }
    Blocks CreateBlockPool()
    {
        //1.225f is max block size
        Blocks blockInstance = Instantiate(block,new Vector3(Random.Range(-screenHalfWidth+ 1f, screenHalfWidth - 1f), blockSpawnPoint.transform.position.y, 0f), Quaternion.Euler(0f,0f,Random.Range(-15f,15f)));
        blockInstance.objectPool = _pool;
        return blockInstance;
    }
    void SpawnDicePoint()
    {
        spawnPoint = new GameObject("Dice Spawn Point");
        spawnPoint.transform.position = new Vector3(0f, -((screenHalfHeight * 2) / lineY_Ratio) + DiceSpawnOffset, 0f);
    }

    DiceScript CreateDicePool()
    {
        DiceScript diceScript = Instantiate(dice, spawnPoint.transform.position, Quaternion.identity);
        diceScript.gameObject.tag = DICE_TAG;
        diceScript.DiceRoll();
        diceScript.objectPool = _dicePool;
        return diceScript;
    }
    void SpawnDice()
    {
        DiceScript diceScriptInstance = GetDiceObjPool.Get();
        if(currentDice == null)
        {
            currentDice = diceScriptInstance;
        }
    }
    void SpawnBlock()
    {
        Blocks block = _Objpool.Get();
    }
    void CreateBoundary()
    {
        bounds = new GameObject[4];
        bounds[0] = new GameObject(UPPER_BOUND);
        bounds[1] = new GameObject(LOWER_BOUND);
        bounds[2] = new GameObject(LEFT_BOUND);
        bounds[3] = new GameObject(RIGHT_BOUND);

        foreach (var bound in bounds)
        {
            bound.AddComponent<BoxCollider2D>();
            bound.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            bound.GetComponent<BoxCollider2D>().sharedMaterial = bounce;
            bound.tag = BOUND_TAG;
            
            if (bound.name == UPPER_BOUND)
            {
                bound.GetComponent<BoxCollider2D>().size = new Vector2(screenHalfWidth * 2f, 1f);
                bound.transform.position = new Vector3(0f, screenHalfHeight + 0.5f, 0f);
                bound.SetActive(false);
            }

            else if (bound.name == LOWER_BOUND)
            {
                bound.GetComponent<BoxCollider2D>().size = new Vector2(screenHalfWidth * 2f, 1f);
                bound.transform.position = new Vector3(0f, (screenHalfHeight + 0.5f) * -1f, 0f);
            }

            else if (bound.name == RIGHT_BOUND)
            {
                bound.GetComponent<BoxCollider2D>().size = new Vector2(1f, screenHalfHeight * 2f);
                bound.transform.position = new Vector3((screenHalfWidth + 0.5f), 0f, 0f);
            }

            else if (bound.name == LEFT_BOUND)
            {
                bound.GetComponent<BoxCollider2D>().size = new Vector2(1f, screenHalfHeight * 2f);
                bound.transform.position = new Vector3((screenHalfWidth + 0.5f) * -1f, 0f, 0f);
            }
        }
    }

    void CreateLine()
    {
        linePosY = (screenHalfHeight * 2) / lineY_Ratio;
        linePosX = screenHalfWidth + lineX_Offset;
        linePosArray = new Vector3[2];
        linePosArray[0] = new Vector3(-linePosX, -linePosY, 0);
        linePosArray[1] = new Vector3(linePosX, -linePosY, 0);
        GameObject lineInstance = Instantiate(lineObj, Vector3.zero, Quaternion.identity);
        LineRenderer line = lineInstance.GetComponent<LineRenderer>();
        line.SetPositions(linePosArray);
    }

    public float GetLinePosY()
    {
        return linePosY;
    }
}
