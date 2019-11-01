using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSettingsMenu()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }


}
