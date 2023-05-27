using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum State
    {
        Move,
        TryCatch,
        Catch
    }

    private State state;
    [SerializeField] private Slider sliderTryCatch;
    [SerializeField] private Slider sliderCatch;
    [SerializeField] private float tryCatchSpeed = 1f;
    [SerializeField] private float playerStrong = 0.2f;
    [SerializeField] private float horseStrong = 2;
    public void Update()
    {
        if (state == State.Move)
        {
            if (Input.GetMouseButtonDown(0))
            {
                state = State.TryCatch;
                sliderTryCatch.gameObject.SetActive(true);
                sliderTryCatch.value = 0;
            }
        }
        if (state == State.TryCatch)
        {
            if (Input.GetMouseButton(0))
            {
                sliderTryCatch.value += tryCatchSpeed * Time.deltaTime;
                if (sliderTryCatch.value is <= 0 or >= 1) tryCatchSpeed = -tryCatchSpeed;
            }
            if(Input.GetMouseButtonUp(0))
            {
                //throw
                //trungs hoac khong
                state = State.Catch;
                sliderTryCatch.gameObject.SetActive(false);
                sliderCatch.gameObject.SetActive(true);
                sliderCatch.value = 0.5f;
            }
        }

        if (state == State.Catch)
        {
            sliderCatch.value -= horseStrong*Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                sliderCatch.value += playerStrong;
                Debug.Log("Catching");
            }

            if (sliderCatch.value is <= 0 or >= 1)
            {
                switch (sliderCatch.value)
                {
                    case <= 0:
                        Debug.Log("Catch Fail");
                        break;
                    case >= 1:
                        Debug.Log("Catch Done");
                        break;
                }
                sliderCatch.gameObject.SetActive(false);
                state = State.Move;
            }
           
        }
    }
}
