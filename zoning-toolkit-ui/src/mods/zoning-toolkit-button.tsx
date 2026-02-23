// File: zoning-toolkit-ui/src/mods/zoning-toolkit-button.tsx
// Purpose: GameTopLeft floating action button that toggles the Zone Tools panel.

import React, { CSSProperties } from "react";
import { Button } from "cs2/ui";

import menuIcon from "../../assets/icons/menu_icon.svg";
import menuButtonStyles from "./zoning-toolkit-button.module.scss";
import { useModUIStore, withStore, togglePanelFromUI } from "./state";
import VanillaBindings from "./vanilla-bindings";

const { DescriptionTooltip } = VanillaBindings.components;

class ZoningToolkitMenuButtonInternal extends React.Component {
    private handleMenuButtonActivate = (): void => {
        togglePanelFromUI();
    };

    public render(): JSX.Element | null {
        const store = useModUIStore.getState();
        const photomodeActive = store.photomodeActive;

        const buttonStyle: CSSProperties = {
            display: photomodeActive ? "none" : undefined,
        };

        return (
            <DescriptionTooltip
                description="Control/modify zoning along roads. Shortcut: Shift + Z"
                direction="right"
                title="Zone Tools"
            >
                <Button
                    style={buttonStyle}
                    variant="floating"
                    // Support both paths; different vanilla buttons fire different callbacks.
                    onSelect={this.handleMenuButtonActivate}
                    onClick={this.handleMenuButtonActivate}
                >
                    <img src={menuIcon} className={menuButtonStyles.menuIcon} />
                </Button>
            </DescriptionTooltip>
        );
    }
}

export const ZoningToolkitMenuButton = withStore(ZoningToolkitMenuButtonInternal);
