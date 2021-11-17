using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileAttack : MonoBehaviour
{

    public GameObject GridManager;
    private void OnMouseDown()
    {
        //Only allow tile to be attacked if it hasn't already been
        if (!this.gameObject.GetComponent<Tile>().attacked)
        {
            if (this.gameObject.GetComponent<Tile>().occupiedGameStart == true)
            {
                Debug.Log("HIT");
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

                //Add hit object to hit list
                GridManager.GetComponent<GridManager>().enemyHitSpaces.Add(((int) this.gameObject.transform.position.x, (int)this.gameObject.transform.position.y - GridManager.GetComponent<GridManager>().enemyTileY));

                //Check to see if any ships were destroyed by the move
                GridManager.GetComponent<GridManager>().checkDestroyedEnemy();
            }
            else
            {
                Debug.Log("MISS");
                //this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                this.gameObject.SetActive(false);
            }

            this.gameObject.GetComponent<Tile>().attacked = true;
        }
    }
}
