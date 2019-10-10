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

    public static void AddExplosionForce(this Rigidbody2D body, Vector3 explosionPosition, float explosionRadius, float explosionForce, float sideForce)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        //body.AddForce(baseForce);

        float sideWearoff = 1 - (sideForce / explosionRadius);
        Vector3 extraForce = Vector2.right * dir.normalized.x * explosionForce * sideWearoff;
        body.AddForce(baseForce + extraForce);
    }
}
