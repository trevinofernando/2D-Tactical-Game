using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, Vector3 explosionPosition, float explosionRadius, float explosionForce)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * explosionForce * wearoff);
        //body.AddForce(new Vector2( (dir.normalized.x * 10f), dir.normalized.y)* explosionForce * wearoff);
    }

    public static void AddExplosionForce(this Rigidbody2D body, Vector3 explosionPosition, float explosionRadius, float explosionForce, float upForce)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        //body.AddForce(baseForce);
        float upWearoff = 1 - (upForce / explosionRadius);
        Vector3 extraForce = Vector2.up * explosionForce * upWearoff;
        body.AddForce(baseForce + extraForce);
    }
}
