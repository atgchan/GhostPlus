using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour {

    GameManager gameManager = null;

    // Upgrade Candy Quality
    public List<int> candyPriceTable;
    int maxCandyLevel = 100;
    float candyPriceIncrease = 8.0f;
    public GameObject candyQualityList;
    Text candyCurrent;
    Text candyNext;
    Text candyPrice;

    // Auto Plus
    public List<int> autoPlusPriceTable;
    int autoPlusIncrease = 20;
    public GameObject autoPlusList;
    Text autoPlusCurrent;
    Text autoPlusNext;
    Text autoPlusPrice;
    public GameObject black_cat = null;

    // Auto Food
    public List<int> autoFoodPriceTable;
    int autoFoodIncrease = 10;
    public GameObject autoFoodList;
    Text autoFoodCurrent;
    Text autoFoodNext;
    Text autoFoodPrice;
    public GameObject auto_food = null;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        InitTableAndText();
    }

    void InitTableAndText()
    {
        candyPriceTable = new List<int>();
        int startValue = 100;
        candyPriceTable.Add(startValue);
        for (int i = 1; i < maxCandyLevel; ++i)
        {
            candyPriceTable.Add((int)(candyPriceTable[i - 1] * candyPriceIncrease));
        }

        candyCurrent = candyQualityList.transform.FindChild("Current").GetComponent<Text>();
        candyNext = candyQualityList.transform.FindChild("Next").GetComponent<Text>();
        candyPrice = candyQualityList.transform.FindChild("Price").GetComponent<Text>();

        // auto plus init
        autoPlusPriceTable = new List<int>();
        autoPlusPriceTable.Add(100);
        for (int i = 1; i < gameManager.maxPlusLevel; ++i)
        {
            autoPlusPriceTable.Add((int)(autoPlusPriceTable[i - 1] + autoPlusIncrease));
        }

        autoPlusCurrent = autoPlusList.transform.FindChild("Current").GetComponent<Text>();
        autoPlusNext = autoPlusList.transform.FindChild("Next").GetComponent<Text>();
        autoPlusPrice = autoPlusList.transform.FindChild("Price").GetComponent<Text>();

        // auto food init
        autoFoodPriceTable = new List<int>();
        autoFoodPriceTable.Add(100);
        for (int i = 1; i < gameManager.maxAuoFoodLevel; ++i)
        {
            autoFoodPriceTable.Add((int)(autoFoodPriceTable[i - 1] + autoFoodIncrease));
        }

        autoFoodCurrent = autoFoodList.transform.FindChild("Current").GetComponent<Text>();
        autoFoodNext = autoFoodList.transform.FindChild("Next").GetComponent<Text>();
        autoFoodPrice = autoFoodList.transform.FindChild("Price").GetComponent<Text>();
    }

    public void ChangeActive()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void MaxCharNumberUpgrade()
    {
        gameManager.MaxCharNumberUpgrade();
    }

    public void IncomeSpeedUpgrade()
    {
        gameManager.IncomeSpeedUpgrade();
    }

    public void MaxFoodCapUpgrade()
    {
        gameManager.MaxFoodCapUpgrade();
    }

    public void FoodGenSpeedUpgrade()
    {
        gameManager.FoodGenSpeedUpgrade();
    }

    public void FoodUpgrade()
    {
        if(GameManager.currentMoney < candyPriceTable[gameManager.maxSpawnLevel])
        {
            Debug.Log("캔디 살 돈이 부족함!");
            return;
        }

        GameManager.currentMoney -= candyPriceTable[gameManager.maxSpawnLevel];
        gameManager.FoodUpgrade();
        UpgradeCandyQualityListInfo();
    }

    void UpgradeCandyQualityListInfo()
    {
        candyCurrent.text = gameManager.maxSpawnLevel.ToString();
        candyNext.text = (gameManager.maxSpawnLevel + 1).ToString();
        candyPrice.text = candyPriceTable[gameManager.maxSpawnLevel].ToString();
    }

    public void AutoPlus()
    {
        if (GameManager.currentDiamond < autoPlusPriceTable[gameManager.autoPlusLevel])
        {
            Debug.Log("자동 합치기 살 돈이 부족함!");
            return;
        }

        if(!black_cat.activeInHierarchy)
        {
            black_cat.SetActive(true);
        }
                
        gameManager.AddDiamonds(autoPlusPriceTable[gameManager.autoPlusLevel] * -1);
        gameManager.AutoPlusUpgrade();

        autoPlusCurrent.text = gameManager.GetAutoPlusTime(gameManager.autoPlusLevel).ToString() + 's';
        autoPlusNext.text = gameManager.GetAutoPlusTime(gameManager.autoPlusLevel + 1).ToString() + 's';
        autoPlusPrice.text = autoPlusPriceTable[gameManager.autoPlusLevel].ToString();
    }

    public void AutoFood()
    {
        if (GameManager.currentDiamond < autoFoodPriceTable[gameManager.autoFoodLevel])
        {
            Debug.Log("자동 먹이주기 살 돈이 부족함!");
            return;
        }

        if (!auto_food.activeInHierarchy)
        {
            auto_food.SetActive(true);
        }

        gameManager.AddDiamonds(autoFoodPriceTable[gameManager.autoFoodLevel] * -1);
        gameManager.AutoFoodUpgrade();

        autoFoodCurrent.text = gameManager.GetAutoFoodTime(gameManager.autoFoodLevel).ToString() + 's';
        autoFoodNext.text = gameManager.GetAutoFoodTime(gameManager.autoFoodLevel + 1).ToString() + 's';
        autoFoodPrice.text = autoFoodPriceTable[gameManager.autoFoodLevel].ToString();
    }
}
