using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

    [SerializeField] Text scoreText;
    [SerializeField] Text livesText;

    public Color goodBucketVFX;
    public Color badBucketVFX;
    public Color magicBucketVFX;

    public List<Bucket> buckets = new List<Bucket>();

    public float bucketGenerationInterval = 3f;
    public float bucketGenerationDelay = 0.5f;
    public float badBucketInitialChance = 0.1f;
    public float badBucketChanceStep = 0.05f;
    public float magicBucketChance = 0.1f;

    public int maxLives = 10;
    public int score = 0;
    public int lives;

    private float badBucketChance;
    private float bucketHeight;
    private Bucket bucketPrefab;
    private GameObject pickupVFX;

    public void PickupBucket(Bucket bucket)
    {
        switch (bucket.type)
        {
            case Bucket.BucketType.Bad:
                DecreaseLives();
                break;

            case Bucket.BucketType.Good:
                score++;
                break;

            case Bucket.BucketType.Magic:
                lives = maxLives;
                break;
        }

        buckets.Remove(bucket);
        PlayVFX(bucket.type, bucket.gameObject.transform.position);
        DestroyImmediate(bucket.gameObject);
        RenderUI();
    }

    private void PlayVFX(Bucket.BucketType type, Vector3 position)
    {
        GameObject vfx = Instantiate(pickupVFX, position, Quaternion.identity);
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();

        var main = ps.main;
        switch (type)
        {
            case Bucket.BucketType.Bad:
                
                main.startColor = badBucketVFX;
                break;

            case Bucket.BucketType.Good:
                main.startColor = goodBucketVFX;
                break;

            case Bucket.BucketType.Magic:
                main.startColor = magicBucketVFX;
                break;
        }

        Destroy(vfx.gameObject, 1f);
    }

    // Use this for initialization
    void Start () {
        badBucketChance = badBucketInitialChance;
		bucketPrefab = Resources.Load<Bucket>("Prefabs/Bucket");
        pickupVFX = Resources.Load<GameObject>("Prefabs/PickupVFX");
        InvokeRepeating("GenerateBucket", bucketGenerationDelay, bucketGenerationInterval);
        lives = maxLives;
        RenderUI();
    }

    private void RenderUI()
    {
        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives + "/" + maxLives;
    }

    private void DecreaseLives()
    {
        lives--;
        if (lives == 0)
        {
            SceneManager.LoadScene("Main");
        }
    }

    private void GenerateBucket()
    {
        if (buckets.Count < 10)
        {
            Vector3 bucketPosition = GenerateBucketPosition();
            Bucket bucket = Instantiate(bucketPrefab, bucketPosition, Quaternion.identity);
            bucket.SetType( DefineBucketType() );
            buckets.Add(bucket);
        }
    }

    private Bucket.BucketType DefineBucketType()
    {
        if (Random.value <= badBucketChance)
        {
            badBucketChance = badBucketInitialChance;
            return Bucket.BucketType.Bad;
        }

        badBucketChance += badBucketChanceStep;

        return Random.value <= magicBucketChance ? Bucket.BucketType.Magic : Bucket.BucketType.Good;
    }

    private Vector3 GenerateBucketPosition()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(Random.Range(0.05f, 0.95f), Random.Range(0.05f, 0.95f)));
        Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));

        float point = 0f;
        plane.Raycast(ray, out point);

        Vector3 bucketPosition = ray.GetPoint(point);
        bucketPosition.y += bucketPrefab.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        return bucketPosition;
    }
}
