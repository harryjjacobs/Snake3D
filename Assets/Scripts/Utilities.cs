using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

	public static Vector3 RoundToNearest(Vector3 v, float nearest)
    {
        return new Vector3(Mathf.Round(v.x / nearest) * nearest, Mathf.Round(v.y / nearest) * nearest, Mathf.Round(v.z / nearest) * nearest);
    }
}
