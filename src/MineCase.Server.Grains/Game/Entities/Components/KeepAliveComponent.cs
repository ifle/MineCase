﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MineCase.Engine;
using MineCase.Server.Components;
using MineCase.Server.Network.Play;

namespace MineCase.Server.Game.Entities.Components
{
    internal class KeepAliveComponent : Component
    {
        private uint _keepAliveId = 0;
        public readonly HashSet<uint> _keepAliveWaiters = new HashSet<uint>();
        private DateTime _keepAliveRequestTime;
        private DateTime _keepAliveResponseTime;
        private bool _isOnline = false;

        private const int ClientKeepInterval = 6;

        public uint Ping
        {
            get
            {
                uint ping;
                var diff = DateTime.UtcNow - _keepAliveRequestTime;
                if (diff.Ticks < 0)
                    ping = int.MaxValue;
                else
                    ping = (uint)diff.TotalMilliseconds;
                return ping;
            }
        }

        public KeepAliveComponent(string name = "keepAlive")
            : base(name)
        {
        }

        protected override Task OnAttached()
        {
            AttachedObject.GetComponent<GameTickComponent>()
                .Tick += OnGameTick;
            _isOnline = true;
            return base.OnAttached();
        }

        protected override Task OnDetached()
        {
            AttachedObject.GetComponent<GameTickComponent>()
                .Tick -= OnGameTick;
            _keepAliveWaiters.Clear();
            return base.OnDetached();
        }

        public Task ReceiveResponse(uint keepAliveId)
        {
            _keepAliveWaiters.Remove(keepAliveId);
            if (_keepAliveWaiters.Count == 0)
                _keepAliveResponseTime = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        private async Task OnGameTick(object sender, (TimeSpan deltaTime, long worldAge) e)
        {
            if (_isOnline && _keepAliveWaiters.Count >= ClientKeepInterval)
            {
                _isOnline = false;
                await AttachedObject.GetComponent<ClientboundPacketComponent>().Kick();
            }
            else
            {
                var id = _keepAliveId++;
                _keepAliveWaiters.Add(id);
                _keepAliveRequestTime = DateTime.UtcNow;
                await AttachedObject.GetComponent<ClientboundPacketComponent>().GetGenerator().KeepAlive(id);
            }
        }
    }
}
