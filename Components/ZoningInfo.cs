// File: Components/ZoningInfo.cs
// Purpose: Zone Tools per-road settings.
// Notes:
// - Do not reorder enum members.
// - Do not change released numeric values.
// - New members can be appended with explicit numeric values.
// - Default(ZoningMode) == Left because Left == 0.
// - Always set the mode explicitly when creating ZoningInfo.

namespace ZoningToolkit.Components
{
    using Colossal.Serialization.Entities;
    using System;
    using Unity.Entities;

    public enum ZoningMode : uint
    {
        Left = 0,
        Right = 1,
        Default = 2,
        None = 3
    }

    public struct ZoningInfo : IComponentData, IQueryTypeParameter, IEquatable<ZoningInfo>, ISerializable
    {
        // Stored on the road owner entity.
        // Used later when applying or re-applying block size edits.
        public ZoningMode zoningMode;

        public readonly bool Equals(ZoningInfo other) => zoningMode == other.zoningMode;

        public override readonly int GetHashCode( ) => zoningMode.GetHashCode();

        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            // Save the raw enum value so released numeric values stay stable.
            writer.Write((uint) zoningMode);
        }

        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            // Read the saved raw enum value back into the enum.
            reader.Read(out uint value);
            zoningMode = (ZoningMode) value;
        }
    }

    // Marker component.
    // Added to block entities to trigger the one-shot update pass for existing roads.
    public struct ZoningInfoUpdated : IComponentData, IQueryTypeParameter
    {
    }
}
