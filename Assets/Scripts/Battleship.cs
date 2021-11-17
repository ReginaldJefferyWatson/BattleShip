using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    public bool intact = true;
    public int size;
    public List<(int, int)> shipCoords = new List<(int, int)>();
    private List<bool> hit;

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    public void checkValid()
    {
        if (shipCoords.Count != size)
        {
            Debug.Log("Incorrect ship placement!");
        }
    }
}
