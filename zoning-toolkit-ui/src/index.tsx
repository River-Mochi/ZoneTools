// File: zoning-toolkit-ui/src/index.tsx
// Purpose: Entry point for the Zone Tools UI (panel + GameTopLeft button).

import React from "react";
import { ModRegistrar } from "cs2/modding";
import { ZoningToolkitPanel } from "mods/zoning-toolkit-panel";
import { ZoningToolkitMenuButton } from "./mods/zoning-toolkit-button";

const register: ModRegistrar = (moduleRegistry) => {
    console.log("ZoningToolkit: Registering modules");

    // Floating panel (Portal inside panel component handles overlay placement).
    moduleRegistry.append("Game", () => <ZoningToolkitPanel />);

    // Trigger button in GameTopLeft.
    moduleRegistry.append("GameTopLeft", () => <ZoningToolkitMenuButton />);
};

export default register;
