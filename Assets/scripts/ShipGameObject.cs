using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGameObject : MonoBehaviour
{

    public Ship shipData;

    private GameObject ship_type;

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
    }

    public void ActivateForceField()
    {
        ship_type.transform.Find("mesh").GetComponent<MeshRenderer>().material = Instantiate<Material>((Material)Resources.Load("ships/materials/ShieldSprite", typeof(Material)));
    }

    public void ActivateRammingShield()
    {
        ship_type.transform.Find("mesh").GetComponent<MeshRenderer>().material = Instantiate<Material>((Material)Resources.Load("ships/materials/RammingShield", typeof(Material)));
    }

    public void DeactivateShields()
    {
        ship_type.transform.Find("mesh").GetComponent<MeshRenderer>().material = Instantiate<Material>((Material)Resources.Load("ships/materials/" + shipData.ship_material, typeof(Material)));

    }

}
