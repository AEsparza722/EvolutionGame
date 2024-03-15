using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelect : MonoBehaviour
{
    Vector2 initialPosition;
    [SerializeField] GameObject clickIndicator;
    [SerializeField] GameObject virusManager;
    [SerializeField] GameObject selectionAreaPrefab;
    GameObject selectionArea;
    [SerializeField] List<GameObject> virusSelected;
    

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
                foreach (GameObject virus in virusSelected) 
                {
                    virus.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
                }
                virusSelected.Clear();                
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
                    virusSelected.Add(virusManager.transform.GetChild(i).gameObject);
                    virusManager.transform.GetChild(i).GetChild(2).GetChild(1).gameObject.SetActive(true);
                }
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            if (virusSelected.Count > 0)
            {
                Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                StartCoroutine(ClickIndicator(clickPosition));

                foreach (GameObject virus in virusSelected)
                {
                    virus.GetComponent<CharacterControler>().MoveSelection(clickPosition);
                    virus.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
                }
                
                virusSelected.Clear();
                
            }
        }

        for (int i = 0; i < virusSelected.Count; i++)
        {
            if (virusSelected[i] == null)
            {
                virusSelected.RemoveAt(i);
                i--;
            }
        }
    }

    IEnumerator ClickIndicator(Vector2 clickPosition)
    {        
        clickIndicator.transform.position = clickPosition;
        clickIndicator.SetActive(true);
        yield return new WaitForSeconds(.5f);
        clickIndicator.SetActive(false);
    }
}
