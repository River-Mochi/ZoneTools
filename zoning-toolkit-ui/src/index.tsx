// src/index.tsx
// Purpose: Entry point for the Zone Tools UI (floating panel + GameTopLeft button + Tool Options injection).
// While launching game in UI development mode (include --uiDeveloperMode in the launch options)
// - Access the dev tools by opening localhost:9444 in chrome browser.
// - use the useModding() hook to access exposed UI, api and native coherent engine interfaces.


import React from "react";
import type { ModRegistrar, ModuleRegistry } from "cs2/modding";
import { ZoningToolkitPanel } from "mods/zoning-toolkit-panel";
import { ZoningToolkitMenuButton } from "./mods/zoning-toolkit-button";
import { setupSubscriptions, teardownSubscriptions } from "./mods/state";
import { ZoningToolkitToolOptions } from "./mods/zoning-toolkit-tool-options";
import { ToolOptionsVisibility } from "./mods/tool-options-visible";

const VANILLA = {
    MouseToolOptions: {
        path: "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
        exportId: "MouseToolOptions",
    },
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
) {
    try {
        registry.extend(modulePath, exportId, extension);
    } catch (err) {
        console.error(`[ZT][UI] extend failed for ${modulePath}#${exportId}`, err);
    }
}

const register: ModRegistrar = (moduleRegistry) => {
    // Mount the floating panel into the main Game UI so it can be positioned anywhere on screen.
    moduleRegistry.append("Game", () => <ZoningToolkitPanelHost />);

    // Keep the mod trigger button in GameTopLeft.
    moduleRegistry.append("GameTopLeft", () => <ZoningToolkitMenuButton />);

    // Inject contour row into the small vanilla Tool Options panel.
    extendSafe(
        moduleRegistry,
        VANILLA.MouseToolOptions.path,
        VANILLA.MouseToolOptions.exportId,
        ZoningToolkitToolOptions,
    );

    // Keep Tool Options visible while ZT ExistingRoads is active.
    extendSafe(
        moduleRegistry,
        VANILLA.ToolOptionsPanelVisible.path,
        VANILLA.ToolOptionsPanelVisible.exportId,
        ToolOptionsVisibility,
    );
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
