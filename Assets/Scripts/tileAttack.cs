using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileAttack : MonoBehaviour
{
    private void OnMouseDown()
    {
        if(this.gameObject.GetComponent<Tile>().occupiedGameStart == true)
        {
            Debug.Log("HIT");
        }
        else
        {
            Debug.Log("MISS");
        }

        this.gameObject.GetComponent<Tile>().attacked = true;
        this.gameObject.SetActive(false);
    }
}
