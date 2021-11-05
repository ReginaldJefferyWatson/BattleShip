using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box : MonoBehaviour
{
    // Start is called before the first frame update
    private float startPosX;
    private float startPosY;
    private bool isBeingHeld = false;
    private float rotZ = 0;

    void Update()
    {
        if(isBeingHeld == true)
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);

            if (Input.GetKeyDown("space"))
            {
                rotZ += 90;
                this.gameObject.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            }
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)) // Left Mouse Button Click
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            startPosX = mousePos.x - this.transform.localPosition.x;
            startPosY = mousePos.y - this.transform.localPosition.y;

            isBeingHeld = true;
        }
    }

    private void OnMouseUp()
    {
        isBeingHeld = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("HI");
        if (collider.gameObject.tag == "tile")
        {
            Debug.Log("HI");
        }
    }
}
