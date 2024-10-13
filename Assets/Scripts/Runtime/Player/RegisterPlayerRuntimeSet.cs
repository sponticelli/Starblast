using Starblast.Services;
using UnityEngine;

namespace Starblast.Player
{
    [DefaultExecutionOrder(ExecutionOrder.RegisterToRuntimeSet)]
    public class RegisterPlayerRuntimeSet : RegisterToRuntimeSet<ShipController, PlayerRuntimeSet>
    {

    }
}