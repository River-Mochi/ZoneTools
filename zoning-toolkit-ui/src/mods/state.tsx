// File: src/mods/state.tsx
// Purpose: Global UI state for Zone Tools (Zustand store + Cohtml event wiring + HOC).
// Notes:
// - C# is source of truth for: visible, tool_enabled, photomode, contour_enabled,
//   contour_button_visible, contour_tooloptions_visible, use_glass_panel.
// - Avoid echo loops: never call JS->C# setters inside C# subscription callbacks.

import engine, { EventHandle } from "cohtml/cohtml";
import { create } from "zustand";
import React from "react";

const DEBUG_LOG = false;

function debugLog(...args: unknown[]): void {
    if (!DEBUG_LOG) return;
    console.log(...args);
}

export interface ModUIState {
    uiVisible: boolean;
    photomodeActive: boolean;
    zoningMode: string;

    // From C# tool state (wait for C# update).
    isToolEnabled: boolean;

    // From C# global contour state (wait for C# update).
    contourEnabled: boolean;

    // From C# Settings/compatibility (wait for C# update).
    contourButtonVisible: boolean;

    // From C# active-tool context (wait for C# update).
    contourToolOptionsVisible: boolean;

    // From C# UI settings (wait for C# update).
    useGlassPanel: boolean;

    updateZoningMode: (newValue: string) => void;
    requestToolEnabled: (newValue: boolean) => void;
    requestToggleContourLines: () => void;

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
    contourEnabled: false,
    contourButtonVisible: true,
    contourToolOptionsVisible: false,
    useGlassPanel: true,

    updateUiVisible: (newValue: boolean) => {
        debugLog("[ZoneTools] visible <- C#", newValue);
        set({ uiVisible: newValue === true });
    },

    updatePhotomodeActive: (newValue: boolean) => {
        debugLog("[ZoneTools] photomode <- C#", newValue);
        set({ photomodeActive: newValue === true });
    },

    requestToolEnabled: (newValue: boolean) => {
        debugLog("[ZoneTools] tool_enabled -> C#", newValue);
        sendDataToCSharp(NS, "tool_enabled", newValue);
    },

    requestToggleContourLines: () => {
        debugLog("[ZoneTools] ToggleContourLines -> C#");
        sendDataToCSharp(NS, "ToggleContourLines", true);
    },

    updateZoningMode: (newValue: string) => {
        debugLog("[ZoneTools] zoning_mode_update -> C#", newValue);
        sendDataToCSharp(NS, "zoning_mode_update", newValue);
        set({ zoningMode: newValue });
    },
}));

export const setupSubscriptions = (): void => {
    debugLog("[ZoneTools] setupSubscriptions");

    subscribeOnce<string>("zoning_mode", (zoningMode) => {
        debugLog("[ZoneTools] zoning_mode <- C#", zoningMode);
        useModUIStore.setState({ zoningMode });
    });

    subscribeOnce<boolean>("tool_enabled", (toolEnabled) => {
        debugLog("[ZoneTools] tool_enabled <- C#", toolEnabled);
        useModUIStore.setState({ isToolEnabled: toolEnabled === true });
    });

    subscribeOnce<boolean>("visible", (visible) => {
        debugLog("[ZoneTools] visible <- C#", visible);
        useModUIStore.getState().updateUiVisible(visible);
    });

    subscribeOnce<boolean>("photomode", (photomodeEnabled) => {
        debugLog("[ZoneTools] photomode <- C#", photomodeEnabled);
        useModUIStore.getState().updatePhotomodeActive(photomodeEnabled);
    });

    subscribeOnce<boolean>("contour_enabled", (contourEnabled) => {
        debugLog("[ZoneTools] contour_enabled <- C#", contourEnabled);
        useModUIStore.setState({ contourEnabled: contourEnabled === true });
    });

    subscribeOnce<boolean>("contour_button_visible", (visible) => {
        debugLog("[ZoneTools] contour_button_visible <- C#", visible);
        useModUIStore.setState({ contourButtonVisible: visible === true });
    });

    subscribeOnce<boolean>("contour_tooloptions_visible", (visible) => {
        debugLog("[ZoneTools] contour_tooloptions_visible <- C#", visible);
        useModUIStore.setState({ contourToolOptionsVisible: visible === true });
    });

    subscribeOnce<boolean>("use_glass_panel", (enabled) => {
        debugLog("[ZoneTools] use_glass_panel <- C#", enabled);
        useModUIStore.setState({ useGlassPanel: enabled === true });
    });
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
    sendDataToCSharp(NS, "toggle_panel", true);
}

export function withStore(WrappedComponent: React.ComponentType<Partial<ModUIState>>): React.FC {
    const WithStore: React.FC = () => {
        const storeState = useModUIStore();
        return <WrappedComponent {...storeState} />;
    };

    WithStore.displayName = `WithStore(${WrappedComponent.displayName ?? WrappedComponent.name ?? "Component"})`;
    return WithStore;
}

// ----- HELPERS -----

function subscribeOnce<T>(eventName: string, callback: (value: T) => void): void {
    const eventKey = `${NS}.${eventName}`;
    if (allSubscriptions.has(eventKey)) return;

    const subscription = updateEventFromCSharp<T>(NS, eventName, callback);
    allSubscriptions.set(eventKey, subscription);
}
