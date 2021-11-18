using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileAttack : MonoBehaviour
{
    public bool attackable = true;
    public bool recentlyDestroyed = false;

    public GameObject GridManager;
    private void OnMouseDown()
    {
        if (attackable)
        {
            //Only allow tile to be attacked if it hasn't already been
            if (!this.gameObject.GetComponent<Tile>().attacked)
            {
                if (this.gameObject.GetComponent<Tile>().occupiedGameStart == true)
                {
                    Debug.Log("HIT");
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

                    //Add hit object to hit list
                    GridManager.GetComponent<GridManager>().enemyHitSpaces.Add(((int)this.gameObject.transform.position.x, (int)this.gameObject.transform.position.y - GridManager.GetComponent<GridManager>().enemyTileY));

                    //Check to see if any ships were destroyed by the move
                    if (GridManager.GetComponent<GridManager>().checkDestroyedEnemy(this.gameObject.GetComponent<Tile>().occupierGameStart.name))
                    {
                        GridManager.gameObject.GetComponent<GridManager>().enemyDestroyedDelay();
                    }
                    else
                    {
                        //Display hit message
                        GridManager.gameObject.GetComponent<GridManager>().enemyHitDelay();
                    }

                }
                else
                {
                    Debug.Log("MISS");
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;

                    GridManager.gameObject.GetComponent<GridManager>().enemyMissDelay();
                    //this.gameObject.SetActive(false);
                }

                this.gameObject.GetComponent<Tile>().attacked = true;

                //Now, move to the enemy's turn to attack
                //Disable enemy tile click script
                GridManager.gameObject.GetComponent<GridManager>().disableEnemyTiles();

                //Show text indicating enemy's turn
                //StartCoroutine(GridManager.gameObject.GetComponent<GridManager>().enemyTurnDelay());
            }
        }
    }

}
