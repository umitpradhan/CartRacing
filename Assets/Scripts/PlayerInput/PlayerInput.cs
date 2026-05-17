using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarRacing.Interface;

namespace CarRacing.Player
{
    public class PlayerInput : MonoBehaviour, IPlayerInput
    {
        public float GetSteering()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                return (touch.position.x < Screen.width / 2) ? -1f : 1f;
            }
            return 0f;
        }

        public bool GetAccelerate()
        {
            foreach (Touch t in Input.touches)
                if (t.position.y > Screen.height * 0.6f)
                    return true;
            return false;
        }

        public bool GetBrake()
        {
            foreach (Touch t in Input.touches)
                if (t.position.y < Screen.height * 0.4f)
                    return true;
            return false;
        }

        public bool GetUTurn()
        {
            foreach (Touch t in Input.touches)
                if (t.position.x > Screen.width * 0.4f && t.position.x < Screen.width * 0.6f &&
                    t.position.y < Screen.height * 0.3f)
                    return true;
            return false;
        }

        public bool GetDrift()
        {
            foreach (Touch t in Input.touches)
            {
                bool bottomZone = t.position.y < Screen.height * 0.4f;
                bool sideZone = t.position.x < Screen.width * 0.2f || t.position.x > Screen.width * 0.8f;
                if (bottomZone && sideZone)
                    return true;
            }
            return false;
        }
    }
}