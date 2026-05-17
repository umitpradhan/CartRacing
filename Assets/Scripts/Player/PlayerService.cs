using UnityEngine;
using CarRacing.Interface;

namespace CarRacing.Player
{
    public class PlayerService : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour inputSource; // must implement IPlayerInput
        [SerializeField] private MonoBehaviour carControllerSource; // must implement ICarController

        private IPlayerInput input;
        private ICarController carController;

        void Awake()
        {
            input = inputSource as IPlayerInput;
            carController = carControllerSource as ICarController;

            if (input == null) Debug.LogError("Input source must implement IPlayerInput.");
            if (carController == null) Debug.LogError("Car controller must implement ICarController.");
        }

        void Update()
        {
            if (input == null || carController == null) return;

            carController.Move(
                input.GetSteering(),
                input.GetAccelerate(),
                input.GetBrake(),
                input.GetUTurn(),
                input.GetDrift()
            );
        }
    }
}