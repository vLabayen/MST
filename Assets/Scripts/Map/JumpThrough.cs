using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpThrough : MonoBehaviour
{
  public Collider2D solidCollider;
  public LayerMask playerLayer;

  void OnTriggerEnter2D(Collider2D collider) {
    //Check if the other collider is from a player
    if (Utils.LayerInMask(collider.gameObject.layer, this.playerLayer)) {
      //Get the rigidbody and verify that the player is moving up (is jumping)
      Rigidbody2D rb = collider.gameObject.GetComponent<Rigidbody2D>();
      if (rb != null && rb.velocity.y > 0) {
        //Set to ignore collisions between both objects
        Physics2D.IgnoreCollision(this.solidCollider, collider, true);
      }
    }
  }

  void OnTriggerExit2D(Collider2D collider) {
    //Same logic but to re-enable collisions
    if (Utils.LayerInMask(collider.gameObject.layer, this.playerLayer)) {
      //No need to check moving direction
      Physics2D.IgnoreCollision(this.solidCollider, collider, false);
    }
  }
}
