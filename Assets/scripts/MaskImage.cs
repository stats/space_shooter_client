using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaskDirection
{
    XDirection, YDirection
}

public class MaskImage : MonoBehaviour
{

    [Header("Main Properties")]
    public MaskDirection m_direction;
    public float m_size;

    private float position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_direction == MaskDirection.XDirection)
        {
            transform.Find("ForegroundImage").GetComponent<RectTransform>().localPosition = new Vector3(position, 0);
        } else
        {
            transform.Find("ForegroundImage").GetComponent<RectTransform>().localPosition = new Vector3(0 ,position);
        }
    }

    public void setValue(float fValue)
    {
        position = (m_size * fValue) - m_size;
    }
}
