using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
public class PlayerShoot : MonoBehaviour
{
    private Transform shootPoint;
    private PlayerStats stats;
    private InputController controller;
    private bool canShoot;

    public void Setup(PlayerStats stats, Transform shootPoint) {
      this.stats = stats;
      this.shootPoint = shootPoint;
      this.controller = this.GetComponent<InputController>();
      this.canShoot = true;
    }

    void FixedUpdate() {
      if (this.controller.fire && this.canShoot) {
        StartCoroutine(Shoot(this.controller.fireDirection));
      }
    }

    private IEnumerator Shoot(Vector2 fireDirection) {
      this.canShoot = false;
      Object.Instantiate(this.stats.bulletPrefab, shootPoint.position, shootPoint.rotation).GetComponent<BulletMovement>().Setup(fireDirection);
      yield return new WaitForSeconds(1f / stats.shootRate);
      this.canShoot = true;
    }
}
