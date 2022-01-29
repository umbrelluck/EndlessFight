using System.Collections;
using UnityEngine;

namespace Umbr.EF.Powerups
{
    public class OriginLogic : MonoBehaviour
    {
        protected float rotationSpeed;
        private float moveSpeed;
        private float heightBound;
        private Vector3 upBoundPos;
        private Vector3 lowBoundPos;
        private Vector3 destination;

        private void Start()
        {
            moveSpeed = .125f;
            heightBound = .25f;

            upBoundPos = transform.position + Vector3.up * heightBound;
            lowBoundPos = transform.position + Vector3.down * heightBound;
            destination = upBoundPos;
        }

        protected void IdleMovement()
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destination) < 0.01)
                destination = (destination == lowBoundPos) ? upBoundPos : lowBoundPos;
        }

        protected void Rotate()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

    }
}