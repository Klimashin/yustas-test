using UnityEngine;
using System.Collections;
using System;

public class Character : MonoBehaviour {

    public float workAnimationLength = 0.5f;
    public float speed = 5f;

    //cached references
    private Animator _animator;
    private Camera _camera;
    private GameState _gameState;

    public bool isMoving = false;
    public bool isWorking = false;
    private Vector3 targetPosition;

    // Use this for initialization
    void Start () {
        _animator = GetComponent<Animator>();
        _gameState = FindObjectOfType<GameState>();
        _camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && !isMoving && !isWorking)
        {
            SetTargetPosition();
        }

        if (isMoving)
        {
            MoveCharacter();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "bucket" && !isMoving && !isWorking)
        {
            Bucket bucket = other.gameObject.GetComponent<Bucket>();
            StartCoroutine("ProcessBucketInteraction", bucket);
        }
    }

    IEnumerator ProcessBucketInteraction(Bucket bucket)
    {
        StartWorking();
        yield return new WaitForSeconds(workAnimationLength);
        StopWorking();
        _gameState.PickupBucket(bucket);
        yield return null;
    }

    private void StopWorking()
    {
        isWorking = false;
        _animator.SetBool("isWorking", false);
    }

    private void StartWorking()
    {
        isWorking = true;
        _animator.SetBool("isWorking", true);
    }

    private void StartMoving()
    {
        isMoving = true;
        _animator.SetBool("isRunning", true);
    }

    private void StopMoving()
    {
        isMoving = false;
        _animator.SetBool("isRunning", false);
    }

    private void MoveCharacter()
    {
        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            StopMoving();
        }

        Debug.DrawLine(transform.position, targetPosition, Color.red);
    }

    private void SetTargetPosition()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        float point = 0f;

        if (plane.Raycast(ray, out point))
        {
            targetPosition = ray.GetPoint(point);
        }

        StartMoving();
    }
}
