using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVirus : MonoBehaviour, IIndicator
{
    public Color color { get => indicatorColor; set => indicatorColor = value; }
    public bool isActive { get => indicatorActive; set => indicatorActive = value; }

    [SerializeField] GameObject virusPrefab;
    [SerializeField] float destroySpawnerAfter = 5;
    [SerializeField] Color indicatorColor = Color.white;
    [SerializeField] GameObject indicatorArrowPrefab;
    GameObject indicatorArrow;
    bool indicatorActive = true;

    void Awake()
    {
        indicatorArrow = Instantiate(indicatorArrowPrefab);
        indicatorArrow.GetComponent<SpriteRenderer>().color = indicatorColor;
    }
    void Update()
    {
        Detect();
    }
    private void OnEnable()
    {
        StartCoroutine(SpawnVirusSystem.instance.ReturnToPool(destroySpawnerAfter, gameObject));
        indicatorArrow.SetActive(true);
    }

    private void OnDisable()
    {
        if (indicatorArrow != null) 
        {
            indicatorArrow.SetActive(false);
        }
        
    }

    private void OnMouseDown()
    {
        Spawn();
    }
    void Spawn()
    {
        int times = SpawnMoreUpgrade.instance.GetRandomAmountData().quantity;
        for (int i = 0; i < times; i++)
        {
            Instantiate(virusPrefab, transform.position, Quaternion.identity, VirusManager.instance.transform);
            StartCoroutine(SpawnVirusSystem.instance.ReturnToPool(0, gameObject));
        }
        
    }

    public void Detect()
    {
        if (indicatorActive)
        {

            Vector2 direction = transform.position - indicatorArrow.transform.position;
            Vector2 targetViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
            float distance = direction.magnitude;
            float normalizedDistance = 1.5f - (distance / 50f) * 2f; //Algoritmo cambiar tama;o de indicador



            if (targetViewportPosition.x > 0f && targetViewportPosition.x < 1f && targetViewportPosition.y > 0f && targetViewportPosition.y < 1f)
            {
                indicatorArrow.SetActive(false);
            }
            else
            {
                indicatorArrow.SetActive(true);

                //Position
                Vector3 screenEgde = Camera.main.ViewportToWorldPoint(new Vector3(Mathf.Clamp(targetViewportPosition.x, 0.05f, 0.95f), Mathf.Clamp(targetViewportPosition.y, 0.1f, 0.9f), Camera.main.nearClipPlane));
                indicatorArrow.transform.position = screenEgde;

                //Rotation
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                indicatorArrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                indicatorArrow.transform.localScale = new Vector3(normalizedDistance, normalizedDistance, indicatorArrow.transform.localScale.z);
            }
        }
    }

}
