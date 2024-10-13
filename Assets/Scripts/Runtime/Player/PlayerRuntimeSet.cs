using System.Linq;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Player
{
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class PlayerRuntimeSet : RuntimeSet<ShipController>
    {

        public ShipController GetPlayer()
        {
            return GetFirst();
        }

    }
}