﻿using MineCase.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MineCase.Protocol
{
    [Packet]
    public struct UncompressedPacket
    {
        [SerializeAs(DataType.VarInt)]
        public uint Length;

        [SerializeAs(DataType.VarInt)]
        public uint PacketId;

        [SerializeAs(DataType.ByteArray)]
        public byte[] Data;

        public async Task SerializeAsync(Stream stream)
        {
            Length = (uint)Data.Length + PacketId.SizeOfVarInt();

            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.WriteAsVarInt(Length);
                bw.WriteAsVarInt(PacketId);
                bw.Flush();
            }
            await stream.WriteAsync(Data, 0, Data.Length);
        }

        public static async Task<UncompressedPacket> DeserializeAsync(Stream stream)
        {
            var packet = new UncompressedPacket();
            int packetIdLen;
            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                packet.Length = br.ReadAsVarInt(out _);
                packet.PacketId = br.ReadAsVarInt(out packetIdLen);
            }
            packet.Data = new byte[packet.Length - packetIdLen];
            await stream.ReadExactAsync(packet.Data, 0, packet.Data.Length);
            return packet;
        }
    }

    [Packet]
    public struct CompressedPacket
    {
        [SerializeAs(DataType.VarInt)]
        public uint PacketLength;

        [SerializeAs(DataType.VarInt)]
        public uint DataLength;

        [SerializeAs(DataType.VarInt)]
        public byte[] CompressedData;

        public async Task SerializeAsync(Stream stream)
        {
            PacketLength = (uint)CompressedData.Length + DataLength.SizeOfVarInt();

            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.WriteAsVarInt(PacketLength);
                bw.WriteAsVarInt(DataLength);
                bw.Flush();
            }
            await stream.WriteAsync(CompressedData, 0, CompressedData.Length);
        }

        public static async Task<CompressedPacket> DeserializeAsync(Stream stream)
        {
            var packet = new CompressedPacket();
            int dataLengthLen;
            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                packet.PacketLength = br.ReadAsVarInt(out _);
                packet.DataLength = br.ReadAsVarInt(out dataLengthLen);
            }
            packet.CompressedData = new byte[packet.PacketLength - dataLengthLen];
            await stream.ReadExactAsync(packet.CompressedData, 0, packet.CompressedData.Length);
            return packet;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class PacketAttribute : Attribute
    {
        public PacketAttribute()
        {
        }
    }
}
