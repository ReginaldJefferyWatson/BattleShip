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

            //this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);

            //For even-length ships
            if (this.gameObject.name == "Battleship" || this.gameObject.name == "Destroyer")
            {
                //To account for weird ass sideways angles
                if (rotZ == 90 || rotZ == -90)
                {
                    this.gameObject.transform.localPosition = new Vector3(Mathf.RoundToInt(mousePos.x - startPosX) - 0.5f, Mathf.RoundToInt(mousePos.y - startPosY), 0);
                }
                else
                {
                    this.gameObject.transform.localPosition = new Vector3(Mathf.RoundToInt(mousePos.x - startPosX), Mathf.RoundToInt(mousePos.y - startPosY) - 0.5f, 0);
                }
            }
            else
            {
                this.gameObject.transform.localPosition = new Vector3(Mathf.RoundToInt(mousePos.x - startPosX), Mathf.RoundToInt(mousePos.y - startPosY), 0);
            }

            if (Input.GetKeyDown("space"))
            {
                //To reset the rotation
                if (rotZ == -180)
                {
                    rotZ = 90;
                }
                else
                {
                    rotZ -= 90;
                }
                this.gameObject.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            }
        }

        //if(this.gameObject.GetComponent
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Tile>().occupied = true;
        collision.gameObject.GetComponent<Tile>().occupier = gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Tile>().occupied = false;
        collision.gameObject.GetComponent<Tile>().occupier = null;
    }
}
