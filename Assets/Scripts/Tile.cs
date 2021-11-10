using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    public bool occupied = false;
    public GameObject occupier = null;
    public bool attacked = false;

    public List<GameObject> curOccupants = new List<GameObject>();

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    public void addShipTiles()
    {
        Debug.Log(gameObject.name);
    }
}
