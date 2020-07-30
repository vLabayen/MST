using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletMovement : MonoBehaviour
{
  public BulletStats stats;
  private Vector3 movementDirection;
  private float liveTime;

  public void Setup(Vector2 shootDirection) {
    this.movementDirection = Vector3.Normalize(new Vector3(shootDirection.x, shootDirection.y, 0f));
    this.liveTime = stats.range / stats.movementSpeed;
    this.enabled = true;
    Destroy(this.gameObject, this.liveTime);
  }

  void FixedUpdate() {
    this.transform.Translate(this.movementDirection * this.stats.movementSpeed * Time.fixedDeltaTime);
  }
}
