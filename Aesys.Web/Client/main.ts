// Red Hat Display — self-hosted via Fontsource. Vite resolves these bare
// specifiers from node_modules and bundles + hashes the woff2 files (no external
// request, no @font-face, version-pinned in package-lock). Weights match the
// design system (docs/design-system.md): 400 body · 600 eyebrow ·
// 700 headings/buttons · 800 section headings/titles · 900 hero/numerals.
import '@fontsource/red-hat-display/400.css';
import '@fontsource/red-hat-display/600.css';
import '@fontsource/red-hat-display/700.css';
import '@fontsource/red-hat-display/800.css';
import '@fontsource/red-hat-display/900.css';

import './main.css';

import.meta.glob('../Views/**/*.scss', { eager: true });
import.meta.glob<{ default?: () => void }>('../Views/**/*.ts', { eager: true });
