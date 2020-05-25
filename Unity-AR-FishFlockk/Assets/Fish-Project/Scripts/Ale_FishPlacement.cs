// Alesandra Miro Quesada
// Computational Form and Process End of Term 2
// 25 05 2020

// This script is responsible for generating the AR scene, interaction and placement of the flocking fish. Using ARFoundation 
// it uses a Raycast manager that is constantly 'scanning' the environment and saving the screen postion in a list. This list
// can then be used to place objects inside its diferent trackable object, in our case they are horizontal  and vertical planes. 
// As for the fish, a separte flocking script is placed on each fish gameObject and used to determine the position of the fish
// depending on each environment. The collaboration between both scripts is what creates the seemless movement of the digital fish
// in the real environment.

// For full code walkthrough visit: https://vimeo.com/422325242




using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class Ale_FishPlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placedFishPrefabs;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private float defaultRotation = 0;

    private GameObject placedFishObject;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private bool isLocked = false;

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //the distance that the fish will be able to travel
    public static int tankHeight = 10;
    public static int tankWidth = 10;
    public static int tankDepth = 10;

    static int numFish = 20;

    public static GameObject[] allFish = new GameObject[numFish];

    public static Vector3 goalPos = Vector3.zero; //setting it to the middle of the tank

    public GameObject shadowPlane;
    private GameObject placedShadowPlane;

    public GameObject portalObject;
    private GameObject placedPortalObject;

    public Transform player;



    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>(); //as soon as app is launched the Raycast manager starts to gather information about the real world
       
    }


    void Update()
    {
            
         // if(Input.GetButtonDown("Fire1")) //debugging with right mouse click instead of mobile touch
       // {

        if(Input.touchCount > 0) //detecting touch. If touch is greater than 0 get information from the Raycast to create the fish.
        {
            Touch touch = Input.GetTouch(0);
            
            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                // Ray ray = arCamera.ScreenPointToRay(Input.mousePosition); //again debugging by using mouse position indeat of mobile position
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if(Physics.Raycast(ray, out hitObject))
                {

                    //creating all of fish
                    for (int i = 0; i < numFish; i++)
                    {

                        //creating position for our fish  and creating the tank space
                        Vector3 pos = new Vector3(Random.Range(-tankWidth, tankWidth),
                                                  Random.Range(1, tankHeight),
                                                  Random.Range(-tankDepth, tankDepth));
                        allFish[i] = Instantiate(placedFishPrefabs[Random.Range(0, placedFishPrefabs.Length)], pos, Quaternion.identity); //create fish accoring to the tank size
                        allFish[i].GetComponent<flock>().init(player);
                    }

                }
            }

            //comment  this if statement out if you want to debug using the mouse
            if(touch.phase == TouchPhase.Ended)
            {
                onTouchHold = false;
            }
        }
        // here is where the magic happens. The Raycast manger takes the rouch position and the hits which have been stored in a list. 
        // Once it has the position from the list of Rays (hits) it uses a Trackable which in our case is a horizontal plane with polygons
        if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose; //if the user taps on screen Instantiate the following

             //placing AR portal
            if(placedPortalObject == null)
            {
                placedPortalObject = Instantiate(portalObject);
            }

            
            //placing shadow plane
            if (placedShadowPlane == null)
            {
                placedShadowPlane = Instantiate(shadowPlane, hitPose.position, Quaternion.identity); //This quaternion corresponds to "no rotation" - the object is perfectly aligned with the world or parent axes.

            }

            //placing the fish
            if(placedFishObject == null)
            {
                if(defaultRotation > 0)
                {
                    placedFishObject = Instantiate(placedFishPrefabs[0], hitPose.position, Quaternion.identity);
                    placedFishObject.transform.Rotate(Vector3.up, defaultRotation);
                }
                else 
                {
                    placedFishObject = Instantiate(placedFishPrefabs[0], hitPose.position, hitPose.rotation);
                }
            }
            else 
            {
                if(onTouchHold) // if you touch hold, one can rotate the fish
                {
                    placedFishObject.transform.position = hitPose.position;
                    if(defaultRotation == 0)
                    {
                        placedFishObject.transform.rotation = hitPose.rotation;
                    }
                }
            }
        }
    }
}
