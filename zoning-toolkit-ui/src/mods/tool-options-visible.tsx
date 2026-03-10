// File: src/mods/tool-options-visible.tsx
// Purpose: Keep the vanilla Tool Options panel visible while Zone Tools ExistingRoads is active.

import { useValue } from "cs2/api";
import { tool } from "cs2/bindings";

type UseToolOptionsVisible = (...args: any[]) => boolean;
type ExtendHook<T extends (...args: any[]) => any> = (original: T) => T;

const ZT_TOOL_ID = "ZoningToolkit.ExistingRoads";

export const ToolOptionsVisibility: ExtendHook<UseToolOptionsVisible> = (useToolOptionsVisible) => {
    const vanillaFnOk = typeof useToolOptionsVisible === "function";

    return (...args: any[]) => {
        const vanillaVisible = vanillaFnOk ? !!useToolOptionsVisible(...args) : false;
        const activeId = useValue(tool.activeTool$)?.id;
        const ours = activeId === ZT_TOOL_ID;

        return vanillaVisible || ours;
    };
};
