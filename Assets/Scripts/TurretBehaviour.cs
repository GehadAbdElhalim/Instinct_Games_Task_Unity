using System.Collections;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    public float bulletSpeed;

    public float rotationSpeed;
    public float angleOfView;
    public float focusRadius;
    [SerializeField] Transform rotatingHeadTransform;
    [SerializeField] Transform raycastStartPoint;
    [SerializeField] Transform bulletSpwanPoint;

    LineRenderer lr;

    public bool playerSeen = false;

    bool rotatingRight = true;

    bool rotatingToShoot = false;

    public float shotDelay;

    float d1, d2, d3;
    float t1, t2, t3;
    float v1, v2, w;
    float theta;

    Vector3 predictedPos;

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
            if (!rotatingToShoot)
            {
                rotatingToShoot = true;
                Vector3 pos = PredictNextPlayerPosiion();
                predictedPos = pos;
                StartCoroutine(ShootAProjectileAtPosition(pos));
            }
        }
        else
        {
            if (!rotatingToShoot)
            {
                UpdateRotation();
                playerSeen = FoundAPlayer();
            }
        }
    }

    private void LateUpdate()
    {
        UpdateLineRenderer();
    }

    bool IsPlayerOutOfRange()
    {
        return Vector3.Distance(GameManager.instance.player.transform.position, new Vector3(transform.position.x, GameManager.instance.player.transform.position.y, transform.position.z)) > focusRadius;
    }

    Vector3 PredictNextPlayerPosiion()
    {
        GameObject player = GameManager.instance.player;

        t1 = shotDelay;
        v1 = player.GetComponent<Rigidbody>().velocity.magnitude;
        d1 = v1 / t1;

        Vector3 movementDir = player.GetComponent<Rigidbody>().velocity.normalized;

        return player.transform.position + movementDir * d1;
    }

    IEnumerator ShootAProjectileAtPosition(Vector3 targetPos)
    {
        GameObject player = GameManager.instance.player;

        d2 = Vector3.Distance(targetPos, rotatingHeadTransform.position);
        d3 = Vector3.Distance(player.transform.position, rotatingHeadTransform.position);

        theta = Mathf.Acos((d2 * d2 + d3 * d3 - d1 * d1) / (2 * d2 * d3));

        v2 = bulletSpeed;

        float distanceBetweenBulletAndTargetPos = d2 - Vector3.Distance(rotatingHeadTransform.position, bulletSpwanPoint.position);

        t3 = v2 / distanceBetweenBulletAndTargetPos;

        t2 = t1 - t3;

        w = theta / t2;

        float angleRotated = 0;

        while (angleRotated < theta)
        {
            print(w);
            rotatingHeadTransform.Rotate(Vector3.up, w);
            angleRotated += w;
            yield return new WaitForFixedUpdate();
        }

        rotatingToShoot = false;
    }

    void UpdateRotation()
    {
        if (rotatingRight)
        {
            //rotatingHeadTransform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
            rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, rotatingHeadTransform.localEulerAngles.y + rotationSpeed * Time.fixedDeltaTime, rotatingHeadTransform.localEulerAngles.z);

            if (ChangeAngleToPositiveAndNegative(rotatingHeadTransform.localEulerAngles.y) > angleOfView)
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
        RaycastHit hit;

        if (Physics.Raycast(raycastStartPoint.position, raycastStartPoint.forward, out hit))
        {
            if (hit.collider.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }

    void UpdateLineRenderer()
    {
        RaycastHit hit;

        Vector3[] vertices = new Vector3[2];

        if (Physics.Raycast(raycastStartPoint.position, raycastStartPoint.forward, out hit))
        {
            vertices[0] = raycastStartPoint.position;
            vertices[1] = hit.point;
        }
        else
        {
            vertices[0] = raycastStartPoint.position;
            vertices[1] = raycastStartPoint.position + raycastStartPoint.forward * focusRadius;
        }

        lr.SetPositions(vertices);
    }

    float ChangeAngleToPositiveAndNegative(float angle)
    {
        if (angle <= 180)
            return angle;

        return ChangeAngleToPositiveAndNegative(angle - 360);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(predictedPos, 0.5f);
    }
}