using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "Stats/Bullet")]
public class BulletStats : ScriptableObject {
  public float movementSpeed;
  public float range;
}
