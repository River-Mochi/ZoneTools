// File: src/mods/tool-options-visible.tsx
// Purpose: Keep the small vanilla Tool Options panel visible while the Zone Tools Existing Roads tool is active.
// Notes:
// - This does not inject a custom contour row.
// - Vanilla already shows the Topography row when the active tool supports contour lines.

import { useValue } from "cs2/api";
import { tool } from "cs2/bindings";

type UseToolOptionsVisible = (...args: any[]) => boolean;
type ExtendHook<T extends (...args: any[]) => any> = (original: T) => T;

const ZT_TOOL_ID = "ZoningToolkit.ExistingRoads";

export const ToolOptionsVisibility: ExtendHook<UseToolOptionsVisible> = (useToolOptionsVisible) => {
    const vanillaFnOk = typeof useToolOptionsVisible === "function";

    if (!vanillaFnOk) {
        console.error("[ZT][UI] useToolOptionsVisible missing or not a function.");
    }

    return (...args: any[]) => {
        const vanillaVisible = vanillaFnOk ? !!useToolOptionsVisible(...args) : false;
        const activeId = useValue(tool.activeTool$)?.id;
        const ours = activeId === ZT_TOOL_ID;

        // Keep the mini Tool Options panel open for:
        // - real Update Road mode
        // - contour host mode
        return vanillaVisible || ours;
    };
};
