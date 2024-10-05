using Starblast.Services;
using UnityEngine;

namespace Starblast.Audio
{
    public class AutoPlayNextMusic : MonoBehaviour
    {
        private void Start()
        {
            ServiceLocator.Main.MusicManager.PlayNext();
        }
    }
}