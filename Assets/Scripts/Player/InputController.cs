using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Camera playerCamera = null;
    private bool active = false;

    public float x { get; private set; }
    public bool jump { get; private set; }
    public bool fire { get; private set; }
    public Vector2 fireDirection { get; private set; }

    public delegate void OnInputActiveDelegate(PlayerInputMsg msg);
    public event OnInputActiveDelegate onInputActive;

    public void Setup(Camera playerCamera) {
      this.playerCamera = playerCamera;
      this.active = true;
    }

    void FixedUpdate() {
      if (!active) return;

      this.x = Input.GetAxis("Horizontal");
      this.jump = Input.GetAxis("Jump") > 0;
      this.fire = Input.GetAxis("Fire1") > 0;
      if (this.fire) this.fireDirection = GetFireDirection();

      // onInputActive?.Invoke();
    }

    private Vector2 GetFireDirection() {
      Vector3 screenPos = playerCamera.WorldToScreenPoint(this.transform.position);
      return new Vector2(Input.mousePosition.x - screenPos.x, Input.mousePosition.y  - screenPos.y);
    }


}
