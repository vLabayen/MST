using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Camera playerCamera;

    public float x { get { return msg.x; } private set { msg.x = value; }}
    public bool jump { get { return msg.jump; } private set { msg.jump = value; }}
    public bool fire { get { return msg.fire; } private set { msg.fire = value; }}
    public Vector2 fireDirection { get { return msg.fireDirection; } private set { msg.fireDirection = value; }}

    public delegate void OnInputChangesDelegate(PlayerInputMsg msg);
    public event OnInputChangesDelegate onInputRefreshed;

    public void Setup(Camera playerCamera) {
      this.playerCamera = playerCamera;
      this.msg = new PlayerInputMsg();
    }

    void FixedUpdate() {
      if (msg == null) return;

      this.x = Input.GetAxis("Horizontal");
      this.jump = Input.GetAxis("Jump") > 0;
      this.fire = Input.GetAxis("Fire1") > 0;
      if (this.fire) this.fireDirection = GetFireDirection();

      onInputRefreshed?.Invoke(msg);
    }

    private Vector2 GetFireDirection() {
      Vector3 screenPos = playerCamera.WorldToScreenPoint(this.transform.position);
      return new Vector2(Input.mousePosition.x - screenPos.x, Input.mousePosition.y  - screenPos.y);
    }


}
