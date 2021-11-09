using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileAttack : MonoBehaviour
{
    private void OnMouseDown()
    {
        this.gameObject.GetComponent<Tile>().attacked = true;
        this.gameObject.SetActive(false);
    }
}
