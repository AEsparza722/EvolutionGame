using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelect : MonoBehaviour
{
    Vector2 initialPosition;
    [SerializeField] GameObject virusManager;
    [SerializeField] GameObject selectionAreaPrefab;
    GameObject selectionArea;

    private void Awake()
    {
        selectionArea = Instantiate(selectionAreaPrefab);
        selectionArea.SetActive(false);
    }

    void Update()
    {
        if (!GameManager.instance.isDraggingVirus)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
            
                selectionArea.SetActive(true);
                initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                selectionArea.transform.position = initialPosition;
            }

            if (Input.GetMouseButton(0)) 
            {
                Vector2 selectionAreaSize = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - initialPosition;
                selectionArea.transform.localScale = selectionAreaSize;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectionArea.SetActive(false);
            Vector2 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lowerLeftPosition = new Vector2(Mathf.Min(initialPosition.x, endPosition.x), Mathf.Min(initialPosition.y, endPosition.y));
            Vector2 upperRightPosition = new Vector2(Mathf.Max(initialPosition.x, endPosition.x), Mathf.Max(initialPosition.y, endPosition.y));

            for (int i = 0; i < virusManager.transform.childCount; i++)
            {
                Vector2 virusPosition = virusManager.transform.GetChild(i).transform.position;
                if (
                    virusPosition.x >= lowerLeftPosition.x &&
                    virusPosition.y >= lowerLeftPosition.y && 
                    virusPosition.x <= upperRightPosition.x &&
                    virusPosition.y <= upperRightPosition.y
                    )
                {
                    Debug.Log(virusManager.transform.GetChild(i).name);
                }
            }

        }
    }
}
