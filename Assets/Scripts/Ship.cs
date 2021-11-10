using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public int size;
    private bool intact;
    public List<(int, int)> shipCoords;
    private List<bool> hit;

    public void checkValid()
    {
        if(shipCoords.Count != size)
        {
            Debug.Log("Incorrect ship placement!");
        }
    }
}
