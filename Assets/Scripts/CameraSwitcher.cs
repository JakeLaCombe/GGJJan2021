using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera TopPriorty;
    public Cinemachine.CinemachineVirtualCamera LowPriorty;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        TopPriorty.Priority = 10;
        LowPriorty.Priority = 9;
    }
}
