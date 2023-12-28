using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    bool isFusion;
    CharacterControler virus1Pos, virus2Pos;
    Vector3 magnetPos;

    private void Start()
    {
        magnetPos = transform.position;
        MoveVirusestoMagnet();
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        
        if (isFusion && virus1Pos != null && virus2Pos != null)
        {
            if ((virus1Pos.transform.position - magnetPos).magnitude <= 0.3f)
            {
                virus1Pos.rb.velocity = Vector2.zero;
                virus2Pos.rb.velocity = Vector2.zero;

                Invoke("FusionVirusMagnet", .5f);
                                               
            }
           
        }
        

    }
    int GetLowerLevel()
    {
        int lowerLevel = 1;
        int lowerLevelAmount = 0;

        for (int j = 0; j < VirusManager.instance.virusData[(VirusManager.instance.virusData.Count) - 1].VirusLevel; j++) //Numero de ejecuciones = Nivel maximo
        {
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
                    
                    return lowerLevel;
                }

            }

            lowerLevel++;
            lowerLevelAmount = 0;
        }                

        return 0;
    }

    GameObject[] GetFusionVirus()
    {
        if (GetLowerLevel() != 0)
        {
            GameObject[] fusionVirus = new GameObject[2];
            int arrayPosition = 0;

            //Recorrer para agregar 2 al array fusionVirus
            for (int i = 0; i < VirusManager.instance.transform.childCount; i++)
            {
                if (GetLowerLevel() == VirusManager.instance.transform.GetChild(i).GetComponent<CharacterControler>().virusData.VirusLevel)
                {
                    fusionVirus[arrayPosition] = VirusManager.instance.transform.GetChild(i).gameObject;
                    arrayPosition++;

                    if (fusionVirus[1] != null)
                    {
                        return fusionVirus;
                    }
                }
            }
        }
        return null;      

    }

    void MoveVirusestoMagnet()
    {
        isFusion = true;
        Vector2 virus1Distance;
        Vector2 virus2Distance;
        GameObject[] virusArray = GetFusionVirus();
        
        CharacterControler virus1 = virusArray[0].GetComponent<CharacterControler>();
        CharacterControler virus2 = virusArray[1].GetComponent<CharacterControler>();

        virus1Pos = virus1;
        virus2Pos = virus2;


        virus1.canMove = false;
        virus2.canMove = false;

        virus1.rb.velocity = Vector2.zero;
        virus2.rb.velocity = Vector2.zero;

        virus1Distance = transform.position - virus1.transform.position;
        virus2Distance = transform.position - virus2.transform.position;

        virus1.circleCollider.isTrigger = true;
        virus2.circleCollider.isTrigger = true;

        virus1.rb.velocity = virus1Distance;
        virus2.rb.velocity = virus2Distance;
                        
    }



    void FusionVirusMagnet()
    {
        isFusion = false;

        if (virus2Pos != null && virus2Pos.gameObject != null)
        {
            Destroy(virus2Pos.gameObject);
        }

        if (virus1Pos != null && virus1Pos.gameObject != null)
        {
            virus1Pos.virusData = VirusManager.instance.NextVirus(virus1Pos.virusData);
            virus1Pos.UpdateVirusData();

            virus1Pos.canMove = true;
            virus1Pos.circleCollider.isTrigger = false;

            virus1Pos = null;
            virus2Pos = null;
        }
        Destroy(gameObject);

    }

}
