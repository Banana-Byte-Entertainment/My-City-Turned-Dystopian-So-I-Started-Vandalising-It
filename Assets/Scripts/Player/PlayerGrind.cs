using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.SocialPlatforms.Impl;

// https://github.com/SGTADMAN/RailGrind

public class PlayerGrind : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] bool jump;         //Inputs aren't used in the tutorial
    [SerializeField] Vector3 input;     //But they're here for rail switching

    [Header("Variables")]
    public bool onRail;
    [SerializeField] float grindSpeed = 20f;
    float heightOffset;
    float timeForFullSpline;
    float elapsedRailTime; // TRAVEL TIME ON RAIL; DO NOT USE FOR SCORE
    float elapsedScoreTime; // DELTA TIME ON RAIL; USE FOR SCORE
    float prevScoreTime; // PREV. DELTA TIME ON RAIL; USE FOR SCOREq
    [SerializeField] float lerpSpeed = 10f;

    [Header("Scripts")]
    [SerializeField] RailScript currentRailScript;
    PlayerScore playerScore;
    Rigidbody playerRigidbody;

    float3 nearestPoint;

    //
    private Quaternion currentRailRotation;
    private Vector3 lastTangent;
    private bool initializedRotation = false;
    //

    private void Start()
    {
        playerScore = GetComponent<PlayerScore>();
        playerRigidbody = GetComponent<Rigidbody>();
        heightOffset = gameObject.GetComponent<Collider>().bounds.size.y / 2 + gameObject.GetComponent<PlayerMovement>()._rideHeight;

        elapsedScoreTime = 0;
        prevScoreTime = 0;
    }

    private void FixedUpdate()
    {
        if (onRail) //If on the rail, move the player along the rail
        {
            GetComponent<PlayerMovement>().Freeze();
            playerRigidbody.useGravity = false;
            MovePlayerAlongRail();
            Quaternion rot = transform.rotation;
            rot.z = 0;
            transform.rotation = rot;

        }
        else
        {
            GetComponent<PlayerMovement>().Unfreeze();
            playerRigidbody.useGravity = true;
        }
    }

    private void Update()
    {

    }

    void MovePlayerAlongRail()
    {
        if (currentRailScript != null && onRail) //This is just some additional error checking.
        {
            Quaternion rot = transform.rotation;
            rot.z = 0;
            transform.rotation = rot;
            //Calculate a 0 to 1 normalised time value which is the progress along the rail.
            //Elapsed time divided by the full time needed to traverse the spline will give you that value.
            float progress = elapsedRailTime / timeForFullSpline;

            playerScore.UpdateScore(Math.Abs(elapsedScoreTime - prevScoreTime) * 100);

            //If progress is less than 0, the player's position is before the start of the rail.
            //If greater than 1, their position is after the end of the rail.
            //In either case, the player has finished their grind.
            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }
            //The rest of this code will not execute if the player is thrown off.

            //Next Time Normalised is the player's progress value for the next update.
            //This is used for calculating the player's rotation.
            //Depending on the direction of the player on the spline, it will either add or subtract time from the
            //current elapsed time.
            float nextTimeNormalised;
            if (currentRailScript.travelDir)
                nextTimeNormalised = (elapsedRailTime + Time.deltaTime) / timeForFullSpline;
            else
                nextTimeNormalised = (elapsedRailTime - Time.deltaTime) / timeForFullSpline;

            //Calculating the local positions of the player's current position and next position
            //using current progress and the progress for the next update.
            float3 pos, tangent, up;
            float3 nextPosfloat, nextTan, nextUp;
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

            //Converting the local positions into world positions.
            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
            Vector3 nextPos = currentRailScript.LocalToWorldConversion(nextPosfloat);

            //Setting the player's position and adding a height offset so that they're sitting on top of the rail
            //instead of being in the middle of it.
            transform.position = worldPos + (transform.up * heightOffset);
            // //Lerping the player's current rotation to the direction of where they are to where they're going.
            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.deltaTime);
            // //Lerping the player's up direction to match that of the rail, in relation to the player's current rotation.
            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerpSpeed * Time.deltaTime);

            Vector3 tangentDir = (nextPos - worldPos).normalized;

            if (!initializedRotation)
            {
                // First frame on the rail: build initial rotation
                currentRailRotation = Quaternion.LookRotation(tangentDir, transform.up);
                initializedRotation = true;
            }
            else
            {
                // Compute rotation from previous tangent to current tangent
                Quaternion rotationDelta = Quaternion.FromToRotation(lastTangent, tangentDir);
                currentRailRotation = rotationDelta * currentRailRotation;
            }

            currentRailRotation.z = 0;

            // Apply rotation smoothly
            transform.rotation = Quaternion.Slerp(transform.rotation, currentRailRotation, lerpSpeed * Time.deltaTime);
            // Zero out the z component of the rotation (roll) if needed
            Vector3 currentEulerAngles = transform.rotation.eulerAngles;
            currentEulerAngles.z = 0;
            transform.rotation = Quaternion.Euler(currentEulerAngles);

            // Update tangent
            lastTangent = tangentDir;


            //Finally incrementing or decrementing elapsed time for the next update based on direction.
            prevScoreTime = elapsedScoreTime;
            elapsedScoreTime += Time.deltaTime;

            if (currentRailScript.travelDir)
                elapsedRailTime += Time.deltaTime;
            else
                elapsedRailTime -= Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision) // OnCollisionEnter is required to prevent score breaking (ensure correct elapsedScoreTime)
    {
        Debug.Log("Hitting something");
        if (onRail)
        {
            // ThrowOffRail();
            transform.RotateAround(transform.position, transform.up, 180f);
            Quaternion rot = transform.rotation;
            rot.z = 0;
            transform.rotation = rot;
            CalculateAndSetRailPosition();
        }
        else if (collision.gameObject.CompareTag("Rail"))
        {
            Debug.Log("Hitting rail");
            Spline rail = collision.gameObject.GetComponentInParent<SplineContainer>().Spline;
            float3 playerPoint = collision.gameObject.GetComponent<RailScript>().WorldToLocalConversion(transform.position);
            SplineUtility.GetNearestPoint(rail, playerPoint, out nearestPoint, out float normalizedPoint, 10, 10);
            nearestPoint = collision.gameObject.GetComponent<RailScript>().LocalToWorldConversion(nearestPoint);

            if (transform.position.y > nearestPoint.y)
            {
                /*When the player hits the rail, onRail is set to true, the current rail script is set to the
                *rail script of the rail the player hits. Then we calculate the player's position on that rail.
                */
                onRail = true;
                elapsedScoreTime = Time.deltaTime;
                prevScoreTime = Time.deltaTime;
                currentRailScript = collision.gameObject.GetComponent<RailScript>();
                CalculateAndSetRailPosition();
                Quaternion rot = transform.rotation;
                rot.z = 0;
                transform.rotation = rot;
            }
        }
    }

    // void OnDrawGizmos()
    // {
    //     Debug.Log("Drawing gizmo at " + nearestPoint);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(nearestPoint, 1f);
    // }

    public void CalculateAndSetRailPosition()
    {
        initializedRotation = false;
        //Figure out the amount of time it would take for the player to cover the rail.
        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;

        //This is going to be the world position of where the player is going to start on the rail.
        Vector3 splinePoint;

        //The 0 to 1 value of the player's position on the spline. We also get the world position of where that
        //point is.
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedRailTime = timeForFullSpline * normalisedTime;
        //Multiply the full time for the spline by the normalised time to get elapsed time. This will be used in
        //the movement code.

        //Spline evaluate takes the 0 to 1 normalised time above, 
        //and uses it to give you a local position, a tangent (forward), and up
        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);
        //Calculate the direction the player is going down the rail
        currentRailScript.CalculateDirection(forward, transform.forward);
        //Set player's initial position on the rail before starting the movement code.
        transform.position = splinePoint + (transform.up * heightOffset);
        Quaternion rot = transform.rotation;
        rot.z = 0;
        transform.rotation = rot;
    }

    void ThrowOffRail() //ALWAYS CALL WHEN PLAYER COMES OFF RAIL; NEEDED FOR SCORE RESET TOO
    {
        //Set onRail to false, clear the rail script, and push the player off the rail.
        //It's a little sudden, there might be a better way of doing using coroutines and looping, but this will work.
        onRail = false;
        currentRailScript = null;
        transform.position += transform.forward * 1;

        //Reset time-count for scores
        elapsedScoreTime = 0;
        prevScoreTime = 0;

        initializedRotation = false;
    }

    public void FeetCollisionOnRail()
    {
        ThrowOffRail();
        Quaternion rot = transform.rotation;
        rot.z = 0;
        transform.rotation = rot;
    }

    public void RideRail(GameObject gameObject)
    {
        currentRailScript = gameObject.GetComponent<RailScript>();
        MovePlayerAlongRail();
    }
}
