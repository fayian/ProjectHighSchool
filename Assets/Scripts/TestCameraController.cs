using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraController : MonoBehaviour {
    public float speed = 1.0f;
    public float zoomSpeed = 0.7f;
    public float maxCameraSize = 8.0f;
    public float minCameraSize = 2.0f;

    private Camera camera;
    void Start() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        if (Input.GetKey(KeyCode.W)) transform.position += new Vector3(0.0f, 1.0f, 0.0f) * speed;
        if (Input.GetKey(KeyCode.A)) transform.position += new Vector3(-1.0f, 0.0f, 0.0f) * speed;
        if (Input.GetKey(KeyCode.S)) transform.position += new Vector3(0.0f, -1.0f, 0.0f) * speed;
        if (Input.GetKey(KeyCode.D)) transform.position += new Vector3(1.0f, 0.0f, 0.0f) * speed;

        camera.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        camera.orthographicSize = Mathf.Max(camera.orthographicSize, minCameraSize);
        camera.orthographicSize = Mathf.Min(camera.orthographicSize, maxCameraSize);
    }
}
