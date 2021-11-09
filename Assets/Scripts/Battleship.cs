using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    private bool intact;
    private List<(int, int)> shipCoords;
    private List<bool> hit;

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
