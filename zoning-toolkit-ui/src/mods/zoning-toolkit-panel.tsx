// File: zoning-toolkit-ui/src/mods/zoning-toolkit-panel.tsx
// Purpose: Floating panel for Zone Tools (zoning mode picker + update tool toggle).
// Notes:
// - Uses cs2/api bindings (TopoToggle pattern) instead of manual cohtml subscriptions.
// - Uses cs2/ui Portal to render as a true overlay (avoids parent layout constraints).

import React from "react";
import { Number2, Panel, PanelSection, PanelSectionRow, Portal } from "cs2/ui";
import engine from "cohtml/cohtml";

import updateToolIcon from "../../assets/icons/replace_tool_icon.svg";
import panelStyles from "./zoning-toolkit-panel.module.scss";
import VanillaBindings from "./vanilla-bindings";
import { getModeFromString, zoneModeIconMap, ZoningMode } from "./zoning-toolkit-utils";
import {
    triggerSetToolEnabled,
    triggerSetZoningMode,
    usePhotoMode,
    useToolEnabled,
    useVisible,
    useZoningMode,
} from "./zoning-toolkit-bindings";

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
    const visible = useVisible();
    const photomodeActive = usePhotoMode();
    const zoningModeString = useZoningMode();
    const isToolEnabled = useToolEnabled();

    // Stable initial position (px)
    const initialPosRef = React.useRef<Number2 | null>(null);
    if (initialPosRef.current == null) {
        const wPx = typeof window !== "undefined" ? window.innerWidth : 1920;
        const hPx = typeof window !== "undefined" ? window.innerHeight : 1080;

        const remPx = getRemPx();

        const panelWidthPx = 320 * remPx;
        const panelHeightPx = 180 * remPx;

        const rightMarginPx = 40 * remPx;
        const bottomHudClearancePx = 220 * remPx;

        initialPosRef.current = {
            x: Math.max(0, wPx - panelWidthPx - rightMarginPx),
            y: Math.max(0, hPx - panelHeightPx - bottomHudClearancePx),
        };
    }

    const currentZoningMode = getModeFromString(zoningModeString);

    const panelStyle: React.CSSProperties = {
        display: !visible || photomodeActive ? "none" : undefined,
        resize: "none",
        overflow: "hidden",
    };

    const zoningModeButtonConfigs: ZoningModeButtonConfig[] = [
        { icon: zoneModeIconMap[ZoningMode.DEFAULT], mode: ZoningMode.DEFAULT, tooltipKey: kLocale_Tooltip_ModeDefault, tooltipFallback: "Both (default)" },
        { icon: zoneModeIconMap[ZoningMode.LEFT], mode: ZoningMode.LEFT, tooltipKey: kLocale_Tooltip_ModeLeft, tooltipFallback: "Left" },
        { icon: zoneModeIconMap[ZoningMode.RIGHT], mode: ZoningMode.RIGHT, tooltipKey: kLocale_Tooltip_ModeRight, tooltipFallback: "Right" },
        { icon: zoneModeIconMap[ZoningMode.NONE], mode: ZoningMode.NONE, tooltipKey: kLocale_Tooltip_ModeNone, tooltipFallback: "None" },
    ];

    const updateRoadLabel = translate(kLocale_UpdateRoadLabel, "Update Road");
    const updateRoadTooltip = translate(
        kLocale_Tooltip_UpdateRoad,
        "Toggle update tool (for existing roads). If it won’t enable, open any road build tool once.",
    );

    return (
        <Portal>
            <Panel
                id="ZoneToolsPanel"
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
                                        onSelect={() => triggerSetZoningMode(config.mode.toString())}
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
                                onSelect={() => triggerSetToolEnabled(!isToolEnabled)}
                            />
                        }
                    />
                </PanelSection>
            </Panel>
        </Portal>
    );
};
