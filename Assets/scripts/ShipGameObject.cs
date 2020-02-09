using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGameObject : MonoBehaviour
{

    public Ship shipData;

    private GameObject ship_type;

    private GameObject _ForceField;
    private GameObject _RammingShield;

    public void UpdateComponents()
    {
        if(shipData == null)
        {
            shipData = new Ship();
        }
        Destroy(ship_type);
        ship_type = (GameObject)Instantiate(Resources.Load("ships/" + shipData.ship_type));
        ship_type.transform.Find("mesh").GetComponent<MeshRenderer>().material = Instantiate<Material>((Material)Resources.Load("ships/materials/" + shipData.ship_material, typeof(Material)));
        ship_type.transform.SetParent(transform, false);

        _ForceField = ship_type.transform.Find("ForceField").gameObject;
        _RammingShield = ship_type.transform.Find("RammingShield").gameObject;
    }

    public void ActivateForceField()
    {
        _ForceField.SetActive(true);
    }

    public void ActivateRammingShield()
    {
        _RammingShield.SetActive(true);
    }

    public void DeactivateShields()
    {
        _ForceField.SetActive(false);
        _RammingShield.SetActive(false);
    }

}
