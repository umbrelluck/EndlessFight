using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Units
{
    public class DeathPart : MonoBehaviour
    {
        private Rigidbody rg;
        private float force;
        private void Awake()
        {
            rg = GetComponent<Rigidbody>();
            force = 0.09f;
        }
        // Start is called before the first frame update
        public void Kick(Vector3 direction)
        {
            rg.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}
