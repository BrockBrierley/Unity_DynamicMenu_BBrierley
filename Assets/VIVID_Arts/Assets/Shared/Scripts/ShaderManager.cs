using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTemplateProjects
{
    [Serializable]
    public struct GlobalFloat
    {
        public string Name;

        public float Value;
    }

    [ExecuteInEditMode]
    public class ShaderManager : MonoBehaviour
    {
        public List<GlobalFloat> Floats;

        public void Update()
        {
            foreach (var instance in Floats)
            {
                Shader.SetGlobalFloat(instance.Name, instance.Value);
            }
        }
    }
}