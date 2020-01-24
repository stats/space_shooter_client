using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PickableMat
{
    public int index;
    string color;
    string sprite;

    public PickableMat(int index, string color, string sprite)
    {
        this.index = index;
        this.color = color;
        this.sprite = sprite;
    }

    public Color GetColor()
    {
        Color color_out;
        ColorUtility.TryParseHtmlString("#" + color, out color_out);
        return color_out;
    }

    public Sprite GetSprite()
    {
        if (sprite == null) return null;
        return Resources.Load<Sprite>("images/" + sprite);
    }

    public string ColorString()
    {
        return color;
    }
}

public class MaterialPicker : MonoBehaviour
{

    private int Value = 0;
    private PickableMat CurrentPickableMat;

    private GameObject MaterialList;
    private GameObject MaterialContainer;
    private GameObject CurrentMaterial;

    [Header("Events")]
    public UnityEvent onUpdateEvent = new UnityEvent();

    static public PickableMat[] MATERIALS = new PickableMat[]
    {
        new PickableMat(0,"000000", null),
        new PickableMat(1, "FFFFFF", null),
        new PickableMat(2, "FF0000", null),
        new PickableMat(3, "00FF00", null),
        new PickableMat(4, "0000FF", null),
        new PickableMat(5, "FFFF00", null),
        new PickableMat(6, "00FFFF", null),
        new PickableMat(7, "FF00FF", null),
        new PickableMat(8, "C0C0C0", null),
        new PickableMat(9, "808080", null),
        new PickableMat(10, "800000", null),
        new PickableMat(11, "808000", null),
        new PickableMat(12, "008000", null),
        new PickableMat(13, "800080", null),
        new PickableMat(14, "008080", null),
        new PickableMat(15, "000080", null)
    };

    // Start is called before the first frame update
    void Start()
    {
        CurrentMaterial = transform.Find("CurrentMaterial").gameObject;
        MaterialList = transform.Find("MaterialList").gameObject;
        MaterialContainer = MaterialList.transform.Find("MaterialContainer").gameObject;
        CurrentPickableMat = MATERIALS[0];
        Image current_img = CurrentMaterial.GetComponent<Image>();
        current_img.color = MATERIALS[0].GetColor();


        MaterialList.SetActive(false);
        foreach(PickableMat mat in MATERIALS)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("PickableButton"), Vector3.zero, Quaternion.identity);
            Image img = go.GetComponent<Image>();
            img.sprite = mat.GetSprite();
            img.color = mat.GetColor();
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { SelectItem(mat); });
            go.transform.SetParent(MaterialContainer.transform);
        }
    }

    public void SelectItem(PickableMat mat)
    {
        CurrentPickableMat = mat;
        Value = mat.index;
        Image current_img = CurrentMaterial.GetComponent<Image>();
        current_img.sprite = mat.GetSprite();
        current_img.color = mat.GetColor();
        
        HideList();
        if(onUpdateEvent != null)
        {
            onUpdateEvent.Invoke();
        }

    }

    public int GetValue()
    {
        return Value;
    }

    public PickableMat GetMat()
    {
        return CurrentPickableMat;
    }

    public void ShowList()
    {
        MaterialList.SetActive(true);
    }

    public void HideList()
    {
        MaterialList.SetActive(false);
    }
}
