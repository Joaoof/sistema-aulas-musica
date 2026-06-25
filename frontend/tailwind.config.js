/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: [
    './index.html',
    './src/**/*.{ts,tsx}',
    // Tremor precisa ler suas próprias classes
    './node_modules/@tremor/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        // ── Shadcn/ui (tema Zinc, via CSS vars) ──────────────
        border: 'hsl(var(--border))',
        input: 'hsl(var(--input))',
        ring: 'hsl(var(--ring))',
        background: 'hsl(var(--background))',
        foreground: 'hsl(var(--foreground))',
        primary: {
          DEFAULT: 'hsl(var(--primary))',
          foreground: 'hsl(var(--primary-foreground))',
        },
        secondary: {
          DEFAULT: 'hsl(var(--secondary))',
          foreground: 'hsl(var(--secondary-foreground))',
        },
        destructive: {
          DEFAULT: 'hsl(var(--destructive))',
          foreground: 'hsl(var(--destructive-foreground))',
        },
        muted: {
          DEFAULT: 'hsl(var(--muted))',
          foreground: 'hsl(var(--muted-foreground))',
        },
        accent: {
          DEFAULT: 'hsl(var(--accent))',
          foreground: 'hsl(var(--accent-foreground))',
        },
        card: {
          DEFAULT: 'hsl(var(--card))',
          foreground: 'hsl(var(--card-foreground))',
        },

        // ── Tremor (dark-first) ──────────────────────────────
        tremor: {
          brand: {
            faint: '#0B1229',
            muted: '#172554',
            subtle: '#1e40af',
            DEFAULT: '#f59e0b', // amber-500 (acento Pro Audio)
            emphasis: '#fbbf24',
            inverted: '#030712',
          },
          background: {
            muted: '#131A2B',
            subtle: '#18181b', // zinc-900
            DEFAULT: '#09090b', // zinc-950
            emphasis: '#a1a1aa',
          },
          border: { DEFAULT: '#27272a' }, // zinc-800
          ring: { DEFAULT: '#27272a' },
          content: {
            subtle: '#52525b',
            DEFAULT: '#71717a',
            emphasis: '#d4d4d8',
            strong: '#fafafa',
            inverted: '#09090b',
          },
        },
      },
      borderRadius: {
        lg: 'var(--radius)',
        md: 'calc(var(--radius) - 2px)',
        sm: 'calc(var(--radius) - 4px)',
        'tremor-small': '0.375rem',
        'tremor-default': '0.5rem',
        'tremor-full': '9999px',
      },
      fontSize: {
        'tremor-label': ['0.75rem', { lineHeight: '1rem' }],
        'tremor-default': ['0.875rem', { lineHeight: '1.25rem' }],
        'tremor-title': ['1.125rem', { lineHeight: '1.75rem' }],
        'tremor-metric': ['1.875rem', { lineHeight: '2.25rem' }],
      },
      boxShadow: {
        'tremor-card': '0 1px 3px 0 rgb(0 0 0 / 0.4)',
        'tremor-dropdown': '0 4px 6px -1px rgb(0 0 0 / 0.5)',
        glow: '0 0 0 1px rgba(245, 158, 11, 0.25), 0 8px 40px -8px rgba(245, 158, 11, 0.35)',
      },
      keyframes: {
        'accordion-down': {
          from: { height: '0' },
          to: { height: 'var(--radix-accordion-content-height)' },
        },
        'accordion-up': {
          from: { height: 'var(--radix-accordion-content-height)' },
          to: { height: '0' },
        },
      },
      animation: {
        'accordion-down': 'accordion-down 0.2s ease-out',
        'accordion-up': 'accordion-up 0.2s ease-out',
      },
    },
  },
  // Tremor aplica classes de cor dinamicamente -> precisam estar na safelist
  safelist: [
    {
      pattern:
        /^(bg|text|border|ring|fill|stroke)-(amber|emerald|zinc|blue)-(50|100|200|300|400|500|600|700|800|900|950)$/,
      variants: ['hover'],
    },
    ...['amber', 'emerald', 'blue', 'zinc'].flatMap((c) => [
      `bg-${c}-500`,
      `fill-${c}-500`,
      `stroke-${c}-500`,
    ]),
  ],
  plugins: [require('tailwindcss-animate')],
}
