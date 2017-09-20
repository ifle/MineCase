﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MineCase.Server.Game;
using MineCase.Server.Game.Entities;
using Orleans;
using Orleans.Concurrency;

namespace MineCase.Server.World
{
    public interface ICollectableFinder : IAddressByPartition
    {
        /*
        Task Register(ICollectable collectable);

        Task Unregister(ICollectable collectable);

        Task<IReadOnlyCollection<ICollectable>> Collision(IEntity entity);

        Task<IReadOnlyCollection<ICollectable>> CollisionInChunk(IEntity entity);
        */

        Task SpawnPickup(Vector3 position, Immutable<Slot[]> slots);
    }
}
