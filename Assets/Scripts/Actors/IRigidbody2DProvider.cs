using UnityEngine;

namespace Starblast.Actors
{
    public interface IRigidbody2DProvider
    {
        Rigidbody2D GetRigidbody2D();
    }
}