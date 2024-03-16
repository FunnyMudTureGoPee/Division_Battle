using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    void Update()
    {
        Ray2D ray = new Ray2D(transform.position, transform.forward);
        //声明一个Ray结构体，用于存储该射线的发射点，方向
        RaycastHit2D info=Physics2D.Raycast(ray.origin,ray.direction);
        //Debug.DrawRay(ray.origin,ray.direction,Color.blue);
        Debug.DrawRay( ray.origin, ray.direction, Color.blue );//起点，方向，颜色（可选）
        if(info.collider!=null){
            if(info.transform.gameObject.CompareTag("Enemy")){
                Debug.LogWarning("检测到敌人");
            }else{
                Debug.Log(info.collider.gameObject.name);
            }
        }else{
            Debug.Log("没有碰撞任何对象");
        }
    }

}
