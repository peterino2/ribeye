using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    public static float RangeMapping(float a, float b, float x, float y, float z) {
        //Return
        return Mathf.Lerp(a, b, Mathf.InverseLerp(x, y, z));
    }

    public static Vector2 TargetAngles(Transform self, Transform target) {
        //Declare
        Vector3 selfForwardXZ = new Vector3(self.forward.x, 0f, self.forward.z);
        Vector3 selfForwardYZ = new Vector3(0f, self.forward.y, self.forward.z);
        Vector3 targetForwardXZ = new Vector3(target.position.x - self.position.x, 0f, target.position.z - self.position.z).normalized;
        Vector3 targetForwardYZ = new Vector3(0f, target.position.y - self.position.y, target.position.z - self.position.z).normalized;
        //Return
        return new Vector2(Vector3.SignedAngle(selfForwardXZ, targetForwardXZ, self.up), Vector3.SignedAngle(selfForwardYZ, targetForwardYZ, self.right));
    }

}