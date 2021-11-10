using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    private bool intact;
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
}
