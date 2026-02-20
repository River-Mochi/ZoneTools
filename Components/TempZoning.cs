// File: Components/TempZoning.cs
// Purpose: Tool-only preview component to drive zoning-depth preview without committing.

namespace ZoningToolkit.Components
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct TempZoning : IComponentData
    {
        public int2 Depths;
    }
}
