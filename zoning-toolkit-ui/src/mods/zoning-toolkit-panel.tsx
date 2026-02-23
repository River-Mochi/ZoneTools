// File: zoning-toolkit-ui/src/mods/zoning-toolkit-panel.tsx
// Purpose: Floating panel for Zone Tools (zoning mode picker + update tool toggle).

import React from "react";
import { Number2, Panel, PanelSection, PanelSectionRow } from "cs2/ui";
import engine from "cohtml/cohtml";

import updateToolIcon from "../../assets/icons/replace_tool_icon.svg";
import { useModUIStore } from "./state";
import panelStyles from "./zoning-toolkit-panel.module.scss";
import VanillaBindings from "./vanilla-bindings";
import { getModeFromString, zoneModeIconMap, ZoningMode } from "./zoning-toolkit-utils";

const { ToolButton } = VanillaBindings.components;

// Locale keys (provided by C# Locale*.cs)
const kLocale_UpdateRoadLabel = "ZoneTools.UI.UpdateRoad";
const kLocale_Tooltip_UpdateRoad = "ZoneTools.UI.Tooltip.UpdateRoad";
const kLocale_Tooltip_ModeDefault = "ZoneTools.UI.Tooltip.ModeDefault";
const kLocale_Tooltip_ModeLeft = "ZoneTools.UI.Tooltip.ModeLeft";
const kLocale_Tooltip_ModeRight = "ZoneTools.UI.Tooltip.ModeRight";
const kLocale_Tooltip_ModeNone = "ZoneTools.UI.Tooltip.ModeNone";

function translate(id: string, fallback: string): string {
    try {
        const value = engine.translate(id);
        if (!value || value === id) {
            return fallback;
        }
        return value;
    } catch {
        return fallback;
    }
}

function getRemPx(): number {
    try {
        const fs = window.getComputedStyle(document.documentElement).fontSize;
        const px = parseFloat(fs);
        return Number.isFinite(px) && px > 0 ? px : 16;
    } catch {
        return 16;
    }
}

interface ZoningModeButtonConfig {
    icon: string;
    mode: ZoningMode;
    tooltipKey: string;
    tooltipFallback: string;
}

export const ZoningToolkitPanel: React.FC = () => {
    const store = useModUIStore();

    // Compute a stable initial position once per session.
    const initialPosRef = React.useRef<Number2 | null>(null);
    if (initialPosRef.current == null) {
        const wPx = typeof window !== "undefined" ? window.innerWidth : 1920;
        const hPx = typeof window !== "undefined" ? window.innerHeight : 1080;

        const remPx = getRemPx();

        // Match SCSS sizing (rem → px)
        const panelWidthPx = 320 * remPx;
        const panelHeightPx = 180 * remPx;

        // “Keep off the bottom HUD” margin (rem → px)
        const rightMarginPx = 40 * remPx;
        const bottomHudClearancePx = 220 * remPx;

        initialPosRef.current = {
            x: Math.max(0, wPx - panelWidthPx - rightMarginPx),
            y: Math.max(0, hPx - panelHeightPx - bottomHudClearancePx),
        };
    }

    const currentZoningMode = getModeFromString(store.zoningMode);
    const isToolEnabled = store.isToolEnabled;

    const panelStyle: React.CSSProperties = {
        display: !store.uiVisible || store.photomodeActive ? "none" : undefined,
        resize: "none",
        overflow: "hidden",
        // Belt-and-suspenders sizing in case the Panel theme fights the className
        width: "320rem",
        maxWidth: "320rem",
        minWidth: "320rem",
        maxHeight: "180rem",
    };

    const zoningModeButtonConfigs: ZoningModeButtonConfig[] = [
        { icon: zoneModeIconMap[ZoningMode.DEFAULT], mode: ZoningMode.DEFAULT, tooltipKey: kLocale_Tooltip_ModeDefault, tooltipFallback: "Default (both)" },
        { icon: zoneModeIconMap[ZoningMode.LEFT], mode: ZoningMode.LEFT, tooltipKey: kLocale_Tooltip_ModeLeft, tooltipFallback: "Left" },
        { icon: zoneModeIconMap[ZoningMode.RIGHT], mode: ZoningMode.RIGHT, tooltipKey: kLocale_Tooltip_ModeRight, tooltipFallback: "Right" },
        { icon: zoneModeIconMap[ZoningMode.NONE], mode: ZoningMode.NONE, tooltipKey: kLocale_Tooltip_ModeNone, tooltipFallback: "None" },
    ];

    const updateRoadLabel = translate(kLocale_UpdateRoadLabel, "Update Road");
    const updateRoadTooltip = translate(
        kLocale_Tooltip_UpdateRoad,
        "Toggle update tool (for existing roads). Roads with zoned buildings are skipped.",
    );

    return (
        <Panel
            draggable
            initialPosition={initialPosRef.current!}
            className={panelStyles.panel}
            contentClassName={panelStyles.content}
            header="Zone Tools"
            style={panelStyle}
        >
            <PanelSection>
                <PanelSectionRow
                    left={null}
                    right={
                        <div className={panelStyles.panelToolModeRow}>
                            {zoningModeButtonConfigs.map((config) => (
                                <ToolButton
                                    key={config.mode}
                                    focusKey={VanillaBindings.common.focus.disabled}
                                    selected={currentZoningMode === config.mode}
                                    src={config.icon}
                                    tooltip={translate(config.tooltipKey, config.tooltipFallback)}
                                    onSelect={() => store.updateZoningMode(config.mode.toString())}
                                />
                            ))}
                        </div>
                    }
                />

                <PanelSectionRow
                    left={<span className={panelStyles.rowLabelNoWrap}>{updateRoadLabel}</span>}
                    right={
                        <ToolButton
                            focusKey={VanillaBindings.common.focus.disabled}
                            selected={isToolEnabled}
                            src={updateToolIcon}
                            tooltip={updateRoadTooltip}
                            onSelect={() => store.updateIsToolEnabled(!isToolEnabled)}
                        />
                    }
                />
            </PanelSection>
        </Panel>
    );
};
