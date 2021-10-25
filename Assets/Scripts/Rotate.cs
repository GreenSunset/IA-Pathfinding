using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float RotationSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,RotationSpeed * Time.deltaTime);
    }

    public void PointNorth()
    {
        transform.localRotation = new Quaternion(0, 0, 0, transform.localRotation.w);
    }
    public void PointSouth()
    {
        transform.localRotation = new Quaternion(0, 180, 0, transform.localRotation.w);
    }
    public void PointEast()
    {
        transform.localRotation = new Quaternion(0, 90, 0, transform.localRotation.w);
    }
    public void PointWest()
    {
        transform.localRotation = new Quaternion(0, 270, 0, transform.localRotation.w);
    }
    public void PointNorthEast()
    {
        transform.localRotation = new Quaternion(0, 45, 0, transform.localRotation.w);
    }
    public void PointSouthEast()
    {
        transform.localRotation = new Quaternion(0, 135, 0, transform.localRotation.w);
    }
    public void PointNorthWest()
    {
        transform.localRotation = new Quaternion(0, 315, 0, transform.localRotation.w);
    }
    public void PointSouthWest()
    {
        transform.localRotation = new Quaternion(0, 225, 0, transform.localRotation.w);
    }
}
