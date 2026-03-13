// File: src/index.tsx
// Purpose: Entry point for the Zone Tools UI.
// Notes:
// - Mounts the floating panel and the top-left button.
// - Keeps vanilla Tool Options visible while the ZT Existing Roads tool is active.
// - Does not inject a second contour row into MouseToolOptions.

import React from "react";
import { ModRegistrar, ModuleRegistry } from "cs2/modding";
import { ZoningToolkitPanel } from "mods/zoning-toolkit-panel";
import { ZoningToolkitMenuButton } from "./mods/zoning-toolkit-button";
import { ToolOptionsVisibility } from "./mods/tool-options-visible";
import { setupSubscriptions, teardownSubscriptions } from "./mods/state";

const VANILLA = {
    ToolOptionsPanelVisible: {
        path: "game-ui/game/components/tool-options/tool-options-panel.tsx",
        exportId: "useToolOptionsVisible",
    },
};

function extendSafe(
    registry: ModuleRegistry,
    modulePath: string,
    exportId: string,
    extension: any,
): void {
    try {
        registry.extend(modulePath, exportId, extension);
    } catch (err) {
        console.error(`[ZT][UI] extend failed for ${modulePath}#${exportId}`, err);
    }
}

const register: ModRegistrar = (moduleRegistry) => {
    // Mount the floating panel into the main Game UI.
    moduleRegistry.append("Game", () => <ZoningToolkitPanelHost />);

    // Keep the mod trigger button in GameTopLeft.
    moduleRegistry.append("GameTopLeft", () => <ZoningToolkitMenuButton />);

    // Keep the small vanilla Tool Options panel visible while the ZT tool is active.
    extendSafe(
        moduleRegistry,
        VANILLA.ToolOptionsPanelVisible.path,
        VANILLA.ToolOptionsPanelVisible.exportId,
        ToolOptionsVisibility,
    );

    console.log("ZoneTools UI module registrations completed.");

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
