using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public int Scene;

    public void SavePlayer()
    {
        SaveData.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveData.LoadPlayer();
        if (data != null)
        {
            Scene = data.Scene;
            SceneManager.LoadScene(Scene);
            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];
            transform.position = position;
        }
    }
}