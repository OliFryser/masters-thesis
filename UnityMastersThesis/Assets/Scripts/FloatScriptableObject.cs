using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Float", order = 0)]
    public class FloatScriptableObject : ScriptableObject
    {
        public float Value;
    }
}