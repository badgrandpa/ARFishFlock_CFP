// Alesandra Miro Quesada
// Computational Form and Process End of Term 2
// 25 05 2020

// This is my main flock algorithm which I translated from OpenFrameworks into Unity. 
// Its responsible for controlling and animating the fish objects. This script should be attached to the object
// one wished to flock.
// For full code walkthrough visit: https://vimeo.com/422325242


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flock : MonoBehaviour
{

    public float speed = 0.5f;
    public float maxSpeed = 5f;
    float rotationSpeed = 3.0f; //how fast fish will turn
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighbourDistance = 2.0f; //max distance fish need to be so that they flock
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        //makes fish more realistic with a bit of randomness
        speed = Random.Range(0.5f, 1);
    }

     public void init(Transform player)
    {
        this.player = player;
    }

    // Update is called once per frame
    void Update()
    {

        //Has the fish gone outside the current bounds?
        if(transform.position.x > Ale_FishPlacement.tankWidth || transform.position.x < -Ale_FishPlacement.tankWidth ||
            transform.position.y > Ale_FishPlacement.tankHeight || transform.position.y < -Ale_FishPlacement.tankHeight ||
            transform.position.z > Ale_FishPlacement.tankDepth || transform.position.z < -Ale_FishPlacement.tankDepth)
        {
            transform.LookAt(player);
        }

        if (Random.Range(0, 5) < 1)
        {
            ApplyRules();
        }

        if(speed > maxSpeed)
        {
            speed = maxSpeed;
        }  

        transform.Translate(0, 0, Time.deltaTime * speed);  
        
    }

    
    void ApplyRules() { //applied the flocking rules


        GameObject[] gos; //gos stands from game object
        gos = Ale_FishPlacement.allFish; //it needs to know the position of all the fish in the array

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero; //this is important for realism
        float gSpeed = 0.1f; //group speed

        Vector3 goalPos = Ale_FishPlacement.goalPos; //talking to the placement script

        float dist;

        int groupSize = 0; //based on who is within the neighbour distance 

        //calculating group size
        foreach (GameObject go in gos) {

            if (go != this.gameObject) {

                //choosing neighbours
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance) {

                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f) { // if distance is too small we want to avoid

                        vavoid = vavoid + (this.transform.position - go.transform.position);

                    }

                    //grabbing the flock script and using the speed of neighbouring fish and adding it to speed total for average speed
                    flock anotherFlock = go.GetComponent<flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }

            }

        }

        //if we are in a group then we can calculate the average centre of group and average speed
        if (groupSize > 0) {

            vcentre = vcentre / groupSize + (goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vcentre + vavoid) - transform.position; // this will give us the direction the fish need to turn into
            if (direction != Vector3.zero) //if direction is not equal to 0 fish will change direction
                transform.rotation = Quaternion.Slerp(transform.rotation, //slerp will slowly turn us from one direction to the new one
                                                      Quaternion.LookRotation(direction),
                                                      rotationSpeed * Time.deltaTime);

        }

    }
}