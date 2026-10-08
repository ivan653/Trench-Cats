using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(0.07f, 0);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-0.07f, 0);
        }
    }
}