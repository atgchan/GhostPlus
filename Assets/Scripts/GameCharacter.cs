using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameCharacter : MonoBehaviour {

    public float moveTime = 3.0f;
    public float moveSpeed = 1.0f;
    
    float deltaMoveTime = 0.0f;

    Vector3 oldVec;
    Vector3 newVec;

    float randomX;
    float randomY;

    Vector3 curPos;

    bool isDragging = false;

    public GameManager gameManager;

    public int charLevel = 0;

    public int income = 0;
    public float deltaIncomeTime = 0.0f;

    public SpriteRenderer spriteRenderer;

    public Text description;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnEnable()
    {
        deltaMoveTime = Time.time;
        deltaIncomeTime = Time.time;

        oldVec = gameObject.transform.position;
        newVec = gameObject.transform.position;
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseDrag()
    {
        curPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        curPos.z = 0.0f;

        if (curPos.x >= GameManager.widthSize || curPos.x <= GameManager.widthSize * -1)
            return;

        if (curPos.y >= GameManager.heightSize + 0.5f || curPos.y <= GameManager.heightSize * -1)
            return;

        gameObject.transform.position = curPos;
    }

    void OnMouseUp()
    {
        isDragging = false;
        oldVec = gameObject.transform.position;
        newVec = gameObject.transform.position;

        GameObject targetCharacter = gameManager.GetTargetCharacter(gameObject);
        if (targetCharacter == null) return;

        targetCharacter.tag = "NotUsing";
        targetCharacter.SetActive(false);

        gameManager.RunSpawnEffect(gameObject.transform.position);
        Upgrade();                
    }

    public void Upgrade()
    {
        ++charLevel;
        income = gameManager.ghostData[charLevel]._earnMoney;
        SetSprite();
        gameManager.MinusCurNumber();
        gameManager.PopupNewGhostIfNew(charLevel);
    }

    public void SetSprite()
    {
        spriteRenderer.sprite = Resources.LoadAll<Sprite>("cats2")[charLevel];
    }

    void Update()
    {
        if (isDragging == true)
            return;

        // 초마다 움직임
        if (Time.time - deltaMoveTime > moveTime)
        {
            oldVec = gameObject.transform.position;
            newVec = gameObject.transform.position;
            GetRandomPosition();
        }

        // 초마다 먹이먹음
        if (Time.time - deltaIncomeTime > gameManager.incomeSpeed)
        {
            deltaIncomeTime = Time.time;
            IncomeProcess();
        }

        gameObject.transform.position = Vector3.Lerp(oldVec, newVec, Time.time - deltaMoveTime);
    }

    void IncomeProcess()
    {
        gameManager.DisplayIncome(income, gameObject.transform.position, spriteRenderer.bounds.size.y * 100);
        gameManager.AddIncome(income);
    }

    void GetRandomPosition()
    {
        deltaMoveTime = Time.time;

        randomX = Random.Range(-moveSpeed, moveSpeed);
        randomY = Random.Range(-moveSpeed, moveSpeed);
        
        if(GameManager.widthSize <= newVec.x + randomX || GameManager.widthSize * -1 > newVec.x + randomX)
            randomX *= -1;

        if (GameManager.heightSize <= newVec.y + randomY || GameManager.heightSize * -1 > newVec.y + randomY)
            randomY *= -1;

        newVec.x += randomX;
        newVec.y += randomY;
    }
}
