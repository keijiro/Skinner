using UnityEngine;

namespace Skinner
{
    // An internal utility that enables a given renderer
    // only while rendered from our camera.
    [AddComponentMenu("")] // Hidden from the component menu.
    internal class CullingStateController : MonoBehaviour
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
