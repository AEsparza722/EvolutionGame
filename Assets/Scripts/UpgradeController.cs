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

    [Header("Upgrades")]
    [SerializeField] SpawnMagnet Magnet;

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
            LeanTween.moveY(upgradeMenu.GetComponent<RectTransform>(), 1007, .5f);
            isMenuActive = false;
        }
        else
        {
            //upgradeMenu.SetActive(true);
            LeanTween.moveY(upgradeMenu.GetComponent<RectTransform>(), 107.9353f, .5f);
            isMenuActive = true;
        }
        
    }

    void AddUpgradesToMenu()
    {
        foreach (UpgradeData upgradeItem in upgradeList)
        {
            //Instanciar upgrades y actualizar valores con el scriptable object
            GameObject upgradeInstance = Instantiate(upgradePrefab, container.transform);
            upgradeInstance.name = upgradeItem.name;
            upgradeInstance.transform.GetChild(0).GetComponent<Image>().sprite = upgradeItem.Icon;
            upgradeInstance.transform.GetChild(1).GetComponent<TMP_Text>().text = upgradeItem.Name;
            upgradeInstance.transform.GetChild(2).GetComponent<TMP_Text>().text = upgradeItem.Description;
            upgradeInstance.transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = upgradeItem.Cost.ToString();
            
            //Agregar casos por cada upgrade por nombre
            switch (upgradeItem.Name)
            {
                case "Magnet":
                    upgradeInstance.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => UpgradeMagnet(upgradeItem));
                    upgradeInstance.transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = (upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level)).ToString();
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
                    container.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = (upgradeList[j].Cost + (upgradeList[j].Cost * upgradeList[j].Level)).ToString();
                }
            }
        }
    }

    #region Upgrades
    //Magnet
    
    void UpgradeMagnet(UpgradeData upgradeItem)
    {
        float MagnetCost = upgradeItem.Cost + (upgradeItem.Cost*upgradeItem.Level);
        Debug.Log(MagnetCost);
        //upgradeItem.Cost + (upgradeItem.Cost * upgradeItem.Level);


        if (GameManager.instance.coins >= MagnetCost)
        {
            Magnet.enabled = true;
            upgradeItem.Level++;
            GameManager.instance.coins -= MagnetCost;

        }
        else
        {
            Debug.Log("No te alcanza");
        }
        
    }


    #endregion
}
