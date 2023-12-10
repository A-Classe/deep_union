using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Module.Player
{
    public class SonarVisualizer : MonoBehaviour
    {
        [SerializeField] private VisualEffect sonarEffect;
        [SerializeField] private float fixRate = 0.5f;

        public void SetSize(float size)
        {
            sonarEffect.SetFloat("SonarSize", size*fixRate);
        }
    }
}