using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieHeadGames.Helpers
{
    public class ZHG_ParticleSystemAutoDestroy : MonoBehaviour
    {
        private ParticleSystem ps;

        public void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void Update()
        {
            if (ps)
            {
                if (!ps.IsAlive())
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

