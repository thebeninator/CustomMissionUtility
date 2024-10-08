﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using static Reticle.ReticleTree;

namespace CustomMissionUtility
{
    internal class Vec3FieldHandler : MonoBehaviour
    {
        public TMP_InputField x_field;
        public TMP_InputField y_field;
        public TMP_InputField z_field;
        
        void Awake() { 
            x_field = transform.Find("X/inp").GetComponentInChildren<TMP_InputField>();
            y_field = transform.Find("Y/inp").GetComponentInChildren<TMP_InputField>();
            z_field = transform.Find("Z/inp").GetComponentInChildren<TMP_InputField>();
        }

        public void SetActive(bool active)
        {
            x_field.interactable = active;
            y_field.interactable = active; 
            z_field.interactable = active;
        }

        public void UpdateVec(Vector3 vec3) {
            x_field.text = vec3.x.ToString();
            y_field.text = vec3.y.ToString();
            z_field.text = vec3.z.ToString();
        }
    }
}
