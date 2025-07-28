using UnityEngine;

public class AnimateImageByMovement : MonoBehaviour
{
    [SerializeField] private Vector3 clampAngles;
    [SerializeField] private float springStrength = 10f; // Fuerza del resorte
    [SerializeField] private float damping = 2f;         // Amortiguación
    [SerializeField] private float speed = 10f;

    Vector3 _currentAngle;
    Vector3 _angularVelocity;
    Vector3 _lastPosition;


    private void FixedUpdate()
    {
        Vector3 difference = transform.position - _lastPosition;
        Vector3 targetAngle = Vector3.zero;

        if (difference.sqrMagnitude > 0.0001f)
        {
            targetAngle = new Vector3(difference.z, 0, difference.x);
            targetAngle.x = Mathf.Clamp(targetAngle.x, -clampAngles.x, clampAngles.x);
            targetAngle.y = Mathf.Clamp(targetAngle.y, -clampAngles.y, clampAngles.y);
            targetAngle.z = Mathf.Clamp(targetAngle.z, -clampAngles.z, clampAngles.z);
        }

        // Simulación de resorte amortiguado
        Vector3 force = (targetAngle - _currentAngle) * springStrength;
        _angularVelocity += force * Time.fixedDeltaTime;
        _angularVelocity *= Mathf.Exp(-damping * Time.fixedDeltaTime); // Exponential dampening
        _currentAngle += _angularVelocity * Time.fixedDeltaTime;

        // Limita los ángulos
        _currentAngle.x = Mathf.Clamp(_currentAngle.x, -clampAngles.x, clampAngles.x);
        _currentAngle.y = Mathf.Clamp(_currentAngle.y, -clampAngles.y, clampAngles.y);
        _currentAngle.z = Mathf.Clamp(_currentAngle.z, -clampAngles.z, clampAngles.z);

        // Aplica la rotación suavizada
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_currentAngle), speed * Time.fixedDeltaTime);

        _lastPosition = transform.position;
    }
}
