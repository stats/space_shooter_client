using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_SlowRotation : MonoBehaviour
{

    int _axis = 0;

    // Start is called before the first frame update
    void Start()
    {
        _axis = Random.Range(0, 3);
        Debug.Log("Axis: " + _axis);
    }

    // Update is called once per frame
    void Update()
    {
        if (_axis == 0) transform.Rotate(Time.deltaTime * 10, 0, 0);
        if (_axis == 1) transform.Rotate(0, Time.deltaTime * 10, 0);
        if (_axis == 2) transform.Rotate(0, 0, Time.deltaTime * 10);
    }
}
