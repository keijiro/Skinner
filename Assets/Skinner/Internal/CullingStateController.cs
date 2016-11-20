using UnityEngine;

namespace Skinner
{
    [AddComponentMenu("")]
    public class CullingStateController : MonoBehaviour
    {
        public Renderer target { get; set; }

        void OnPreCull()
        {
            target.enabled = true;
        }

        void OnPostRender()
        {
            target.enabled = false;
        }
    }
}
