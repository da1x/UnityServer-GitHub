using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(this);
        ServerTCP serverTCP = new ServerTCP();
        serverTCP.InitNetwork();

    }

    void Update()
    {

    }
}
