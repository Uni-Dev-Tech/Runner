using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform endPoint, leftPoint, rightPoint;
    public Transform centerCoin, leftCoin, rightCoin;
    public Transform[] centCoin, lfCoin, rhtCoin;
    public Transform leftBarrierPoint, rightBarrierPoint;
    public Transform craneCenterPoint, craneRightPoint, craneLeftPoint;

    void Start()
    {
        if(WorldController.instance != null) 
           WorldController.instance.OnPLatformMovement += TryDelAndAddPlatform; // привязка метода к ивенту
    }
    private void TryDelAndAddPlatform()
    {
        if (transform.position.z < WorldController.instance.minZ)
        {
            if (WorldBuilder.longhighlevelplatforms < 1) WorldController.instance.worldBuilder.CreatePlatform();
            if (WorldBuilder.longhighlevelplatforms > 0) WorldBuilder.longhighlevelplatforms--;
            Destroy(gameObject);
        }
        //if (PlayerControllerv2.death == true) Invoke("SelfDestroy", 2f);
    }
    private void OnDestroy()
    {
        if(WorldController.instance != null)
            WorldController.instance.OnPLatformMovement -= TryDelAndAddPlatform; // отвзяка от ивента
    }
    //public void SelfDestroy()
    //{
    //    Destroy(gameObject);
    //}
}
