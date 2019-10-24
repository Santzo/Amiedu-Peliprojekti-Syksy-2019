using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    [HideInInspector]
    public Bar healthBar;
    [HideInInspector]
    public Bar staminaBar;

    public static References rf;
    private void Awake()
    {
        if (rf == null)
        {
            rf = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

       
    }

    void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Bar>();
        staminaBar = GameObject.Find("StaminaBar").GetComponent<Bar>();

    }

}
