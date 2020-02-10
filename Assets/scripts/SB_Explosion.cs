using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_Explosion
{
    public static GameObject GetExplosion(float radius, Vector3 position)
    {
        GameObject Explosion = GameObject.Instantiate(Resources.Load<GameObject>("explosions/BulletExplosion_1"), position, Quaternion.identity);
        Explosion.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        return Explosion;
    }
}
