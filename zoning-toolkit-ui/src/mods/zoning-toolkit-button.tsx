// File: zoning-toolkit-ui/src/mods/zoning-toolkit-button.tsx
// Purpose: GameTopLeft floating action button that toggles the Zone Tools panel.

import React, { CSSProperties } from "react";
import { Button } from "cs2/ui";

import menuIcon from "../../assets/icons/menu_icon.svg";
import menuButtonStyles from "./zoning-toolkit-button.module.scss";
import VanillaBindings from "./vanilla-bindings";
import { triggerTogglePanel, usePhotoMode } from "./zoning-toolkit-bindings";

const { DescriptionTooltip } = VanillaBindings.components;

export const ZoningToolkitMenuButton: React.FC = () => {
    const photomodeActive = usePhotoMode();

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
                onSelect={triggerTogglePanel}
            >
                <img
                    src={menuIcon}
                    className={menuButtonStyles.menuIcon}
                />
            </Button>
        </DescriptionTooltip>
    );
};
