using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class TakeShipControl : MonoBehaviour
{
    public bool isDetecting;
    
    
    private bool canChange = true;
    private GameObject player;

    [SerializeField] private float rotationSpeed;
    [SerializeField] Material boatMat;
    
    [SerializeField] private GameObject traps;
    [SerializeField] private GameObject ropes;

    private void Awake()
    {
        boatMat.SetFloat("_LerpOutline", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            isDetecting = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            isDetecting = false;
            player = null;
        }
    }

    private void LateUpdate()
    {
        TakeControl();
    }

    private void TakeControl()
    {
        if (isDetecting && canChange && player.GetComponent<PlayerController>().interacting)
        {
            StartCoroutine(ChangeOwnership());
        }
    }

    private IEnumerator ChangeOwnership()
    {
        canChange = false;
        StartCoroutine(RotateRudder());

        if (player.name == "Player_Red")
        {
            boatMat.SetFloat("_LerpOutline", 1);
            boatMat.SetFloat("_WhichColor", 0);
            foreach (var rope in ropes.GetComponentsInChildren<RopesManager>())
            {
                rope.state = RopesManager.State.Red;
            }
            foreach (var trap in traps.GetComponentsInChildren<TrapManager>())
            {
                if (trap.state == TrapManager.State.Disabled)
                {
                    trap.state = TrapManager.State.Red;
                }
                else if (trap.state == TrapManager.State.Blue)
                {
                    trap.state = TrapManager.State.Disabled;
                }
            }
        }
        else if (player.name == "Player_Blue")
        {
            boatMat.SetFloat("_LerpOutline", 1);
            boatMat.SetFloat("_WhichColor", 1);
            foreach (var rope in ropes.GetComponentsInChildren<RopesManager>())
            {
                rope.state = RopesManager.State.Blue;
            }
            foreach (var trap in traps.GetComponentsInChildren<TrapManager>())
            {
                if (trap.state == TrapManager.State.Disabled)
                {
                    trap.state = TrapManager.State.Blue;
                }
                else if (trap.state == TrapManager.State.Red)
                {
                    trap.state = TrapManager.State.Disabled;
                }
            }
        }


        yield return new WaitForSeconds(3);
        canChange = true;
        rotationSpeed *= -1;
    }

    private IEnumerator RotateRudder()
    {
        transform.Rotate(Vector3.forward * (Time.deltaTime * rotationSpeed));
        yield return new WaitForFixedUpdate();
        if (canChange == false)
        {
            StartCoroutine(RotateRudder());
        }

        
    }
}