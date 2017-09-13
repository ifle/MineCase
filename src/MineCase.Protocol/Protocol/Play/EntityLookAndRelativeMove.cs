﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MineCase.Serialization;
using Orleans.Concurrency;

namespace MineCase.Protocol.Play
{
    [Immutable]
    [Packet(0x27)]
    public sealed class EntityLookAndRelativeMove : ISerializablePacket
    {
        [SerializeAs(DataType.VarInt)]
        public uint EID;

        [SerializeAs(DataType.Short)]
        public short DeltaX;

        [SerializeAs(DataType.Short)]
        public short DeltaY;

        [SerializeAs(DataType.Short)]
        public short DeltaZ;

        [SerializeAs(DataType.Angle)]
        public float Yaw;

        [SerializeAs(DataType.Angle)]
        public float Pitch;

        [SerializeAs(DataType.Boolean)]
        public bool OnGround;

        public void Serialize(BinaryWriter bw)
        {
            bw.WriteAsVarInt(EID, out _);
            bw.WriteAsShort(DeltaX);
            bw.WriteAsShort(DeltaY);
            bw.WriteAsShort(DeltaZ);
            bw.WriteAsFloat(Yaw);
            bw.WriteAsFloat(Pitch);
            bw.WriteAsBoolean(OnGround);
        }
    }
}
