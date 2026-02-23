using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(Visualizer))]
    public class VisualizerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            
            Visualizer mapVisualizer = (Visualizer)target;
            
            root.Add(new Button(() =>
            {
                mapVisualizer.DisplayMap();
            })
            {
                text = "Display Map"
            });
            
            return root;
        }
    }
}