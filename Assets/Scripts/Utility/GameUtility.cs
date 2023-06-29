using UnityEngine;

namespace Utility
{
    public class GameUtility
    {
        void SetLayerReceive(Transform parent, int layer)
        {
            parent.gameObject.layer = layer;
            foreach (Transform child in parent)
            {
                SetLayerReceive(child.transform, layer);
            }
        }
    }
}