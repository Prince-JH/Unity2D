using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneCommand : MonoBehaviour
{
    public AudioSource BirdSound_1, BirdSound_2, BirdSound_3, BirdSound_4;
    Rigidbody2D rig;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.AddForce(Vector3.up * 270);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Animator>().SetFloat("Velocity", rig.velocity.y);
        if((Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            rig.velocity = Vector3.zero; //순간 속력이 0이 되어야 힘을 제대로 받음
            rig.AddForce(Vector3.up * 270);
            BirdSound_1.Play();
        }
    }
}
