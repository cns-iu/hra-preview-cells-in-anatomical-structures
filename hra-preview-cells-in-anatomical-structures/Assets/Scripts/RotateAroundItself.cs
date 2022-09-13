using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundItself : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(0f, 35f * Time.deltaTime, 0f), Space.World);
    }
}
