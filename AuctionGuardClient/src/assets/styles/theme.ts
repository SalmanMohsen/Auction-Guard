/**
 * A theme object for use in JS/TS components.
 * This provides a way to access design tokens directly in your component logic
 * or for configuring libraries that accept a theme object.
 */
export const theme = {
  colors: {
    primary: '#3b82f6',
    secondary: '#6b7280',
    danger: '#ef4444',
    success: '#22c55e',
    warning: '#f59e0b',
    light: '#f9fafb',
    dark: '#1f2937',
    white: '#ffffff',
    black: '#000000',
  },
  fonts: {
    sans: "Inter, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif",
    mono: "'Menlo', 'Monaco', 'Courier New', monospace",
  },
  breakpoints: {
    sm: '640px',
    md: '768px',
    lg: '1024px',
    xl: '1280px',
  },
};

// Export a type for the theme object for strong typing with TypeScript.
export type Theme = typeof theme;