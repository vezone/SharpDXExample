using System;
using System.Collections.Generic;
using System.Linq;
using TankClient;
using TankCommon;
using TankCommon.Enum;
using TankCommon.Objects;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using SharpDX.Mathematics.Interop;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using System.Threading.Tasks;
using System.Threading;

namespace TankGuiObserver2
{
    class GuiSpectator : IClientBot
    {
        public Map Map { get; set; }
        protected DateTime _lastMapUpdate;
        protected readonly CancellationToken _cancellationToken;
        protected readonly object _syncObject = new object();
        protected int _msgCount;
        protected bool _wasUpdate;

        public GuiSpectator(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
#pragma warning disable 4014
            DisplayMap();
#pragma warning restore 4014
        }

        protected async Task DisplayMap()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
                if (!_wasUpdate)
                {
                    continue;
                }

                Map map;
                lock (_syncObject)
                {
                    _wasUpdate = false;
                    map = new Map(Map, Map.InteractObjects);
                }
            }
        }
        public ServerResponse Client(int msgCount, ServerRequest request)
        {
            lock (_syncObject)
            {
                if (request.Map.Cells != null)
                {
                    Map = request.Map;
                    _lastMapUpdate = DateTime.Now;
                }
                else if (Map == null)
                {
                    return new ServerResponse { ClientCommand = ClientCommandType.UpdateMap };
                }

                Map.InteractObjects = request.Map.InteractObjects;
                _msgCount = msgCount;
                _wasUpdate = true;

                return new ServerResponse { ClientCommand = ClientCommandType.None };
            }
        }
    }
}
