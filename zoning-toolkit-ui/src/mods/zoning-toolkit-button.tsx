// File: src/mods/zoning-toolkit-button.tsx
// GameTopLeft floating action button that toggles the Zone Tools panel.
// Notes:
// - Use onSelect (CS2 contract) instead of onClick.
// - Tooltip text uses Locale keys for translation.

import React, { CSSProperties } from "react";
import { Button } from "cs2/ui";
import engine from "cohtml/cohtml";

import menuIcon from "../../assets/icons/menu_icon.svg";
import { ModUIState, withStore, togglePanelFromUI } from "./state";
import VanillaBindings from "./vanilla-bindings";

const { DescriptionTooltip } = VanillaBindings.components;

const kLocale_FabTitle = "ZoneTools.UI.Fab.Title";
const kLocale_FabDesc = "ZoneTools.UI.Fab.Desc";

function translate(id: string, fallback: string): string {
    try {
        const value = engine.translate(id);
        if (!value || value === id) return fallback;
        return value;
    } catch {
        return fallback;
    }
}

class ZoningToolkitMenuButtonInternal extends React.Component<Partial<ModUIState>> {
    private handleMenuButtonSelect = (): void => {
        togglePanelFromUI();
    };

    public render(): JSX.Element | null {
        const photomodeActive = this.props.photomodeActive === true;

        const buttonStyle: CSSProperties = {
            display: photomodeActive ? "none" : undefined,
        };

        const title = translate(kLocale_FabTitle, "Zone Tools");
        const desc = translate(
            kLocale_FabDesc,
            "Modify zoning along roads.\nShortcut: Shift+X (set in Options menu).",
        );

        return (
            <DescriptionTooltip title={title} description={desc} direction="right">
                <Button
                    style={buttonStyle}
                    variant="floating"
                    src={menuIcon}
                    onSelect={this.handleMenuButtonSelect}
                />
            </DescriptionTooltip>
        );
    }
}

export const ZoningToolkitMenuButton = withStore(ZoningToolkitMenuButtonInternal);
