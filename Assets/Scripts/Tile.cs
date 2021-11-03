using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    void OnMouseEnter()
    {
        Debug.Log("On Tile");
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        Debug.Log("Off Tile");
        highlight.SetActive(true);
    }
}
