using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5f;
    public float scrollSpeed = 5f;
    public float screenBorderOffset = 10f;
    public Vector3 cameraOffset;

    public Vector2 minEdgeLimit = new Vector2(-50f, -10f);
    public Vector2 maxEdgeLimit = new Vector2(50f, 100f);
    public float minZoom = 5f;
    public float maxZoom = 50f;

    public GameObject soldier;
    public bool shouldFollowTarget = false;
    public float timeBeforeResetCamera = 5f;

    [Range(0f, 1f)] //should be between 0 and 1
    public float smoothSpeed = 0.25f;

    private IEnumerator coroutine;
    private Vector3 newPosition;
    private float mousePos;
    private float scroll;
    private float speed;
    private Camera cam;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        cameraOffset = new Vector3(0, 0, -10);
        newPosition = cameraOffset;

        //normalize speeds to be around the same value
        cameraSpeed /= 1000f;
        scrollSpeed *= 100f;
    }

    void FixedUpdate()
    {
        if (shouldFollowTarget || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            shouldFollowTarget = true;
            FollowTarget(soldier);
        }

        HandleZoom();
        FollowMouse();
    }



    public void FollowTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        //normalize speed
        speed = smoothSpeed * 10f * Time.deltaTime;
        newPosition = Vector3.Lerp(transform.position, target.transform.position + cameraOffset, speed);
        transform.position = newPosition;
    }
    private void HandleZoom()
    {
        scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * scrollSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    private void FollowMouse()
    {

        //get mouse x postion to stop calling Input.etc multiple times
        mousePos = Input.mousePosition.x;

        //Normalize speed depending on time and zoom level
        speed = cameraSpeed * cam.orthographicSize * Time.deltaTime;

        if (mousePos > Screen.width - screenBorderOffset)
        {
            //increase camera position on x by a fixed amount every frame 
            //depending on the mosude distance from the screen edge
            //so the farther out the mouse is, the faster the camera will move
            newPosition.x += speed * (mousePos - (Screen.width - screenBorderOffset));
        }
        else if (mousePos < screenBorderOffset)
        {
            newPosition.x -= speed * (-(mousePos + screenBorderOffset));
        }


        mousePos = Input.mousePosition.y;

        if (mousePos > Screen.height - screenBorderOffset)
        {
            newPosition.y += speed * (mousePos - (Screen.height - screenBorderOffset));
        }
        else if (mousePos < screenBorderOffset)
        {
            newPosition.y -= speed * (-(mousePos + screenBorderOffset));
        }


        newPosition.x = Mathf.Clamp(newPosition.x, minEdgeLimit.x, maxEdgeLimit.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minEdgeLimit.y, maxEdgeLimit.y);

        if(transform.position != newPosition)
        {
            shouldFollowTarget = false;
            transform.position = newPosition;
            if(coroutine != null)
            {
                StopCoroutine(coroutine); //stop previous coroutine
            }
            coroutine = FollowTargetIn(timeBeforeResetCamera); 
            StartCoroutine(coroutine); //start new timer

        }
    }

    public IEnumerator FollowTargetIn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        shouldFollowTarget = true;
    }
}

