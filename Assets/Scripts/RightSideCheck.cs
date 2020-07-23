using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightSideCheck : MonoBehaviour
{
    public Transform objToFollow;
    private Vector3 deltaPos;

    static public bool obstructionRight;

    private void Start()
    {
        deltaPos = transform.position - objToFollow.position;
        obstructionRight = false;
    }
    private void Update()
    {
        transform.position = objToFollow.position + deltaPos;
    }
    private void OnTriggerExit(Collider other)
    {
       obstructionRight = false;
    }
    private void OnTriggerStay(Collider other)
    {
        obstructionRight = true;
    }
}
