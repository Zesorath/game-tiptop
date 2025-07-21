using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public static float Speed { set; get; }

    void Awake()
    {
        //Speed = 10f;
    }

    void Update()
    {
        float direction = Speed >= 0 ? -1f : 1f;

        transform.Translate(Vector3.right * Mathf.Abs(Speed) * direction * Time.deltaTime);
    }
}
