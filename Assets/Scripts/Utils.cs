using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
  public static bool LayerInMask(int layer, LayerMask mask) {
    return (mask == (mask | (1 << layer)));
  }
}
