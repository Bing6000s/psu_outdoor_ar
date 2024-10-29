using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 

public class GameData
{
    public Vector3 playerVec;                       // Stores the players position
    //* Note that the vector3 will need to be replaced by a list of two elements for Long and Lat
    public Dictionary<Vector3, bool> targets;       // Stores the targets position

    public GameData()
    {
        this.playerVec = new Vector3(1f,0.6f,1f);               // Starting position of the player
        this.targets = new Dictionary<Vector3, bool>();        // Declares empty Dictionary of markers

    }
}