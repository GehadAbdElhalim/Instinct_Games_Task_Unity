using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    public float rotationSpeed;
    public float angleOfView;
    public float focusRadius;
    [SerializeField] Transform rotatingHeadTransform;
    [SerializeField] Transform raycastStartPos;

    LineRenderer lr;

    bool playerSeen = false;

    bool rotatingRight = true;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        SetPitchOfRotatingHead();
    }

    void SetPitchOfRotatingHead()
    {
        Vector3 endPoint = new Vector3(transform.position.x, 0, transform.position.z) + transform.forward * focusRadius;
        //Vector3 forward = (endPoint - rotatingHeadTransform.position).normalized;

        //float xAngle = Mathf.Asin(focusRadius / distanceBetweenHeadandEndPoint);

        rotatingHeadTransform.LookAt(endPoint);

        //rotatingHeadTransform.localEulerAngles = new Vector3(xAngle, 0, 0);
    }

    private void FixedUpdate()
    {
        if (playerSeen)
        {
            playerSeen = !IsPlayerOutOfRange();
        }

        if (playerSeen)
        {
            Vector3 pos = PredictNextPlayerPosiion();
            ShootAProjectileAtPosition(pos);
        }
        else
        {
            UpdateRotation();
            playerSeen = FoundAPlayer();
        }
    }

    private void LateUpdate()
    {
        UpdateLineRenderer();
    }

    bool IsPlayerOutOfRange()
    {
        return false;
    }

    Vector3 PredictNextPlayerPosiion()
    {
        return Vector3.zero;
    }

    void ShootAProjectileAtPosition(Vector3 pos)
    {

    }

    void UpdateRotation()
    {
        if (rotatingRight)
        {
            //rotatingHeadTransform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
            rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, rotatingHeadTransform.localEulerAngles.y + rotationSpeed * Time.fixedDeltaTime, rotatingHeadTransform.localEulerAngles.z);

            if(ChangeAngleToPositiveAndNegative(rotatingHeadTransform.localEulerAngles.y) > angleOfView)
            {
                rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, angleOfView, rotatingHeadTransform.localEulerAngles.z);
                rotatingRight = false;
            }
        }
        else
        {
            //rotatingHeadTransform.Rotate(Vector3.up, -rotationSpeed * Time.fixedDeltaTime);
            rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, rotatingHeadTransform.localEulerAngles.y - rotationSpeed * Time.fixedDeltaTime, rotatingHeadTransform.localEulerAngles.z);

            if (ChangeAngleToPositiveAndNegative(rotatingHeadTransform.localEulerAngles.y) < -angleOfView)
            {
                rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, -angleOfView, rotatingHeadTransform.localEulerAngles.z);
                rotatingRight = true;
            }
        }
    }

    bool FoundAPlayer()
    {
        return false;
    }

    void UpdateLineRenderer()
    {
        RaycastHit hit;

        Vector3[] vertices = new Vector3[2];

        if (Physics.Raycast(raycastStartPos.position, raycastStartPos.forward, out hit))
        {
            vertices[0] = raycastStartPos.position;
            vertices[1] = hit.point;
        }
        else
        {
            vertices[0] = raycastStartPos.position;
            vertices[1] = raycastStartPos.position + raycastStartPos.forward * focusRadius;
        }

        lr.SetPositions(vertices);
    }

    float ChangeAngleToPositiveAndNegative(float angle)
    {
        if (angle <= 180)
            return angle;

        return ChangeAngleToPositiveAndNegative(angle - 360);
    }
}
