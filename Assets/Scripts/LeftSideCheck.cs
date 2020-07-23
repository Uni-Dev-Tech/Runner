using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftSideCheck : MonoBehaviour
{
    public Transform objToFollow;
    private Vector3 deltaPos;

    static public bool obstructionLeft;

    private void Start()
    {
        deltaPos = transform.position - objToFollow.position;
        obstructionLeft = false;
    }
    private void Update()
    {
        transform.position = objToFollow.position + deltaPos;
    }
    private void OnTriggerExit(Collider other)
    {
        obstructionLeft = false;
    }
    private void OnTriggerStay(Collider other)
    {
        obstructionLeft = true;
    }
}
