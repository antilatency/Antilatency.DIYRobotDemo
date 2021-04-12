using UnityEngine;

public static class CustomMaths
{
    public struct WheelsSpeed
    {
        public float LeftSpeed;
        public float RightSpeed;
    }

    public static void SetEpsilon(float value)=>
        _e = value;
    
    private static float _e = .25f;
    public static WheelsSpeed GetEfficientSpeeds(Transform tank, Transform target, bool drawGizmos = false)
    {
        var forward1 = tank.forward;
        Vector2 forward = new Vector2(forward1.x, forward1.z);
        forward.Normalize();

        var right1 = tank.right;
        Vector2 right = new Vector2(right1.x, right1.z);
        right.Normalize();

        var position = tank.position;
        Vector2 tankPosition = new Vector2(position.x, position.z);
        
        //float e = 0.25f;
        Vector2 eVector = _e * forward;
        Vector2 manipulatorPosition = tankPosition + eVector;
        var position1 = target.position;
        Vector2 targetPosition = new Vector2(position1.x, position1.z);
        float wHalf = .2f;

        Vector2 tau = (targetPosition - manipulatorPosition).normalized;

        float k = Vector2.Dot(eVector.normalized, tau);
        float k2 = k * k;
        float radius = Mathf.Sqrt(k2 * _e * _e / (1 - k2));
        float sqrCurvature = (1 - k2) / (k2 * _e * _e);
        if (sqrCurvature < 0)
            sqrCurvature = 0;

        float curvature = Mathf.Sqrt(sqrCurvature);

        float tauLocalX = Vector2.Dot(tau, right);
        float tauLocalZ = Vector2.Dot(tau, forward);

        float leftVelocity = 0;
        float rightVelocity = 0;

        if (tauLocalZ > 0 && tauLocalX > 0) {
            leftVelocity = 1 + wHalf * curvature;
            rightVelocity = 1 - wHalf * curvature;
        }

        if (tauLocalZ > 0 && tauLocalX <= 0) {
            radius = -radius;
            curvature = -curvature;
            leftVelocity = 1 + wHalf * curvature;
            rightVelocity = 1 - wHalf * curvature;
        }

        if (tauLocalZ < 0 && tauLocalX > 0) {
            radius = -radius;
            curvature = -curvature;
            leftVelocity = -(1 + wHalf * curvature);
            rightVelocity = -(1 - wHalf * curvature);
        }

        if (tauLocalZ < 0 && tauLocalX <= 0)
        {
            leftVelocity = -(1 + wHalf * curvature);
            rightVelocity = -(1 - wHalf * curvature);
        }
        

        float absLeftVelocity = Mathf.Abs(leftVelocity);
        float absRightVelocity = Mathf.Abs(rightVelocity);
        float absMaxVelocity = Mathf.Max(absLeftVelocity, absRightVelocity);

        float maxVelocity = 1;
        if (absMaxVelocity > maxVelocity) {
            float scale = maxVelocity / absMaxVelocity;
            leftVelocity *= scale;
            rightVelocity *= scale;
        }
        
        if (drawGizmos) {
            Vector3 tankPosition3 = new Vector3(tankPosition.x, 0, tankPosition.y);
            Vector3 manipulatorPosition3 = new Vector3(manipulatorPosition.x, 0, manipulatorPosition.y);
            Gizmos.DrawLine(tankPosition3, manipulatorPosition3);
            Gizmos.DrawLine(manipulatorPosition3, target.position);

            Vector3 right3 = new Vector3(right.x, 0, right.y);
            Vector3 center3 = tankPosition3 + right3 * radius;
            Gizmos.DrawLine(tankPosition3, center3);
            Gizmos.DrawLine(manipulatorPosition3, center3);

            float absRadius = Mathf.Abs(radius);
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireArc(center3, Vector3.up, manipulatorPosition3, 360.0f, absRadius);
        }
        
        return new WheelsSpeed {
            LeftSpeed = leftVelocity,
            RightSpeed = rightVelocity
        };
    }
    public static float GetAngle(Transform origin, Vector3 toPosition) => Vector3.SignedAngle(origin.forward, toPosition, Vector3.up);

}
