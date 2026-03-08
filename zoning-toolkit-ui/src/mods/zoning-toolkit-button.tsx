// src/mods/zoning-toolkit-button.tsx
// GameTopLeft floating action button that toggles the Zone Tools panel.
// Notes:
// - Use onSelect (CS2 contract) instead of onClick.
// - Use Button src= so vanilla sizes the icon; no custom SCSS required.

import React, { CSSProperties } from "react";
import { Button } from "cs2/ui";

import menuIcon from "../../assets/icons/menu_icon.svg";
import { ModUIState, withStore, togglePanelFromUI } from "./state";
import VanillaBindings from "./vanilla-bindings";

const { DescriptionTooltip } = VanillaBindings.components;

class ZoningToolkitMenuButtonInternal extends React.Component<Partial<ModUIState>> {
    private handleMenuButtonSelect = (): void => {
        togglePanelFromUI();
    };

    public render(): JSX.Element | null {
        const photomodeActive = !!this.props.photomodeActive;

        const buttonStyle: CSSProperties = {
            display: photomodeActive ? "none" : undefined,
        };

        return (
            <DescriptionTooltip
                description="Control/modify zoning along roads. Allows zoning on both sides of roads, on any one side, or no sides at all."
                direction="right"
                title="Zone Tools"
            >
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
