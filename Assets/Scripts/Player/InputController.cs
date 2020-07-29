using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float x { get; private set; }
    public bool jump { get; private set; }
    public bool fire { get; private set; }
    public Vector2 fireDirection { get; private set; }

    void FixedUpdate() {
      this.x = Input.GetAxis("Horizontal");
      this.jump = Input.GetAxis("Jump") > 0;
      this.fire = Input.GetAxis("Fire1") > 0;
      if (this.fire) this.fireDirection = GetFireDirection();
    }

    private Vector2 GetFireDirection() {
      return new Vector2(Input.mousePosition.x - this.transform.position.x, Input.mousePosition.y  - this.transform.position.y);
    }


}
