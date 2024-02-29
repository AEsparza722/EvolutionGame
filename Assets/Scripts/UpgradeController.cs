using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UpgradeController : MonoBehaviour
{
    public static UpgradeController instance;
    [SerializeField] GameObject upgradeMenu;
    bool isMenuActive;
    [SerializeField] GameObject upgradePrefab;
    [SerializeField] GameObject container;
    [SerializeField] List<UpgradeData> upgradeList;
    public bool canClickDamage = false;

    [Header("Upgrades")]
    [SerializeField] SpawnMagnet Magnet;
    [SerializeField] public UpgradeData ClickDamage;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        AddUpgradesToMenu();
    }

    private void Update()
    {
        if (isMenuActive)
        {
            UpdateUpgrades();
        }
    }

    public void OpenCloseMenu()
    {
        

        if (isMenuActive) 
        {
            //upgradeMenu.SetActive(false);
            LeanTween.moveY(upgradeMenu.GetComponent<RectTransform>(), -1075f, .5f); //initial pos
            isMenuActive = false;
        }
        else
        {
            //upgradeMenu.SetActive(true);
            LeanTween.moveY(upgradeMenu.GetComponent<RectTransform>(), -660f, .5f); //open pos
            isMenuActive = true;
        }
        
    }

    void AddUpgradesToMenu()
    {
        foreach (UpgradeData upgradeItem in upgradeList)
        {
            upgradeItem.Level = 0;
            //Instanciar upgrades y actualizar valores con el scriptable object
            GameObject upgradeInstance = Instantiate(upgradePrefab, container.transform);
            upgradeInstance.name = upgradeItem.name;
            upgradeInstance.transform.GetChild(0).GetComponent<Image>().sprite = upgradeItem.Icon;
            upgradeInstance.transform.GetChild(1).GetComponent<TMP_Text>().text = upgradeItem.Name;
            upgradeInstance.transform.GetChild(2).GetComponent<TMP_Text>().text = upgradeItem.Description;
            //upgradeInstance.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = upgradeItem.Cost.ToString();
            
            //Agregar casos por cada upgrade por nombre
            switch (upgradeItem.Name)
            {
                case "Magnet":
                    upgradeInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => UpgradeMagnet(upgradeItem));
                    upgradeInstance.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = (upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level)).ToString();
                    break;

                case "SpawnMoreViruses":
                    upgradeInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => UpgradeVirusAmount(upgradeItem));
                    upgradeInstance.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = (upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level)).ToString();
                    SpawnMoreUpgrade.instance.amountList[1].chance = upgradeItem.Level * 5f;
                    SpawnMoreUpgrade.instance.amountList[2].chance = upgradeItem.Level * 1.5f;
                    break;
                case "ClickDamage":
                    upgradeInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => UpgradeClickDamage(upgradeItem));
                    upgradeInstance.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = (upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level)).ToString();
                    break;
                case "GeneticUpgrade":
                    upgradeInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => GeneticUpgrade(upgradeItem));
                    upgradeInstance.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = (upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level)).ToString();
                    break;
            }
        }
    }

    void UpdateUpgrades()
    {
        for (int i = 0; i < container.transform.childCount; i++)
        {
            for (int j = 0; j < upgradeList.Count; j++)
            {
                if (container.transform.GetChild(i).name == upgradeList[j].Name)
                {
                    container.transform.GetChild(i).GetChild(4).GetChild(0).GetComponent<TMP_Text>().text = (upgradeList[j].Cost + (upgradeList[j].Cost * upgradeList[j].Level)).ToString();
                    
                }
            }
        }
    }

    #region Upgrades
    //Magnet
    
    void UpgradeMagnet(UpgradeData upgradeItem)
    {
        float MagnetCost = upgradeItem.Cost + (upgradeItem.Cost*upgradeItem.Level);
        //Debug.Log(MagnetCost);
        //upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level);


        if (GameManager.instance.coins >= MagnetCost)
        {
            Magnet.enabled = true;
            upgradeItem.Level++;
            GameManager.instance.UpdateCoins(-MagnetCost);

        }
        else
        {
            Debug.Log("No te alcanza");
        }
        
    }

    //Spawn More
    void UpgradeVirusAmount(UpgradeData upgradeItem)
    {
        float cost = upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level);
        Debug.Log(cost);
        //upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level);


        if (GameManager.instance.coins >= cost)
        {
            upgradeItem.Level++;
            GameManager.instance.UpdateCoins(-cost);
            SpawnMoreUpgrade.instance.amountList[1].chance = upgradeItem.Level * 5f;
            SpawnMoreUpgrade.instance.amountList[2].chance = upgradeItem.Level * 1.5f;
            
        }
        else
        {
            Debug.Log("No te alcanza");
        }
        UpdateUpgrades();
    }

    //Click damage
    void UpgradeClickDamage(UpgradeData upgradeItem)
    {
        float cost = upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level);
        Debug.Log(cost);
        //upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level);


        if (GameManager.instance.coins >= cost)
        {
            upgradeItem.Level++;
            GameManager.instance.UpdateCoins(-cost);
            canClickDamage = true;

        }
        else
        {
            Debug.Log("No te alcanza");
        }
        UpdateUpgrades();
    }

    void GeneticUpgrade(UpgradeData upgradeItem)
    {
        float cost = upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level);
        Debug.Log(cost);


        if (GameManager.instance.coins >= cost)
        {
            upgradeItem.Level++;
            GameManager.instance.maxVirusLevel++;
            GameManager.instance.UpdateCoins(-cost);            

        }
        else
        {
            Debug.Log("No te alcanza");
        }
        UpdateUpgrades();
    }

    #endregion
}
