using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Controller : MonoBehaviour {

    public GameObject Camera;

    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 5f;
    public float DashDistance = 5f;
    public LayerMask Ground;

    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;

	// Use this for initialization
	void Start () {
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
        //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        _inputs = Vector3.zero;
        _inputs.x = -Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");
        if (_inputs != Vector3.zero)
        {
            Quaternion rotate = Camera.transform.rotation * Quaternion.Euler(0, -90, 0);
            rotate.x = 0;
            rotate.z = 0;

            transform.rotation = rotate;

        }
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        //if (Input.GetButtonDown("Dash"))
        //{
        //    Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
        //    _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        //}
	}

    // Physics Forced Update
    private void FixedUpdate()
    {
        
        _body.MovePosition(_body.position +  ((transform.forward * _inputs.x) + (transform.right * _inputs.z))  * Speed * Time.fixedDeltaTime);
    }
}
