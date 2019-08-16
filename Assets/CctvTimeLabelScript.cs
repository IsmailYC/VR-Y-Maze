using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CctvTimeLabelScript : MonoBehaviour
{
    Text timeLabel;
    // Start is called before the first frame update
    void Start()
    {
        timeLabel = GetComponent<Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        timeLabel.text = System.DateTime.Now.ToString("hh:mm:ss");
    }
}
