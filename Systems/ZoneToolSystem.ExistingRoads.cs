// File: Systems/ZoneToolSystem.ExistingRoads.cs
// Purpose: Existing Roads tool lifecycle + activation logic.
// Notes:
// - Same tool can run in 2 modes:
//   1) contour host mode: active only to hold contour/topography snap
//   2) real Update Road mode: hover/select/apply on existing roads
// - ToolSystem.activeTool swap drives the real start/stop cycle.

namespace ZoningToolkit.Systems
{
    using Colossal.Serialization.Entities; // Purpose (OnGameLoadingComplete signature)
    using Game;                            // GameMode, ToolBaseSystem
    using Game.Common;
    using Game.Net;                        // Layer
    using Game.Prefabs;                    // PrefabBase, PrefabSystem
    using Game.Tools;                      // ToolSystem, DefaultToolSystem, NetToolSystem, ToolOutputBarrier
    using Game.Zones;                      // Layer
    using Unity.Collections;               // NativeHashSet, Allocator
    using Unity.Entities;                  // Entity, EntityCommandBuffer
    using Unity.Jobs;                      // JobHandle

    internal sealed partial class ZoneToolSystemExistingRoads : ToolBaseSystem
    {
        private ToolSystem m_ZTToolSystem = null!;
        private DefaultToolSystem m_ZTDefaultToolSystem = null!;
        private NetToolSystem m_NetToolSystem = null!;
        private ToolOutputBarrier m_ToolOutputBarrier = null!;
        private PrefabSystem m_ZTPrefabSystem = null!;
        private ZoneToolBridgeUI m_UISystem = null!;

        // Host mode = active only to hold contour/topography snap.
        // Real mode = full Update Road behavior.
        private bool m_ContourHostActive;

        // Shared runtime state used by apply/highlight partials.
        private NativeHashSet<Entity> m_Selected;
        private int m_SelectedCount;
        private Entity m_Hovered;
        private Entity m_Highlighted;

        internal bool toolEnabled
        {
            get;
            private set;
        }

        public override string toolID => "ZoningToolkit.ExistingRoads";

        protected override void OnCreate( )
        {
            base.OnCreate();

            Enabled = false;

            m_ZTToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_ZTDefaultToolSystem = World.GetOrCreateSystemManaged<DefaultToolSystem>();
            m_NetToolSystem = World.GetOrCreateSystemManaged<NetToolSystem>();
            m_ToolOutputBarrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_ZTPrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_UISystem = World.GetOrCreateSystemManaged<ZoneToolBridgeUI>();

            InitializeAudio();

            m_Selected = new NativeHashSet<Entity>(128, Allocator.Persistent);
            m_SelectedCount = 0;

            m_Hovered = Entity.Null;
            m_Highlighted = Entity.Null;

            toolEnabled = false;
            ResetSafePrefabForUI();
        }

        protected override void OnDestroy( )
        {
            if (m_Selected.IsCreated)
            {
                m_Selected.Dispose();
            }

            base.OnDestroy();
        }

        protected override void OnStartRunning( )
        {
            base.OnStartRunning();

            // Host mode keeps the tool active only so contour/topography can exist.
            // No road interaction, no actions, no road raycast.
            if (m_ContourHostActive)
            {
                toolEnabled = false;

                applyAction.shouldBeEnabled = false;
                secondaryApplyAction.shouldBeEnabled = false;

                requireNet = Layer.None;
                requireZones = false;
                allowUnderground = false;

                Snap snap = m_NetToolSystem.selectedSnap;
                if (snap == default)
                {
                    snap = Snap.All;
                }

                selectedSnap = snap;
                return;
            }

            // Real Update Road mode: enable actions and road interaction.
            toolEnabled = true;

            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;

            requireNet = Layer.Road;
            requireZones = true;
            allowUnderground = false;

            // Seed snap from vanilla so contour overlay can render.
            Snap snapToUse = selectedSnap;
            if (snapToUse == default)
            {
                snapToUse = m_NetToolSystem.selectedSnap;
                if (snapToUse == default)
                {
                    snapToUse = Snap.All;
                }
            }

            selectedSnap = snapToUse;
        }

        protected override void OnStopRunning( )
        {
            base.OnStopRunning();

            toolEnabled = false;

            applyAction.shouldBeEnabled = false;
            secondaryApplyAction.shouldBeEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

            // ToolOutputBarrier.CreateCommandBuffer() is not allowed in OnStopRunning().
            // Highlight cleanup uses immediate EntityManager structural changes.
            ClearHoverHighlightImmediate();
        }

