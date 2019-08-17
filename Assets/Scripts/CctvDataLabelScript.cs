using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CctvDataLabelScript : MonoBehaviour
{
    Text dateLabel;
    // Start is called before the first frame update
    void Start()
    {
        dateLabel = GetComponent<Text>();
        dateLabel.text = System.DateTime.Now.ToString("dd/MM/yyyy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
