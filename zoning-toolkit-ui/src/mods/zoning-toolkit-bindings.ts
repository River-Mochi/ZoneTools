// File: zoning-toolkit-ui/src/mods/zoning-toolkit-bindings.ts
// Purpose: Typed cs2/api bindings + triggers for Zone Tools UI.
// Notes:
// - Uses cs2/api bindValue/useValue/trigger (TopoToggle pattern).
// - Group string must match the C# binding group (ZoneToolBridgeUI.kGroup).

import { bindValue, trigger, useValue } from "cs2/api";

// Must match ZoneToolBridgeUI.kGroup
export const ZT_GROUP = "zoning_adjuster_ui_namespace";

// C# -> UI (GetterValueBinding)
export const ZoningMode$ = bindValue<string>(ZT_GROUP, "zoning_mode", "Default");
export const ToolEnabled$ = bindValue<boolean>(ZT_GROUP, "tool_enabled", false);
export const Visible$ = bindValue<boolean>(ZT_GROUP, "visible", false);
export const PhotoMode$ = bindValue<boolean>(ZT_GROUP, "photomode", false);

// Hooks (UI reads)
export function useZoningMode(): string {
    return useValue(ZoningMode$);
}

export function useToolEnabled(): boolean {
    return useValue(ToolEnabled$);
}

export function useVisible(): boolean {
    return useValue(Visible$);
}

export function usePhotoMode(): boolean {
    return useValue(PhotoMode$);
}

// UI -> C# (TriggerBinding)
export function triggerTogglePanel(): void {
    trigger(ZT_GROUP, "toggle_panel", true);
}

export function triggerSetToolEnabled(enabled: boolean): void {
    trigger(ZT_GROUP, "tool_enabled", enabled);
}

export function triggerSetZoningMode(mode: string): void {
    trigger(ZT_GROUP, "zoning_mode_update", mode);
}
