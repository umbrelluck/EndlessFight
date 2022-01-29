using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAnimations : MonoBehaviour
{
    [SerializeField] private Rigidbody rg;
    [SerializeField] private Animator animCtrl;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rg.AddForce(movement.normalized * 2);
        Debug.Log(rg.velocity.magnitude);
        float speedMode = 0;
        if (rg.velocity.magnitude < 0.0001)
            speedMode = 0;
        else if (rg.velocity.magnitude < 3)
            speedMode = 0.5f;
        else
            speedMode = 1;
        animCtrl.SetFloat("Movement Speed", speedMode, 0.15f, Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
            animCtrl.SetTrigger("Attack");
    }
}
