using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPanning : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    //[SerializeField] GameObject backgroundVertical;
    //[SerializeField] GameObject backgroundHorizontal;
    //[SerializeField] float panningSpeed;
    //float HorizontalPanningSpeed;
    //float scrollVerticalLimit = 79.4f;
    //bool moveRight;

    //private void Start()
    //{
    //    HorizontalPanningSpeed = panningSpeed * 1.5f; 
    //}

    //void Update()
    //{

    //    backgroundVertical.transform.position += new Vector3(0f, panningSpeed*Time.deltaTime, 0f);


    //    if (backgroundVertical.transform.position.y >= scrollVerticalLimit)
    //    {
    //        backgroundVertical.transform.position = new Vector3(0f, 0f, 0f);
    //    }

    //    //Horizontal

    //    if (!moveRight && backgroundHorizontal.transform.position.x > -50)
    //    {
    //        backgroundHorizontal.transform.position -= new Vector3(HorizontalPanningSpeed * Time.deltaTime, 0f, 0f);
    //    }
    //    else
    //    {
    //        moveRight = true;

    //    }

    //    if (moveRight && backgroundHorizontal.transform.position.x < 0)
    //    {
    //        backgroundHorizontal.transform.position += new Vector3(HorizontalPanningSpeed * Time.deltaTime, 0f, 0f);
    //    }
    //    else
    //    {
    //        moveRight = false;
    //    }



    //}
}
