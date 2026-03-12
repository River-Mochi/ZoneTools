// File: src/mods/zoning-toolkit-panel.tsx
// Zone Tools floating panel.
//
// Layout:
// - Transparent custom title bar (drag handle)
// - Row 1: Both / Left / Right / None
// - Row 2: Update Road + Contour, grouped on the right
//
// Notes:
// - Uses React Draggable, not cs2/ui draggable.
// - Dragging is restricted to the title bar only.
// - Vanilla ToolButton still handles hover/selected/tooltip visuals.

import React from "react";
import Draggable from "react-draggable";
import { Panel } from "cs2/ui";
import engine from "cohtml/cohtml";

import updateToolIcon from "../../assets/icons/replace_tool_icon.svg";
import contourIcon from "../../assets/icons/contour_lines.svg";

import { ModUIState, useModUIStore, withStore } from "./state";
import panelStyles from "./zoning-toolkit-panel.module.scss";
import VanillaBindings from "./vanilla-bindings";
import { getModeFromString, zoneModeIconMap, ZoningMode } from "./zoning-toolkit-utils";

const { ToolButton } = VanillaBindings.components;

const kLocale_Tooltip_UpdateRoad = "ZoneTools.UI.Tooltip.UpdateRoad";
const kLocale_Tooltip_ModeDefault = "ZoneTools.UI.Tooltip.ModeDefault";
const kLocale_Tooltip_ModeLeft = "ZoneTools.UI.Tooltip.ModeLeft";
const kLocale_Tooltip_ModeRight = "ZoneTools.UI.Tooltip.ModeRight";
const kLocale_Tooltip_ModeNone = "ZoneTools.UI.Tooltip.ModeNone";
const kLocale_Tooltip_Contour = "ZoneTools.UI.Tooltip.Contour";
const kLocale_Tooltip_TitleBar = "ZoneTools.UI.Tooltip.TitleBar";

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

export class ZoningToolkitPanelInternal extends React.Component<Partial<ModUIState>> {
    private handleZoneModeSelect(zoningMode: ZoningMode): void {
        if (this.props.updateZoningMode) {
            this.props.updateZoningMode(zoningMode.toString());
            return;
        }

        useModUIStore.getState().updateZoningMode(zoningMode.toString());
    }

    private handleZoneToolSelect(enabled: boolean): void {
        if (this.props.requestToolEnabled) {
            this.props.requestToolEnabled(enabled);
            return;
        }

        useModUIStore.getState().requestToolEnabled(enabled);
    }

    private handleContourSelect(): void {
        if (this.props.requestToggleContourLines) {
            this.props.requestToggleContourLines();
            return;
        }

        useModUIStore.getState().requestToggleContourLines();
    }

    public render(): JSX.Element | null {
        const zoningModeString = this.props.zoningMode ?? "Default";
        const currentZoningMode = getModeFromString(zoningModeString);

        const isToolEnabled = this.props.isToolEnabled === true;
        const contourEnabled = this.props.contourEnabled === true;
        const contourButtonVisible = this.props.contourButtonVisible !== false;

        const uiVisible = this.props.uiVisible === true;
        const photomodeActive = this.props.photomodeActive === true;

        const panelStyle = {
            display: !uiVisible || photomodeActive ? "none" : undefined,
        };

        const zoningModeButtonConfigs: ZoningModeButtonConfig[] = [
            {
                icon: zoneModeIconMap[ZoningMode.DEFAULT],
                mode: ZoningMode.DEFAULT,
                tooltipKey: kLocale_Tooltip_ModeDefault,
                tooltipFallback: "Both (default)",
            },
            {
                icon: zoneModeIconMap[ZoningMode.LEFT],
                mode: ZoningMode.LEFT,
                tooltipKey: kLocale_Tooltip_ModeLeft,
                tooltipFallback: "Left only",
            },
            {
                icon: zoneModeIconMap[ZoningMode.RIGHT],
                mode: ZoningMode.RIGHT,
                tooltipKey: kLocale_Tooltip_ModeRight,
                tooltipFallback: "Right only",
            },
            {
                icon: zoneModeIconMap[ZoningMode.NONE],
                mode: ZoningMode.NONE,
                tooltipKey: kLocale_Tooltip_ModeNone,
                tooltipFallback: "None",
            },
        ];

        const updateRoadTooltip = translate(
            kLocale_Tooltip_UpdateRoad,
            "Toggle update panel ON / OFF (for existing roads).",
        );

        const contourTooltip = translate(
            kLocale_Tooltip_Contour,
            "Terrain lines toggle.",
        );

        const titleBarTooltip = translate(
            kLocale_Tooltip_TitleBar,
            "Drag panel from the title bar.",
        );

        return (
            <Draggable
                bounds="parent"
                grid={[5, 5]}
                enableUserSelectHack={false}
                handle={`.${panelStyles.header}`}
            >
                <Panel
                    className={panelStyles.panel}
                    style={panelStyle}
                    header={
                        <div
                            className={panelStyles.header}
                            title={titleBarTooltip}
                        >
                            <div className={panelStyles.headerText}>Zone Tools</div>
                        </div>
                    }
                >
                    <div className={panelStyles.body}>
                        <div className={panelStyles.rowBlock}>
                            <div className={panelStyles.topRow}>
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

                            <div className={panelStyles.bottomRow}>
                                <div className={panelStyles.bottomUpdate}>
                                    <ToolButton
                                        focusKey={VanillaBindings.common.focus.disabled}
                                        selected={isToolEnabled}
                                        src={updateToolIcon}
                                        tooltip={updateRoadTooltip}
                                        onSelect={() => this.handleZoneToolSelect(!isToolEnabled)}
                                    />
                                </div>

                                {contourButtonVisible ? (
                                    <div className={panelStyles.bottomContour}>
                                        <ToolButton
                                            focusKey={VanillaBindings.common.focus.disabled}
                                            selected={contourEnabled}
                                            src={contourIcon}
                                            tooltip={contourTooltip}
                                            onSelect={() => this.handleContourSelect()}
                                        />
                                    </div>
                                ) : null}
                            </div>
                        </div>
                    </div>
                </Panel>
            </Draggable>
        );
    }
}

export const ZoningToolkitPanel = withStore(ZoningToolkitPanelInternal);
