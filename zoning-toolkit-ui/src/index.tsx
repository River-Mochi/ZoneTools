// src/index.tsx
// Entry point for the Zone Tools UI (panel + GameTopLeft button).

import React from "react";
import { ModRegistrar } from "cs2/modding";
import { ZoningToolkitPanel } from "mods/zoning-toolkit-panel";
import { ZoningToolkitMenuButton } from "./mods/zoning-toolkit-button";
import { setupSubscriptions, teardownSubscriptions } from "./mods/state";

const register: ModRegistrar = (moduleRegistry) => {
    console.log("ZoningToolkit: Registering modules");

    moduleRegistry.find(".*").forEach((each) => {
        console.log(`Module: ${each}`);
    });

    // While launching game in UI development mode (include --uiDeveloperMode in the launch options)
    // - Access the dev tools by opening localhost:9444 in chrome browser.
    // - use the useModding() hook to access exposed UI, api and native coherent engine interfaces.

    // Mount the floating panel into the main Game UI so it can be positioned anywhere on screen.
    moduleRegistry.append("Game", () => <ZoningToolkitPanelHost />);

    // Keep the mod trigger button in GameTopLeft.
    moduleRegistry.append("GameTopLeft", () => <ZoningToolkitMenuButton />);
};

function ZoningToolkitPanelHost() {
    React.useEffect(() => {
        setupSubscriptions();
        return () => {
            teardownSubscriptions();
        };
    }, []);

    return <ZoningToolkitPanel />;
}

export default register;
