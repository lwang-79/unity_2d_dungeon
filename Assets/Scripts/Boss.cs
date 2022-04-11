using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public float[] fireballSpeed = { 2.5f, -2.5f };
    public float[] distance = { 0.25f, 0.4f };
    public Transform[] fireballs;


    private void Update()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] != null)
                fireballs[i].position = transform.position + new Vector3(
                    -Mathf.Cos(Time.time * fireballSpeed[i]) * distance[i],
                    Mathf.Sin(Time.time * fireballSpeed[i]) * distance[i], 0);
        }
    }
}
