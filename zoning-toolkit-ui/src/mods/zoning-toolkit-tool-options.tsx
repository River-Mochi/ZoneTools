// File: src/mods/zoning-toolkit-tool-options.tsx
// Purpose: Inject a small contour row into the vanilla Tool Options panel.
// Notes:
// - Keeps Zone Tools distinct from EasyZoning: only contour lives here.
// - Zoning mode buttons stay in the floating Zone Tools panel.

import React from "react";
import type { ModuleRegistryExtend } from "cs2/modding";
import engine from "cohtml/cohtml";

import contourIcon from "../../assets/icons/contour_lines.svg";
import VanillaBindings from "./vanilla-bindings";
import { useModUIStore } from "./state";

const { ToolButton, Section } = VanillaBindings.components;

const kLocale_ContourLabel = "ZoneTools.UI.Contour";
const kLocale_Tooltip_Contour = "ZoneTools.UI.Tooltip.Contour";

function translate(id: string, fallback: string): string {
    try {
        const value = engine.translate(id);
        if (!value || value === id) return fallback;
        return value;
    } catch {
        return fallback;
    }
}

export const ZoningToolkitToolOptions: ModuleRegistryExtend = (Component: any) => {
    return (props: any) => {
        const contourEnabled = useModUIStore((s) => s.contourEnabled);
        const contourButtonVisible = useModUIStore((s) => s.contourButtonVisible);
        const contourToolOptionsVisible = useModUIStore((s) => s.contourToolOptionsVisible);
        const requestToggleContourLines = useModUIStore((s) => s.requestToggleContourLines);

        let result: any;
        try {
            result = Component(props);
        } catch (err) {
            console.error("[ZT][UI] MouseToolOptions extension failed", err);
            return null;
        }

        if (!contourButtonVisible || !contourToolOptionsVisible) {
            return result;
        }

        if (!React.isValidElement(result)) {
            return result;
        }

        if (typeof Section !== "function" || typeof ToolButton !== "function") {
            return result;
        }

        const contourSection = (
            <Section
                key="ZT_Contour"
                title={translate(kLocale_ContourLabel, "Contour")}
            >
                <div style={{ display: "flex" }}>
                    <ToolButton
                        focusKey={VanillaBindings.common.focus.disabled}
                        selected={contourEnabled}
                        src={contourIcon}
                        tooltip={translate(kLocale_Tooltip_Contour, "Terrain lines toggle.")}
                        onSelect={() => requestToggleContourLines()}
                    />
                </div>
            </Section>
        );

        const existingChildren = (result as any).props?.children;

        const mergedChildren =
            existingChildren == null
                ? [contourSection]
                : Array.isArray(existingChildren)
                    ? [...existingChildren, contourSection]
                    : [existingChildren, contourSection];

        return React.cloneElement(result as any, undefined, mergedChildren);
    };
};
