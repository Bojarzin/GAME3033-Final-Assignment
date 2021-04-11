using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [SerializeField] Slider diveChargeSlider;
    [SerializeField] PlayerController arrowhead;

    // Start is called before the first frame update
    void Start()
    {
        diveChargeSlider.minValue = arrowhead.diveStrength;
        diveChargeSlider.maxValue = arrowhead.GetMaxDiveStrength();
    }

    // Update is called once per frame
    void Update()
    {
        diveChargeSlider.value = arrowhead.GetCurrentDiveStrength();   
    }
}
