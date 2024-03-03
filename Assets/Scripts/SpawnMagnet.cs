using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMagnet : MonoBehaviour
{
    bool canCreateMagnet = true;
    [SerializeField] GameObject MagnetSpawner;
    [SerializeField] float cooldown;
    [SerializeField] UpgradeData Magnet;

    private void Awake()
    {
        Magnet.Level = 0;
    }

    private void Update()
    {
        cooldown = Magnet.CurrentEffect;
        if (canCreateMagnet && CanSpawnMagnet())
        {
            StartCoroutine(CreateMagnet());
        }


    }
    IEnumerator CreateMagnet()
    {
        canCreateMagnet = false;
        GameObject magnetInstance = Instantiate(MagnetSpawner, Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f)), Quaternion.identity, transform.parent);
        magnetInstance.transform.position = new Vector3(magnetInstance.transform.position.x, magnetInstance.transform.position.y, 0f);
        Debug.Log("Spawning Magnet");
        yield return new WaitForSeconds(cooldown - (Magnet.MultiplierEffect*Magnet.Level));
        canCreateMagnet = true;
    }


    //Copy paste de Magnet para obtener 2 del menor nivel
    bool CanSpawnMagnet()
    {
        int lowerLevel = 1;
        int lowerLevelAmount = 0;
                
        for (int j = 0; j < VirusManager.instance.virusData[(VirusManager.instance.virusData.Count) - 1].VirusLevel; j++) //Numero de ejecuciones = Nivel maximo
        {
            if (lowerLevel == GameManager.instance.maxVirusLevel)
            {
                return false;
            }
            //Obtener lowerlevel
            for (int i = 0; i < VirusManager.instance.transform.childCount; i++)
            {
                int virusLevel = VirusManager.instance.transform.GetChild(i).GetComponent<CharacterControler>().virusData.VirusLevel;
                
                if (virusLevel == lowerLevel)
                {
                    lowerLevelAmount++;
                }
                if (lowerLevelAmount == 2)
                {

                    return true;
                }

            }
            lowerLevel++;
            lowerLevelAmount = 0;
        }

        return false;
    }

    

}
