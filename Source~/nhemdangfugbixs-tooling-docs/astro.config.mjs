import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
  site: 'https://nhomnhem.github.io',
  base: '/NhemDangFugBixs.Tooling',
  integrations: [
    starlight({
      title: 'NhemDangFugBixs.Tooling',
      description: 'Compile-time VContainer workflow tooling for Unity projects.',
      favicon: '/logo.png',
      logo: {
        light: './public/nhem-studio-logo-light.svg',
        dark: './public/nhem-studio-logo-dark.svg',
        alt: 'NhemDangFugBixs.Tooling',
      },

      customCss: ['./src/styles/global.css'],
      head: [
        {
          tag: 'script',
          content:
            "if (typeof localStorage !== 'undefined' && !localStorage.getItem('starlight-theme')) { localStorage.setItem('starlight-theme', 'dark'); }",
        },
      ],

      social: [
        {
          icon: 'github',
          label: 'GitHub',
          href: 'https://github.com/NhomNhem/NhemDangFugBixs.Tooling',
        },
      ],

      sidebar: [
        {
          label: 'Start Here',
          items: [
            { label: 'Overview', slug: 'docs' },
            { label: 'Installation', slug: 'docs/installation' },
            { label: 'Quick Start', slug: 'docs/quick-start' },
          ],
        },
        {
          label: 'Concepts',
          items: [
            { label: 'Concepts Overview', slug: 'docs/concepts/overview' },
            { label: 'Scope Marker Pattern', slug: 'docs/concepts/scope-marker-pattern' },
            { label: 'Package Architecture', slug: 'docs/concepts/architecture' },
            { label: 'Generated Code', slug: 'docs/concepts/generated-code' },
          ],
        },
        {
          label: 'Guides',
          items: [
            { label: 'Auto Register Services', slug: 'docs/guides/auto-register' },
            { label: 'Binding Contracts', slug: 'docs/guides/binding' },
            { label: 'Entry Points', slug: 'docs/guides/entry-points' },
            { label: 'Scene Components', slug: 'docs/guides/scene-components' },
            { label: 'MessagePipe Integration', slug: 'docs/guides/messagepipe' },
            { label: 'R3 Guardrails', slug: 'docs/guides/r3' },
          ],
        },
        {
          label: 'Reference',
          items: [
            { label: 'Attributes', slug: 'docs/reference/attributes' },
            { label: 'Diagnostics', slug: 'docs/reference/diagnostics' },
            { label: 'CLI', slug: 'docs/reference/cli' },
            { label: 'Configuration', slug: 'docs/reference/configuration' },
          ],
        },
        {
          label: 'Tooling',
          items: [
            { label: 'Editor Window', slug: 'docs/tooling/editor-window' },
            { label: 'Reports', slug: 'docs/tooling/reports' },
            { label: 'Generated Files', slug: 'docs/tooling/generated-files' },
          ],
        },
        {
          label: 'Samples',
          items: [
            { label: 'Basic Auto Register', slug: 'docs/samples/basic-auto-register' },
            { label: 'Scope Marker Architecture', slug: 'docs/samples/scope-marker-architecture' },
            { label: 'Solar Phobia Style', slug: 'docs/samples/solar-phobia-style' },
          ],
        },
        {
          label: 'Troubleshooting',
          items: [
            { label: 'Troubleshooting', slug: 'docs/troubleshooting' },
          ],
        },
        {
          label: 'Roadmap',
          items: [
            { label: 'Roadmap', slug: 'docs/roadmap' },
          ],
        },
      ],
    }),
  ],

  vite: {
    plugins: [tailwindcss()],
  },
});
