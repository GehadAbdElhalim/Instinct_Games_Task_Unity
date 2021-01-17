using System;
using System.Collections;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    public float rotationSpeed_Idle;
    public float maxYAngle_Idle;
    public float focusRadius;

    [SerializeField] Transform rotatingHeadTransform;
    [SerializeField] Transform raycastStartPoint;
    [SerializeField] Transform bulletSpwanPoint;
    [SerializeField] GameObject bullet;

    public LayerMask raycastLayers;

    //Components
    LineRenderer lr;

    //private variables
    TurretState state;
    float startingXAngle;
    bool rotatingToShoot = false;
    bool rotatingRight = true;

    //Variables for predicting the player location and shooting towards it
    float shotDelay = 1;
    float d1, d2, d3;
    float t1, t2, t3;
    float v1, v2, w;
    float theta;

    //Variables for Gizmos
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

        startingXAngle = rotatingHeadTransform.localEulerAngles.x;

        //rotatingHeadTransform.localEulerAngles = new Vector3(xAngle, 0, 0);
    }

    private void FixedUpdate()
    {
        if (state == TurretState.Idle)
        {
            ReturnToRange();
            UpdateRotation();

            if (FoundAPlayer())
            {
                state = TurretState.PlayerInRange;
            }
        }

        if (state == TurretState.PlayerInRange)
        {
            if (!rotatingToShoot)
            {
                rotatingToShoot = true;
                Vector3 pos = PredictNextPlayerPosiion();
                predictedPos = pos;
                StartCoroutine(RotateAtPositionAndShoot(pos));
            }

            if (IsPlayerOutOfRange())
            {
                state = TurretState.Idle;
            }
        }
    }

    private void LockLaserOnPlayer()
    {
        raycastStartPoint.LookAt(GameManager.instance.player.transform.position);
    }

    private void ReturnToRange()
    {
        if (rotatingHeadTransform.transform.localEulerAngles.x == startingXAngle)
        {
            return;
        }

        float currentXAngle = Mathf.Lerp(rotatingHeadTransform.localEulerAngles.x, startingXAngle, Time.fixedDeltaTime * 5);

        if (Mathf.Abs(currentXAngle - startingXAngle) < 0.5f)
        {
            currentXAngle = startingXAngle;
        }

        rotatingHeadTransform.transform.localEulerAngles = new Vector3(currentXAngle, rotatingHeadTransform.transform.localEulerAngles.y, rotatingHeadTransform.transform.localEulerAngles.z);
        raycastStartPoint.forward = rotatingHeadTransform.forward;
    }

    private void LateUpdate()
    {
        if (state == TurretState.PlayerInRange || rotatingToShoot)
        {
            LockLaserOnPlayer();
        }

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

        //return FirstOrderIntercept(bulletSpwanPoint.transform.position, Vector3.zero, bullet.GetComponent<BulletBehaviour>().speed, player.transform.position, player.GetComponent<Rigidbody>().velocity);
    }

    #region unused functions
    //first-order intercept using absolute target position
    public Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);
        return targetPosition + t * (targetRelativeVelocity);
    }
    //first-order intercept using relative target position
    public float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
    #endregion

    IEnumerator RotateAtPositionAndShoot(Vector3 targetPos)
    {
        GameObject player = GameManager.instance.player;

        d2 = Vector3.Distance(targetPos, bulletSpwanPoint.position);
        d3 = Vector3.Distance(player.transform.position, bulletSpwanPoint.position);

        theta = Mathf.Acos((d2 * d2 + d3 * d3 - d1 * d1) / (2 * d2 * d3));

        v2 = bullet.GetComponent<BulletBehaviour>().speed * Time.fixedDeltaTime;

        float distanceBetweenBulletAndTargetPos = d2 - Vector3.Distance(rotatingHeadTransform.position, bulletSpwanPoint.position);

        t3 = distanceBetweenBulletAndTargetPos / v2;

        t2 = t1 - t3;

        w = theta / t2;

        Vector3 targetVector = targetPos - rotatingHeadTransform.position;

        //rotatingHeadTransform.forward = Vector3.Lerp(rotatingHeadTransform.forward, targetVector, w);

        if (w > 0.001 && v1 > 0.001f)
        {
            float angleRotated = 0;

            while (angleRotated < theta)
            {
                //rotatingHeadTransform.Rotate(rotatingHeadTransform.transform.up, w);
                //rotatingHeadTransform.forward = Vector3.MoveTowards(rotatingHeadTransform.forward, targetVector, 0.1f);
                rotatingHeadTransform.forward = Vector3.Lerp(rotatingHeadTransform.forward, targetVector, w * Time.fixedDeltaTime);
                angleRotated += w * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            //rotatingHeadTransform.transform.forward = targetVector;

            float timer = 0;
            while (Mathf.Abs(Vector3.Angle(rotatingHeadTransform.transform.forward, targetVector)) > 3f)
            {
                rotatingHeadTransform.transform.forward = Vector3.Lerp(rotatingHeadTransform.transform.forward,targetVector, 2 * Time.fixedDeltaTime);
                timer += Time.fixedDeltaTime;
                if(timer > 2f)
                {
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            rotatingHeadTransform.transform.forward = targetVector;
        }

        Invoke("FalsifyRotatingToShoot", 1f);

        ShootProjectile();
    }

    void FalsifyRotatingToShoot()
    {
        rotatingToShoot = false;
    }

    void ShootProjectile()
    {
        //Instantiate(bullet, bulletSpwanPoint.position, bulletSpwanPoint.rotation);
        ObjectPooler.instance.SpawnFromPool("Bullet", bulletSpwanPoint.position, bulletSpwanPoint.rotation);
    }

    void UpdateRotation()
    {
        if (rotatingRight)
        {
            //rotatingHeadTransform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
            rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, rotatingHeadTransform.localEulerAngles.y + rotationSpeed_Idle * Time.fixedDeltaTime, rotatingHeadTransform.localEulerAngles.z);

            if (ChangeAngleToPositiveAndNegative(rotatingHeadTransform.localEulerAngles.y) > maxYAngle_Idle)
            {
                //rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, angleOfView, rotatingHeadTransform.localEulerAngles.z);
                rotatingRight = false;
            }
        }
        else
        {
            //rotatingHeadTransform.Rotate(Vector3.up, -rotationSpeed * Time.fixedDeltaTime);
            rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, rotatingHeadTransform.localEulerAngles.y - rotationSpeed_Idle * Time.fixedDeltaTime, rotatingHeadTransform.localEulerAngles.z);

            if (ChangeAngleToPositiveAndNegative(rotatingHeadTransform.localEulerAngles.y) < -maxYAngle_Idle)
            {
                //rotatingHeadTransform.localEulerAngles = new Vector3(rotatingHeadTransform.localEulerAngles.x, -angleOfView, rotatingHeadTransform.localEulerAngles.z);
                rotatingRight = true;
            }
        }
    }

    bool FoundAPlayer()
    {
        RaycastHit hit;

        if (Physics.Raycast(raycastStartPoint.position, raycastStartPoint.forward, out hit, 1000f, raycastLayers))
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

        if (Physics.Raycast(raycastStartPoint.position, raycastStartPoint.forward, out hit, 1000f, raycastLayers))
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
        Gizmos.DrawWireSphere(predictedPos, 0.5f);
    }
}

public enum TurretState
{
    Idle,
    PlayerInRange,
}