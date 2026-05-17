using UnityEngine;
using CarRacing.Interface;

namespace CarRacing.Player.Car
{

    [RequireComponent(typeof(Rigidbody2D))]
    public class CarController : MonoBehaviour, ICarController
    {
        [SerializeField] private CarScriptableObject carSO;
        private Rigidbody2D rb;
        private float currentSpeed;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Move(float steering, bool accelerate, bool brake, bool uTurn, bool drift)
        {
            float grip = carSO.NormalGrip;

            if (Mathf.Abs(steering) > carSO.DriftThreshold)
                grip = carSO.SharpTurnGrip;

            if (drift)
                grip = carSO.ManualDriftGrip;

            if (accelerate)
                currentSpeed += carSO.Acceleration * Time.deltaTime;
            else if (brake)
                currentSpeed -= carSO.BrakeForce * Time.deltaTime;
            else
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, carSO.BrakeForce * 0.5f * Time.deltaTime);

            currentSpeed = Mathf.Clamp(currentSpeed, -carSO.MaxSpeed * 0.5f, carSO.MaxSpeed);

            if (currentSpeed != 0)
            {
                transform.Rotate(Vector3.forward, -steering * carSO.SteeringSpeed * Time.deltaTime * (currentSpeed / carSO.MaxSpeed));
            }

            if (uTurn)
            {
                transform.Rotate(Vector3.forward, carSO.UTurnSpeed * Time.deltaTime);
            }

            Vector2 forwardVel = transform.up * Vector2.Dot(rb.velocity, transform.up);
            Vector2 rightVel = transform.right * Vector2.Dot(rb.velocity, transform.right);
            rb.velocity = forwardVel + rightVel * grip;

            rb.velocity += (Vector2)transform.up * currentSpeed * Time.deltaTime;
        }
    }
}