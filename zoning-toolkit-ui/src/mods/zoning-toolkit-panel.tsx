// src/mods/zoning-toolkit-panel.tsx
// Floating panel for Zone Tools (zoning mode picker + update tool toggle).
// Zone Tools header stays English; row label + tooltips are localized via engine.translate() keys.

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

interface ZoningModeButtonConfig {
    icon: string;
    mode: ZoningMode;
    tooltipKey: string;
    tooltipFallback: string;
}

function getRemPixels(): number {
    try {
        if (typeof window === "undefined" || typeof document === "undefined") {
            return 1;
        }
        const fontSize = window.getComputedStyle(document.documentElement).fontSize;
        const px = parseFloat(fontSize);
        return Number.isFinite(px) && px > 0 ? px : 1;
    } catch {
        return 1;
    }
}

export const ZoningToolkitPanel: React.FC = () => {
    const store = useModUIStore();

    const initialPosRef = React.useRef<Number2 | null>(null);
    if (initialPosRef.current == null) {
        const fallbackWidthPx = 1920;
        const fallbackHeightPx = 1080;

        const wPx = typeof window !== "undefined" ? window.innerWidth : fallbackWidthPx;
        const hPx = typeof window !== "undefined" ? window.innerHeight : fallbackHeightPx;

        const remPx = getRemPixels();
        const wRem = wPx / remPx;
        const hRem = hPx / remPx;

        const panelWidthRem = 320;
        const rightMarginRem = 40;
        const bottomHudClearanceRem = 220;

        initialPosRef.current = {
            x: Math.max(0, wRem - panelWidthRem - rightMarginRem),
            y: Math.max(0, hRem - bottomHudClearanceRem),
        };
    }

    const currentZoningMode = getModeFromString(store.zoningMode);
    const isToolEnabled = store.isToolEnabled;

    const panelStyle = {
        display: !store.uiVisible || store.photomodeActive ? "none" : undefined,
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
