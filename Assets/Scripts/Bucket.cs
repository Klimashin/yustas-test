using UnityEngine;

public class Bucket : MonoBehaviour {
    public enum BucketType
    {
        Good,
        Bad,
        Magic
    }

    public BucketType type;

    public void SetType(BucketType t)
    {
        type = t;
    }
}
