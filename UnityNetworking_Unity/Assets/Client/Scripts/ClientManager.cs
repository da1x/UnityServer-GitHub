using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
        ClientTCP.instance.Connect();
    }

    private void OnApplicationQuit()
    {
        ClientTCP.instance.client.Close();
    }

    void Update()
    {

    }
}
