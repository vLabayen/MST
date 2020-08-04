using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
  private PlayerStats stats;
  private LayerMask floorLayers;
  private InputController controller;
  private Rigidbody2D rb;
  private bool isGrounded = true;
  
  public void Setup(PlayerStats stats, LayerMask floorLayers) {
    this.stats = stats;
    this.floorLayers = floorLayers;
    this.controller = this.GetComponent<InputController>();
    this.rb = this.GetComponent<Rigidbody2D>();
    this.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    this.rb.gravityScale = this.stats.gravityScale;
  }

  void FixedUpdate() {
    this.transform.Translate(Vector3.right * this.controller.x * this.stats.movementSpeed * Time.fixedDeltaTime);

    if (this.isGrounded && this.controller.jump) {
      this.rb.velocity = new Vector2(0f, 0f);
      this.rb.AddForce(Vector2.up * this.stats.jumpForce, ForceMode2D.Impulse);
      this.isGrounded = false;
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    //Check that (the player is already in the floor) or (hits a floot and is not going up)
    this.isGrounded = this.isGrounded || (Utils.LayerInMask(collider.gameObject.layer, this.floorLayers) && this.rb.velocity.y <= 0);
  }

  //Esta funcion evita que puedas saltar desde el aire cuando te dejas caer de una plataforma
  //  desactivando isGrounded momento que dejas de estar en contacto con el suelo
  //Puede dar problemas si se hace una plataforma con multiples colliders y se entra
  //  en uno de ellos antes de dejar el anterior. Al entrar y estar en contacto no se activaria
  //  posteriormente al salir se desactivaria, a pesar de estar en contacto con el otro
  //  Se podria mirar de fixearlo (menos eficiente) con OnTriggerStay2D()
  void OnTriggerExit2D(Collider2D collider) {
    if (Utils.LayerInMask(collider.gameObject.layer, this.floorLayers)) this.isGrounded = false;
  }
}
