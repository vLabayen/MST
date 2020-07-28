using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
public class PlayerShoot : MonoBehaviour
{
    public Transform shootPoint;
    public PlayerStats stats;

    private InputController controller;
    private bool canShoot;

    void Start() {
      this.controller = this.GetComponent<InputController>();
      this.canShoot = true;
    }

    void FixedUpdate() {
      if (this.controller.fire && this.canShoot) {
        StartCoroutine(Shoot());
      }
    }

    private IEnumerator Shoot() {
      this.canShoot = false;
      Object.Instantiate(this.stats.bulletPrefab, shootPoint.position, shootPoint.rotation).GetComponent<BulletMovement>().Setup(Vector2.right);
      yield return new WaitForSeconds(1f / stats.shootRate);
      this.canShoot = true;
    }
}
