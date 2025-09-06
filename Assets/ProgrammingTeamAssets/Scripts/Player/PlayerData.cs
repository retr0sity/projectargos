using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData
{
    public int Scene;
    public float[] position;

    public PlayerData(Player player)
    {
        Scene = SceneManager.GetActiveScene().buildIndex;
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}