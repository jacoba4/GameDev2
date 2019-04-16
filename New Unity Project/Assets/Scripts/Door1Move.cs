﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door1Move : MonoBehaviour
{
    private Vector3 shift;
    private Vector3 start;
    float i;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.localPosition;
        shift = new Vector3(-170f,0f,0f);
        i = 0;
        speed = .05f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MoveTheDoor()
    {
        //transform.localPosition = new Vector3(160f,0f,0f);
        StartCoroutine("doormove");
    }
    
    IEnumerator doormove()
    {
        while(transform.localPosition != shift)
        {
            transform.localPosition = Vector3.Lerp(start,shift,i);
            i+=speed;
            yield return null;
        }
        
    }
}