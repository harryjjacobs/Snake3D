using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

	public static Vector3 RoundToNearestInt(Vector3 v, int nearest)
    {
        return new Vector3((int)Mathf.Round(v.x / nearest) * nearest, (int)Mathf.Round(v.y / nearest) * nearest, (int)Mathf.Round(v.z / nearest) * nearest);
    }
}
