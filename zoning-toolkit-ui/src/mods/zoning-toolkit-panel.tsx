// File: src/mods/zoning-toolkit-panel.tsx
// Zone Tools floating panel.
//
// Layout:
// - Transparent custom title bar (drag handle)
// - Row 1: Both / Left / Right / None
// - Row 2: Update Road + Contour
//
// Notes:
// - Uses React Draggable, not cs2/ui draggable.
// - Dragging is restricted to the title bar only.
// - grid={[5, 5]} is drag snap in JS; it is NOT CSS grid.
// - Vanilla ToolButton still handles hover/selected/tooltip visuals.
// - Title bar tooltip is explicit; locale entry alone does not make a tooltip appear.
// - bottomRowLeft is the current default; includes option to Swap to bottomRowRight later.

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

const { ToolButton, DescriptionTooltip } = VanillaBindings.components;

// Locale keys
const kLocale_Title = "ZoneTools.UI.Fab.Title";
const kLocale_Tooltip_TitleBar = "ZoneTools.UI.Tooltip.TitleBar";
const kLocale_Tooltip_UpdateRoad = "ZoneTools.UI.Tooltip.UpdateRoad";
const kLocale_Tooltip_ModeDefault = "ZoneTools.UI.Tooltip.ModeDefault";
const kLocale_Tooltip_ModeLeft = "ZoneTools.UI.Tooltip.ModeLeft";
const kLocale_Tooltip_ModeRight = "ZoneTools.UI.Tooltip.ModeRight";
const kLocale_Tooltip_ModeNone = "ZoneTools.UI.Tooltip.ModeNone";
const kLocale_Tooltip_Contour = "ZoneTools.UI.Tooltip.Contour";

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
        const useGlassPanel = this.props.useGlassPanel !== false;

        const uiVisible = this.props.uiVisible === true;
        const photomodeActive = this.props.photomodeActive === true;

        const panelStyle = {
            display: !uiVisible || photomodeActive ? "none" : undefined,
        };

        const panelClassName = [
            panelStyles.panel,
            useGlassPanel ? panelStyles.panelGlass : panelStyles.panelVanilla,
        ].join(" ");

        const zoningModeButtonConfigs: ZoningModeButtonConfig[] = [
            {
                icon: zoneModeIconMap[ZoningMode.DEFAULT],
                mode: ZoningMode.DEFAULT,
                tooltipKey: kLocale_Tooltip_ModeDefault,
                tooltipFallback: "Both sides",
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

        const titleText = translate(kLocale_Title, "Zone Tools");

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
                grid={[1, 1]}
                enableUserSelectHack={false}
                handle='[class*="title-bar_"]'
            >
                <Panel
                    className={panelClassName}
                    style={panelStyle}
                    header={
                        <DescriptionTooltip
                            title={titleText}
                            description={titleBarTooltip}
                        >
                            <div className={panelStyles.titleTooltipAnchor}>
                                <div className={panelStyles.header}>
                                    <div className={panelStyles.headerText}>{titleText}</div>
                                </div>
                            </div>
                        </DescriptionTooltip>
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

                            <div className={`${panelStyles.bottomRow} ${panelStyles.bottomRowLeft}`}>
                                <ToolButton
                                    focusKey={VanillaBindings.common.focus.disabled}
                                    selected={isToolEnabled}
                                    src={updateToolIcon}
                                    tooltip={updateRoadTooltip}
                                    onSelect={() => this.handleZoneToolSelect(!isToolEnabled)}
                                />

                                {contourButtonVisible ? (
                                    <ToolButton
                                        focusKey={VanillaBindings.common.focus.disabled}
                                        selected={contourEnabled}
                                        src={contourIcon}
                                        tooltip={contourTooltip}
                                        onSelect={() => this.handleContourSelect()}
                                    />
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