        public override void InitializeRaycast( )
        {
            base.InitializeRaycast();

            // Host mode must raycast nothing.
            if (m_ContourHostActive)
            {
                m_ToolRaycastSystem.typeMask = TypeMask.None;
                m_ToolRaycastSystem.netLayerMask = Layer.None;
                return;
            }

            // Real mode raycasts roads/lanes so hover + selection can work.
            m_ToolRaycastSystem.typeMask = TypeMask.Net | TypeMask.Lanes;
            m_ToolRaycastSystem.netLayerMask = Layer.Road;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            ResetSafePrefabForUI();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // Host mode intentionally does no road work.
            // It only exists so contour/topography can stay active.
            if (m_ContourHostActive)
            {
                return inputDeps;
            }

            if (!toolEnabled)
            {
                return inputDeps;
            }

            applyMode = ApplyMode.Clear;

            EntityCommandBuffer ecb = m_ToolOutputBarrier.CreateCommandBuffer();

            if (secondaryApplyAction.WasPressedThisFrame())
            {
                CycleZoningMode();
                PlaySnapSound();
            }

            UpdateHover();
            UpdateHoverHighlight(ecb);

            if (applyAction.WasPressedThisFrame() || applyAction.IsPressed())
            {
                AddHoveredToSelection();
            }

            if (applyAction.WasReleasedThisFrame())
            {
                ApplySelection();
            }

            return inputDeps;
        }

        public override PrefabBase GetPrefab( )
        {
            return GetSafePrefabForUI();
        }

        public override bool TrySetPrefab(PrefabBase prefab)
        {
            return false;
        }

        public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
        {
            base.GetAvailableSnapMask(out onMask, out offMask);

            // Contour must be present in both masks so selectedSnap can turn it on/off.
            // Putting it only in onMask forces it on.
            onMask |= Snap.ContourLines;
            offMask |= Snap.ContourLines;
        }

        internal bool EnableContourHost( )
        {
            if (m_ContourHostActive)
            {
                // Host flag already set, but tool may no longer be active.
                Enabled = true;

                if (m_ZTToolSystem.activeTool != this)
                {
                    m_ZTToolSystem.activeTool = this;
                }

                return true;
            }

            // Full Update Road mode already active.
            if (toolEnabled)
            {
                return true;
            }

            if (!TryResolveSafePrefabForUI(out _))
            {
                Enabled = false;
                Mod.s_Log.Warn($"{Mod.ModTag} ContourHost enable refused: safe prefab not ready.");
                return false;
            }

            m_ContourHostActive = true;
            Enabled = true;
            m_ZTToolSystem.activeTool = this;

            Snap snap = m_NetToolSystem.selectedSnap;
            if (snap == default)
            {
                snap = Snap.All;
            }

            selectedSnap = snap;

            // Keep vanilla road-tool snap in sync while host mode is active.
            m_NetToolSystem.selectedSnap = selectedSnap;

            return true;
        }

        internal void DisableContourHost( )
        {
            if (!m_ContourHostActive)
            {
                return;
            }

            // If full Update Road mode is already active, only clear the host flag.
            if (toolEnabled)
            {
                m_ContourHostActive = false;
                return;
            }

            m_ContourHostActive = false;

            // Hand last known snap back to vanilla before returning to DefaultToolSystem.
            m_NetToolSystem.selectedSnap = selectedSnap;

            if (m_ZTToolSystem.activeTool == this)
            {
                m_ZTToolSystem.activeTool = m_ZTDefaultToolSystem;
            }

            Enabled = false;
        }

        internal bool EnableTool( )
        {
            // Preserve snap state so contour/topography does not get lost
            // when switching from host mode into real Update Road mode.
            Snap snapToKeep = selectedSnap;
            if (snapToKeep == default)
            {
                snapToKeep = m_NetToolSystem.selectedSnap;
                if (snapToKeep == default)
                {
                    snapToKeep = Snap.All;
                }
            }

            // Host mode and real tool mode need different startup paths.
            // Shut down host mode first so this tool can restart correctly.
            if (m_ContourHostActive)
            {
                DisableContourHost();
            }

            m_ContourHostActive = false;

            if (!TryResolveSafePrefabForUI(out _))
            {
                toolEnabled = false;
                Enabled = false;
                Mod.s_Log.Warn($"{Mod.ModTag} ExistingRoads enable refused: safe prefab not ready.");
                return false;
            }

            // Seed snap before activation so OnStartRunning() sees the intended state.
            selectedSnap = snapToKeep;
            m_NetToolSystem.selectedSnap = selectedSnap;

            Enabled = true;
            m_ZTToolSystem.activeTool = this;

            toolEnabled = true;

            Mod.s_Log.Info($"{Mod.ModTag} ExistingRoads enabled");
            return true;
        }

        internal void DisableTool( )
        {
            m_ContourHostActive = false;
            toolEnabled = false;

            ClearSelection();
            m_Hovered = Entity.Null;

            // ToolOutputBarrier.CreateCommandBuffer() is not allowed from toolbar/tool-change paths.
            // Highlight cleanup uses immediate EntityManager structural changes.
            ClearHoverHighlightImmediate();

            // Persist snap state back to vanilla for consistency.
            m_NetToolSystem.selectedSnap = selectedSnap;

            m_ZTToolSystem.activeTool = m_ZTDefaultToolSystem;
            Enabled = false;

            PlayCancelSound();
            Mod.s_Log.Info($"{Mod.ModTag} ExistingRoads disabled");
        }
    }
}
