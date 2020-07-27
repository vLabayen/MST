using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Stats/Player")]
public class PlayerStats : ScriptableObject {
  public float movementSpeed;
  public float jumpForce;
  public float gravityScale;
}
