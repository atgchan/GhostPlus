using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public class GameManager : MonoBehaviour {

    int maxLevel = 0; // 게임 내 최대 유령 레벨
    public string fileName = "data";
    public struct GhostData
    {
        public int _level;
        public string _name;
        public int _earnMoney;
        public string _description;
        public bool _isActive;
    }
    public List<GhostData> ghostData; // 유령 정보를 저장하고 있는 리스트

    // 유령 정보를 보여주기 위한 변수
    public GameObject ghostInfoPannel;
    public GameObject newGhostEffect;
    GameObject tmpEffect;

    public int maxSpawnLevel = 0;
    public int charPoolSize = 100;
    public List<GameObject> charPool;
    public List<GameObject> candyPool;

    public GameObject character = null;
    public GameObject candy = null;
    public GameObject smoke = null;

    public static float widthSize = 2.7f;
    public static float heightSize = 3.3f;

    public int currentFood = 10;
    public int maxFood = 10;

    public float deltaFoodRecoverTime = 0.0f;
    public float foodRecoverTime = 6.0f;
    public Text foodText = null;
    bool isFoodLack = true;

    public int curCharNumber = 0;
    public int maxCharCap = 10;

    public Text curCharText = null;

    public static double currentMoney = 0.0d;
    public static int currentDiamond = 0;

    public float incomeSpeed = 4.0f;
    
    public GameObject incomeText = null;
    GameObject panel = null;

    public float incomeTextTime = 1.0f;
    public float incomeTextSpeed = 2.0f;

    StringBuilder sbForFood;
    StringBuilder sbForChar;
    
    public Text diamondText = null;
    public Text moneyText = null;

    // auto plus variables
    public List<float> autoPlusTimeTable;
    public int autoPlusLevel = 0;
    public int maxPlusLevel = 25;
    float deltaAutoPlusTime = 0.0f;
    bool isAutoPlusAvailable = false;

    // auto food variables
    public List<float> autoFoodTimeTable;
    public int autoFoodLevel = 0;
    public int maxAuoFoodLevel = 25;
    float deltaAutoFoodTime = 0.0f;
    bool isAutoFoodAvailable = false;

    void Start()
    {
        panel = GameObject.Find("Income");

        sbForFood = new StringBuilder("(10/10)");
        sbForChar = new StringBuilder("0 / 10");

        LoadGhostData();
        LoadCharacters();
        LoadCandies();
        InitializePriceTables();
    }

    void Update()
    {
        // food recovery loop
        isFoodLack = true;
        if(currentFood >= maxFood)
        {
            deltaFoodRecoverTime = Time.time;
            isFoodLack = false;
        }

        if (isFoodLack && Time.time - deltaFoodRecoverTime > foodRecoverTime)
        {
            deltaFoodRecoverTime = Time.time;
            if(currentFood < maxFood)
            {
                PlusFood();
            }
        }

        // auto plus loop
        isAutoPlusAvailable = true;
        if(autoPlusLevel <= 0)
        {
            isAutoPlusAvailable = false;
            deltaAutoPlusTime = Time.time;
        }

        if(isAutoPlusAvailable && Time.time - deltaAutoPlusTime > autoPlusTimeTable[autoPlusLevel])
        {
            AutoPlus();
            deltaAutoPlusTime = Time.time;
        }

        // auto food loop
        isAutoFoodAvailable = true;
        if (autoFoodLevel <= 0)
        {
            isAutoFoodAvailable = false;
            deltaAutoFoodTime = Time.time;
        }

        if (isAutoFoodAvailable && Time.time - deltaAutoFoodTime > autoFoodTimeTable[autoFoodLevel])
        {
            SpawnCharacter();
            deltaAutoFoodTime = Time.time;
        }
    }

    public void PopupNewGhostIfNew(int level)
    {
        if(ghostData[level]._isActive == true)
        {
            return;
        }

        GhostData nowActive;
        nowActive._level = ghostData[level]._level;
        nowActive._name = ghostData[level]._name;
        nowActive._description = ghostData[level]._description;
        nowActive._earnMoney = ghostData[level]._earnMoney;
        nowActive._isActive = true;

        ghostData[level] = nowActive;
        
        DisplayGhostInfo(ghostData[level]);
    }

    void DisplayGhostInfo(GhostData data)
    {
        ghostInfoPannel.SetActive(true);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0}. {1}", data._level, data._name);
        ghostInfoPannel.transform.FindChild("Name").GetComponent<Text>().text = sb.ToString();

        ghostInfoPannel.transform.FindChild("Description").GetComponent<Text>().text = data._description;

        sb.Remove(0, sb.Length);
        sb.AppendFormat("Income : {0}", data._earnMoney);
        ghostInfoPannel.transform.FindChild("Income").GetComponent<Text>().text = sb.ToString();

        ghostInfoPannel.transform.FindChild("Ghost").GetComponent<Image>().overrideSprite = Resources.LoadAll<Sprite>("cats2")[data._level - 1];

        GameObject effect = Instantiate(newGhostEffect) as GameObject;
        effect.transform.position = new Vector3(0.0f, 1.1f, 0.0f);
        effect.GetComponent<Renderer>().sortingLayerName = "Particle";

        tmpEffect = effect;
    }

    public void DisableGhostInfo()
    {
        ghostInfoPannel.SetActive(false);
        Destroy(tmpEffect);
    }

    void LoadGhostData()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("Xml/" + fileName);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        XmlNodeList nodes = xmlDoc.SelectNodes("GhostSet/Ghost");

        ghostData = new List<GhostData>();

        foreach(XmlNode node in nodes)
        {
            GhostData ghost = new GhostData();
            ghost._level =System.Int32.Parse(node.SelectSingleNode("Level").InnerText);
            ghost._name = node.SelectSingleNode("Name").InnerText;
            ghost._earnMoney =System.Int32.Parse(node.SelectSingleNode("EarnMoney").InnerText);
            ghost._description = node.SelectSingleNode("Description").InnerText;
            ghost._isActive = false;
            ghostData.Add(ghost);
        }
    }

    void LoadCharacters()
    {
        charPool = new List<GameObject>();
        for(int i=0; i<charPoolSize; ++i)
        {
            GameObject obj = (GameObject)Instantiate(character);
            obj.SetActive(false);
            obj.GetComponent<SpriteRenderer>().sortingLayerName = "character";
            charPool.Add(obj);
        }
    }

    void LoadCandies()
    {
        candyPool = new List<GameObject>();
        for (int i = 0; i < charPoolSize; ++i)
        {
            GameObject obj = (GameObject)Instantiate(candy);
            obj.SetActive(false);
            obj.GetComponent<SpriteRenderer>().sortingLayerName = "character";
            candyPool.Add(obj);
        }
    }

    void InitializePriceTables()
    {
        // auto plus init
        autoPlusTimeTable = new List<float>();
        autoPlusTimeTable.Add(1000.0f);
        autoPlusTimeTable.Add(60.0f);
        for (int i=2; i<maxPlusLevel; ++i)
        {
            autoPlusTimeTable.Add(autoPlusTimeTable[i - 1] - 2.0f);
        }

        // auto food init
        autoFoodTimeTable = new List<float>();
        autoFoodTimeTable.Add(1000.0f);
        autoFoodTimeTable.Add(30.0f);
        for (int i = 2; i < maxAuoFoodLevel; ++i)
        {
            autoFoodTimeTable.Add(autoFoodTimeTable[i - 1] - 1.0f);
        }
    }

    public void SpawnCharacter()
    {
        if (currentFood < 1) return;
        if (curCharNumber >= maxCharCap) return;

        GameObject nextCharacter = GetAvailableCharFromPool();
        GameObject nextCandy = GetAvailableCandyFromPool();

        if (nextCharacter == null || nextCandy == null)
        {
            Debug.Log("error : No Pool available");
            return;
        }

        GameCharacter nextCharScript = nextCharacter.GetComponent<GameCharacter>();

        int randomLevel = Random.Range(0, maxSpawnLevel + 1);
        float randomX = Random.Range(-widthSize, widthSize);
        float randomY = Random.Range(-heightSize, heightSize);

        Vector3 vec = new Vector3(randomX, randomY, 0.0f);

        MinusFood();
        PlusCurNumber();

        nextCandy.transform.position = vec;
        nextCharacter.transform.position = vec;

        nextCharScript.income = ghostData[randomLevel]._earnMoney;
        nextCharScript.charLevel = randomLevel;
        nextCharScript.SetSprite();

        object[] parms = new object[4] { nextCandy, nextCharacter, vec, randomLevel };
        
        StartCoroutine("SpawnCandyAndChange", parms);
    }

    IEnumerator SpawnCandyAndChange(object[] parms)
    {
        GameObject candy = (GameObject)parms[0];
        GameObject character = (GameObject)parms[1];
        Vector3 vec = (Vector3)parms[2];

        candy.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        candy.tag = "NotUsing";
        candy.SetActive(false);
        RunSpawnEffect(vec);
        
        character.SetActive(true);
        PopupNewGhostIfNew((int)parms[3]);

        yield return new WaitForSeconds(0.3f);

    }


    void MinusFood()
    {
        if(currentFood <= 0)
        {
            Debug.Log("Food is lower than 1");
        }

        --currentFood;
        DisplayFood();
    }

    void PlusFood()
    {
        ++currentFood;
        DisplayFood();
    }

    void DisplayFood()
    {
        sbForFood.Remove(0, sbForFood.Length);
        sbForFood.AppendFormat("({0}/{1})", currentFood.ToString(), maxFood.ToString());
        foodText.text = sbForFood.ToString();
    }

    public void MinusCurNumber()
    {
        --curCharNumber;
        DisplayCurNumber();
    }

    void PlusCurNumber()
    {
        ++curCharNumber;
        DisplayCurNumber();
    }

    void DisplayCurNumber()
    {
        sbForChar.Remove(0, sbForChar.Length);
        sbForChar.AppendFormat("{0} / {1}", curCharNumber.ToString(), maxCharCap.ToString());
        curCharText.text = sbForChar.ToString();
    }

    public void MaxCharNumberUpgrade()
    {
        ++maxCharCap;
        DisplayCurNumber();
    }

    public void IncomeSpeedUpgrade()
    {
        incomeSpeed -= 0.2f;
    }

    public void MaxFoodCapUpgrade()
    {
        ++maxFood;
        DisplayFood();
    }

    public void FoodGenSpeedUpgrade()
    {
        foodRecoverTime -= 0.2f;
    }

    public void FoodUpgrade()
    {
        ++maxSpawnLevel;
    }

    public void RunSpawnEffect(Vector3 vec)
    {
        GameObject smokeObj = Instantiate(smoke) as GameObject;
        smokeObj.transform.position = vec;
    }

    GameObject GetAvailableCharFromPool()
    {
        foreach(GameObject obj in charPool)
        {
            if(obj.activeInHierarchy == false && obj.tag == "NotUsing")
            {
                obj.tag = "InUsing";
                return obj;
            }
        }

        return null;
    }

    GameObject GetAvailableCandyFromPool()
    {
        foreach (GameObject obj in candyPool)
        {
            if (obj.activeInHierarchy == false && obj.tag == "NotUsing")
            {
                obj.tag = "InUsing";
                return obj;
            }
        }

        return null;
    }

    string GetCharSpriteNameByNumber(int num)
    {
        return "cats_" + (num-1);
    }

    public GameObject GetTargetCharacter(GameObject movedObj)
    {
        Collider2D[] overlap = Physics2D.OverlapAreaAll(movedObj.GetComponent<BoxCollider2D>().bounds.min, movedObj.GetComponent<BoxCollider2D>().bounds.max);
        
        if(overlap.Length > 1)
        {
            for(int i=0; i<overlap.Length; ++i)
            {
                if (overlap[i].gameObject == movedObj)
                    continue;

                if (overlap[i].gameObject.GetComponent<GameCharacter>().charLevel == movedObj.GetComponent<GameCharacter>().charLevel)
                    return overlap[i].gameObject;
            }
        }

        return null;
    }

    public void DisplayIncome(int income, Vector3 vec, float height)
    {
        StartCoroutine(ShowIncomeNumber(income, vec, height));
    }

    public IEnumerator ShowIncomeNumber(int income, Vector3 vec, float height)
    {
        yield return new WaitForEndOfFrame();

        GameObject obj = (GameObject)Instantiate(incomeText);
        obj.GetComponent<Text>().text = income.ToString();
        obj.GetComponent<Text>().fontSize = 40;
        obj.transform.SetParent(panel.transform);
        obj.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f);

        Vector3 worldPos = Camera.main.WorldToScreenPoint(vec);

        obj.transform.position = new Vector3(worldPos.x, worldPos.y + (height / 2) , 0);
        
        float startTime = Time.time;

        Vector3 newIncomeVec = new Vector3(obj.transform.position.x, obj.transform.position.y + height, 0);

        while (Time.time - startTime < incomeTextTime)
        {
            yield return new WaitForEndOfFrame();
            obj.transform.position = Vector3.Lerp(obj.transform.position, newIncomeVec, Time.deltaTime);
        }

        Destroy(obj);
    }

    public void AddDiamonds(int amount)
    {
        currentDiamond += amount;
        diamondText.text = currentDiamond.ToString();
    }
    
    public void AddIncome(double amount)
    {
        currentMoney += amount;
        moneyText.text = currentMoney.ToString();
    }

    public void AutoPlusUpgrade()
    {
        ++autoPlusLevel;
    }

    public float GetAutoPlusTime(int level)
    {
        return autoPlusTimeTable[level];
    }

    void AutoPlus()
    {
        GameObject fromChar = null;
        GameObject toChar = null;

        for(int i=0; i<charPoolSize; ++i)
        {
            fromChar = charPool[i];
            if(!fromChar.activeInHierarchy)
            {
                continue;
            }

            for(int j=0; j<charPoolSize; ++j)
            {
                toChar = charPool[j];
                if (!toChar.activeInHierarchy)
                {
                    continue;
                }

                if (fromChar == toChar)
                {
                    continue;
                }

                GameCharacter fromCharScript = fromChar.GetComponent<GameCharacter>();
                GameCharacter toCharScript = toChar.GetComponent<GameCharacter>();

                if (fromCharScript.charLevel == toCharScript.charLevel)
                {
                    RunSpawnEffect(fromChar.transform.position);
                    fromChar.GetComponent<GameCharacter>().Upgrade();

                    toChar.tag = "NotUsing";
                    toChar.SetActive(false);
                    return;
                }
            }
        }
    }

    public void AutoFoodUpgrade()
    {
        ++autoFoodLevel;
    }

    public float GetAutoFoodTime(int level)
    {
        return autoFoodTimeTable[level];
    }
}
