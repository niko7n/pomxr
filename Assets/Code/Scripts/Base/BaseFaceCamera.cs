using UnityEngine;

public class BaseFaceCamera : MonoBehaviour
{
    public void RotateTowardsCamera()
    {
        Vector3 direction = -(Camera.main.transform.position - transform.position);
        direction.y = 0;

        if (direction.sqrMagnitude > 0.0f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
