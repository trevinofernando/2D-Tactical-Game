using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5f;
    public float scrollSpeed = 5f;
    public float screenBorderOffset = 10f;
    public Vector3 cameraOffset = new Vector3(0, 0, -10);

    public float minZoom = 5f;
    public float maxZoom = 60f;
    private Vector2 minEdgeLimit;
    private Vector2 maxEdgeLimit;

    public GameObject soldier;
    public bool shouldFollowTarget = false;
    public float timeBeforeResetCamera = 10f;

    [Range(0f, 1f)] //should be between 0 and 1
    public float smoothSpeed = 0.35f;
    
    public static CameraController instance;

    private IEnumerator coroutine;
    private Vector3 newPosition;
    private float mousePos;
    private float scroll;
    private float speed;
    private Camera cam;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        cam = gameObject.GetComponent<Camera>();
        transform.position = cameraOffset;
        newPosition = cameraOffset;
        minEdgeLimit = new Vector2(-25f, -15f);
        maxEdgeLimit = new Vector2(GlobalVariables.Instance.mapXMax + 30f, GlobalVariables.Instance.mapYMax + 25f);

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
        FollowMouse(); //if out of the screen
    }



    public void FollowTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        //normalize speed
        speed = smoothSpeed * 10f * Time.deltaTime;
        //move a fraction (speed) of the way between the current location and de desired location
        newPosition = Vector3.Lerp(transform.position, target.transform.position + cameraOffset, speed);
        newPosition.z = cameraOffset.z;
        transform.position = newPosition;
    }
    private void HandleZoom()
    {
        //get direction and amount to zoom
        scroll = Input.GetAxis("Mouse ScrollWheel");
        //Calculate new size
        cam.orthographicSize -= scroll * scrollSpeed * Time.deltaTime;
        //bound the size between min and max zoom
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
            //depending on the mouse distance from the screen edge
            //so the farther out the mouse is, the faster the camera will move
            newPosition.x += speed * (mousePos - (Screen.width - screenBorderOffset));
        }
        else if (mousePos < screenBorderOffset)
        {
            newPosition.x -= speed * (screenBorderOffset + Mathf.Abs(mousePos));
        }

        //reuse mousePos for y input
        mousePos = Input.mousePosition.y;

        if (mousePos > Screen.height - screenBorderOffset)
        {
            newPosition.y += speed * (mousePos - (Screen.height - screenBorderOffset));
        }
        else if (mousePos < screenBorderOffset)
        {
            newPosition.y -= speed * (screenBorderOffset + Mathf.Abs(mousePos));
        }

        //set the x and y boundaries between min and max edge limits
        newPosition.x = Mathf.Clamp(newPosition.x, minEdgeLimit.x, maxEdgeLimit.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minEdgeLimit.y, maxEdgeLimit.y);

        //if position changed
        if(transform.position != newPosition)
        {
            //stop following target
            shouldFollowTarget = false;

            //move to new location
            transform.position = newPosition;
            
            //stop any ongoing coroutine
            if(coroutine != null)
            {
                StopCoroutine(coroutine); //stop previous coroutine
            }

            //set and start a new coroutine to reset the camera back to the player's position
            coroutine = FollowTargetIn(timeBeforeResetCamera); 
            StartCoroutine(coroutine); //start new timer
        }
    }

    public IEnumerator FollowTargetIn(float waitTime)
    {
        //wait about 5 seconds to reset camera to following the player again
        yield return new WaitForSeconds(waitTime);
        //change camera state to following again
        shouldFollowTarget = true;
    }

    public void SetZoom(float zoomAmount){
        //bound the size between min and max zoom
        zoomAmount = Mathf.Clamp(zoomAmount, minZoom, maxZoom);
        //Set new size
        cam.orthographicSize = zoomAmount;
    }

    public void SetCameraOffset(Vector2 offset){
        cameraOffset = new Vector3(offset.x, offset.y, cameraOffset.z);
    }
}

