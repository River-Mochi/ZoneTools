// src/mods/zoning-toolkit-panel.tsx
// Floating panel for Zone Tools (zoning mode picker + update tool toggle).
// Zone Tools header stays English; row label + tooltips are localized via engine.translate() keys.

import React from "react";
import { Panel, PanelSection, PanelSectionRow } from "cs2/ui";
import engine from "cohtml/cohtml";

import updateToolIcon from "../../assets/icons/replace_tool_icon.svg";
import { useModUIStore, withStore } from "./state";
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

export class ZoningToolkitPanelInternal extends React.Component {
    private handleZoneModeSelect(zoningMode: ZoningMode): void {
        useModUIStore.getState().updateZoningMode(zoningMode.toString());
    }

    private handleZoneToolSelect(enabled: boolean): void {
        useModUIStore.getState().updateIsToolEnabled(enabled);
    }

    public render(): JSX.Element | null {
        const store = useModUIStore.getState();
        const currentZoningMode = getModeFromString(store.zoningMode);
        const isToolEnabled = store.isToolEnabled;
        const uiVisible = store.uiVisible;
        const photomodeActive = store.photomodeActive;

        // Hide in photo mode or when not visible (Shift+Z / FAB toggle driven by C# -> UI state)
        const panelStyle = {
            display: !uiVisible || photomodeActive ? "none" : undefined,
        };

        const zoningModeButtonConfigs: ZoningModeButtonConfig[] = [
            {
                icon: zoneModeIconMap[ZoningMode.DEFAULT],
                mode: ZoningMode.DEFAULT,
                tooltipKey: kLocale_Tooltip_ModeDefault,
                tooltipFallback: "Default (both)",
            },
            {
                icon: zoneModeIconMap[ZoningMode.LEFT],
                mode: ZoningMode.LEFT,
                tooltipKey: kLocale_Tooltip_ModeLeft,
                tooltipFallback: "Left",
            },
            {
                icon: zoneModeIconMap[ZoningMode.RIGHT],
                mode: ZoningMode.RIGHT,
                tooltipKey: kLocale_Tooltip_ModeRight,
                tooltipFallback: "Right",
            },
            {
                icon: zoneModeIconMap[ZoningMode.NONE],
                mode: ZoningMode.NONE,
                tooltipKey: kLocale_Tooltip_ModeNone,
                tooltipFallback: "None",
            },
        ];

        const updateRoadLabel = translate(kLocale_UpdateRoadLabel, "Update Road");
        const updateRoadTooltip = translate(
            kLocale_Tooltip_UpdateRoad,
            "Toggle update tool (for existing roads). Roads with zoned buildings are skipped.",
        );

        // Best-effort default position near bottom-right.
        // CS2 UI coordinates are effectively “px-ish”; tweak offsets if needed.
        const initialPosition = {
            x: Math.max(0, window.innerWidth - 520),
            y: Math.max(0, window.innerHeight - 360),
        };

        return (
            <Panel
                draggable
                initialPosition={initialPosition}
                className={panelStyles.panel}
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
                                        onSelect={() => this.handleZoneModeSelect(config.mode)}
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
                                onSelect={() => this.handleZoneToolSelect(!isToolEnabled)}
                            />
                        }
                    />
                </PanelSection>
            </Panel>
        );
    }
}

export const ZoningToolkitPanel = withStore(ZoningToolkitPanelInternal);
