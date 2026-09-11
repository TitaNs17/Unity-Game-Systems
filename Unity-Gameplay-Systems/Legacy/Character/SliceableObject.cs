using System;
using UnityEngine;

public class SliceableObject : MonoBehaviour
{
    [SerializeField, Min(1)] private int totalSlices = 10;
    [SerializeField, Range(0.1f, 1f)] private float shrinkFactor = 0.9f;
    [SerializeField] private float sliceOffset = 0.3f;
    [SerializeField] private GameObject slicePrefab;
    [SerializeField] private Transform sliceSpawnPoint;
    [SerializeField] private bool destroyWhenFinished = true;

    private int remainingSlices;

    public int RemainingSlices => remainingSlices;
    public bool IsFinished => remainingSlices <= 0;

    public event Action<int> Sliced;
    public event Action Finished;

    private void Awake()
    {
        remainingSlices = Mathf.Max(1, totalSlices);
    }

    public bool SliceOnce()
    {
        if (IsFinished)
            return false;

        SpawnSlice();
        remainingSlices--;

        if (remainingSlices > 0)
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y,
                Mathf.Max(0.01f, transform.localScale.z * shrinkFactor));

            transform.position += transform.forward * sliceOffset * 0.5f;
            Sliced?.Invoke(remainingSlices);
            return true;
        }

        Sliced?.Invoke(0);
        Finished?.Invoke();

        if (destroyWhenFinished)
            Destroy(gameObject);

        return true;
    }

    public void Slice()
    {
        SliceOnce();
    }

    public void ResetSlices()
    {
        remainingSlices = Mathf.Max(1, totalSlices);
    }

    private void SpawnSlice()
    {
        if (slicePrefab == null)
            return;

        var spawn = sliceSpawnPoint != null ? sliceSpawnPoint : transform;
        var instance = Instantiate(slicePrefab, spawn.position, spawn.rotation);

        var body = instance.GetComponent<Rigidbody>();
        if (body != null)
            body.AddForce(transform.forward * 0.25f, ForceMode.Impulse);
    }

    private void OnValidate()
    {
        totalSlices = Mathf.Max(1, totalSlices);
        shrinkFactor = Mathf.Clamp(shrinkFactor, 0.1f, 1f);
    }
}
