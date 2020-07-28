using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float x { get; private set; }
    public bool jump { get; private set; }
    public bool fire { get; private set; }

    void FixedUpdate() {
      this.x = Input.GetAxis("Horizontal");
      this.jump = Input.GetAxis("Jump") > 0;
      this.fire = Input.GetAxis("Fire1") > 0;
    }
}
