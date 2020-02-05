using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour
{


    DataManager dataManager;
    public Text name;
    public Text niveau;
    public Text fragment;
    public Text or;

    float timerAfk = 0.0f;



    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.dataManager;

    }

    // Update is called once per frame
    void Update()
    {
      
        name.text = dataManager.GetUsername();
        niveau.text = dataManager.GetPlayerLevel().ToString();
        fragment.text = dataManager.GetRuneFragment().ToString();
        or.text = dataManager.GetIRLMonney().ToString();


    }
}
