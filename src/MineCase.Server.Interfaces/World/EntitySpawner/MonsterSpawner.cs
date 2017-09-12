﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MineCase.Server.Game;
using MineCase.Server.Game.Entities;
using Orleans;

namespace MineCase.Server.World.EntitySpawner
{
    public class MonsterSpawner
    {
        private int _groupMaxNum;

        private MobType _mobType;

        public MonsterSpawner(MobType mobType, int groupMaxNum)
        {
            _mobType = mobType;
            _groupMaxNum = groupMaxNum;
        }

        public async void Spawn(IWorld world, IGrainFactory grainFactory, ChunkColumnStorage chunk, Random random, BlockWorldPos pos)
        {
            int num = random.Next(_groupMaxNum);
            for (int n = 0; n < num; ++n)
            {
                int x = random.Next(16);
                int z = random.Next(16);

                if (CanMobStand(world, grainFactory, chunk, random, pos.ToBlockChunkPos()))
                {
                    // 添加一个生物
                    var eid = await world.NewEntityId();
                    var entity = grainFactory.GetGrain<IPassiveMob>(world.MakeEntityKey(eid));
                    await world.AttachEntity(entity);
                    await entity.Spawn(Guid.NewGuid(), new Vector3(pos.X, pos.Y, pos.Z), _mobType);
                }
            }
        }

        public bool CanMobStand(IWorld world, IGrainFactory grainFactory, ChunkColumnStorage chunk, Random random, BlockChunkPos pos)
        {
            // TODO 以后结合boundbox判断
            BlockChunkPos downPos = new BlockChunkPos(pos.X, pos.Y - 1, pos.Z);
            if (chunk[pos.X, pos.Y - 1, pos.Z].IsLightOpacity() == 0)
            {
                if (chunk[pos.X, pos.Y, pos.Z] == BlockStates.Air() &&
                    chunk[pos.X, pos.Y + 1, pos.Z] == BlockStates.Air())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
