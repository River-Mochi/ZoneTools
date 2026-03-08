// src/mods/state.tsx
// Global UI state for Zone Tools (Zustand store + Cohtml event wiring + HOC).
// Notes:
// - C# is the source of truth for: visible, tool_enabled, photomode.
// - Avoid echo loops: never call JS->C# setters inside C# subscription callbacks.

import engine, { EventHandle } from "cohtml/cohtml";
import { create } from "zustand";
import React from "react";

// Flip this to true locally for noisy console logging for UI/debug.
const DEBUG_LOG = false;

function debugLog(...args: unknown[]): void {
    if (!DEBUG_LOG) {
        return;
    }
    console.log(...args);
}

export interface ModUIState {
    uiVisible: boolean;
    photomodeActive: boolean;
    zoningMode: string;

    // Derived from C# tool state (do not set optimistically; wait for C# update).
    isToolEnabled: boolean;

    updateZoningMode: (newValue: string) => void;
    requestToolEnabled: (newValue: boolean) => void;
    updatePhotomodeActive: (newValue: boolean) => void;
    updateUiVisible: (newValue: boolean) => void;
}

const allSubscriptions = new Map<string, () => void>();

// Namespace shared with C# ZoneToolBridgeUI.
const NS = "zoning_adjuster_ui_namespace";

export const useModUIStore = create<ModUIState>((set) => ({
    uiVisible: false,
    photomodeActive: false,
    zoningMode: "Default",
    isToolEnabled: false,

    updateUiVisible: (newValue: boolean) => {
        debugLog("[ZoneTools] uiVisible <- C#", newValue);
        set({ uiVisible: newValue });
        // visible is driven from C# → JS only
    },

    updatePhotomodeActive: (newValue: boolean) => {
        debugLog("[ZoneTools] photomodeActive <- C#", newValue);
        set({ photomodeActive: newValue });
    },

    // Request tool enable/disable.
    // IMPORTANT: do NOT set isToolEnabled here; C# may refuse to enable (safe prefab not ready).
    requestToolEnabled: (newValue: boolean) => {
        debugLog("[ZoneTools] Request tool_enabled -> C#", newValue);
        sendDataToCSharp(NS, "tool_enabled", newValue);
    },

    // Zoning mode can be updated optimistically (C# accepts it); C# will still echo it back.
    updateZoningMode: (newValue: string) => {
        debugLog("[ZoneTools] zoningMode -> C#", newValue);
        sendDataToCSharp(NS, "zoning_mode_update", newValue);
        set({ zoningMode: newValue });
    },
}));

export const setupSubscriptions = (): void => {
    debugLog("[ZoneTools] setupSubscriptions");

    // zoning_mode (string)
    const zoningModeEventKey = `${NS}.zoning_mode`;
    if (!allSubscriptions.has(zoningModeEventKey)) {
        const subscription = updateEventFromCSharp<string>(NS, "zoning_mode", (zoningMode) => {
            debugLog("[ZoneTools] zoning_mode <- C#", zoningMode);
            useModUIStore.setState({ zoningMode });
        });
        allSubscriptions.set(zoningModeEventKey, subscription);
    }

    // tool_enabled (bool)
    const toolEnabledEventKey = `${NS}.tool_enabled`;
    if (!allSubscriptions.has(toolEnabledEventKey)) {
        const subscription = updateEventFromCSharp<boolean>(NS, "tool_enabled", (toolEnabled) => {
            debugLog("[ZoneTools] tool_enabled <- C#", toolEnabled);
            useModUIStore.setState({ isToolEnabled: toolEnabled });
        });
        allSubscriptions.set(toolEnabledEventKey, subscription);
    }

    // visible (bool)
    const visibleEventKey = `${NS}.visible`;
    if (!allSubscriptions.has(visibleEventKey)) {
        const subscription = updateEventFromCSharp<boolean>(NS, "visible", (visible) => {
            debugLog("[ZoneTools] visible <- C#", visible);
            useModUIStore.getState().updateUiVisible(visible);
        });
        allSubscriptions.set(visibleEventKey, subscription);
    }

    // photomode (bool)
    const photomodeEventKey = `${NS}.photomode`;
    if (!allSubscriptions.has(photomodeEventKey)) {
        const subscription = updateEventFromCSharp<boolean>(NS, "photomode", (photomodeEnabled) => {
            debugLog("[ZoneTools] photomode <- C#", photomodeEnabled);
            useModUIStore.getState().updatePhotomodeActive(photomodeEnabled);
        });
        allSubscriptions.set(photomodeEventKey, subscription);
    }
};

export const teardownSubscriptions = (): void => {
    debugLog("[ZoneTools] teardownSubscriptions");

    allSubscriptions.forEach((unsubscribe, eventKey) => {
        debugLog("[ZoneTools] unsubscribe", eventKey);
        unsubscribe();
    });

    allSubscriptions.clear();
};

export function updateEventFromCSharp<T>(
    ns: string,
    eventName: string,
    callback: (value: T) => void,
): () => void {
    const updateEvent = `${ns}.${eventName}.update`;
    const subscribeEvent = `${ns}.${eventName}.subscribe`;
    const unsubscribeEvent = `${ns}.${eventName}.unsubscribe`;

    debugLog(`[ZoneTools] subscribe ${updateEvent}`);

    const sub: EventHandle = engine.on(updateEvent, callback);
    engine.trigger(subscribeEvent);

    return () => {
        engine.trigger(unsubscribeEvent);
        sub.clear();
    };
}

export function sendDataToCSharp<T>(ns: string, eventName: string, newValue: T): void {
    debugLog(`[ZoneTools] trigger ${ns}.${eventName}`, newValue);
    engine.trigger(`${ns}.${eventName}`, newValue);
}

export function togglePanelFromUI(): void {
    // Payload is ignored on C# side; bool is fine.
    sendDataToCSharp(NS, "toggle_panel", true);
}

export function withStore(
    WrappedComponent: React.ComponentType<Partial<ModUIState>>,
): React.FC {
    const WithStore: React.FC = () => {
        const storeState = useModUIStore();
        return <WrappedComponent {...storeState} />;
    };

    WithStore.displayName = `WithStore(${WrappedComponent.displayName ?? WrappedComponent.name ?? "Component"})`;
    return WithStore;
}
