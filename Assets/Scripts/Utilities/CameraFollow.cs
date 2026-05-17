using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CameraFollow : MonoBehaviour
    {
        private Vector3 m_Offset = new Vector3(0,6, -10);
        public float m_SmoothTime;
        private Vector3 m_Velocity = Vector3.zero;
        [SerializeField]
        private Transform m_Target;

        void Start()
        {

        }
        void FixedUpdate()
        {
            if (PlayerCar.m_Current == null) return;
            m_Target = PlayerCar.m_Current.transform;

            float vertical = Input.GetAxis("Vertical");
            if (vertical < 0f)
            {
                m_Offset = new Vector3(0, -6, -10);
            }
            else if (vertical > 0f)
            {
                m_Offset = new Vector3(0, 6, -10);
            }

            Vector3 targetPosition = m_Target.position + m_Offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref m_Velocity, m_SmoothTime);
        }
    }
