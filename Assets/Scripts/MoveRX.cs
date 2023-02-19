using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRX : MonoBehaviour
{
    public float moveDistance = .5f;
    public float degrees = 1f;

    public Transform circle;

/*    private void Start()
    {
        circle = GameObject.Find("Circle").GetComponent<Transform>();
    }*/

    void FixedUpdate()
    {
        transform.position += new Vector3(moveDistance, 0f, 0f);
        //circle.position += new Vector3(moveDistance, 0f, 0f);
        transform.eulerAngles += Vector3.forward * degrees;
    }
}
