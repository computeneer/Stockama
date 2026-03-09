const preset = require('../tailwind.preset.cjs')

/** @type {import('tailwindcss').Config} */
module.exports = {
  ...preset,
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}', '../packages/shared/src/**/*.{js,ts,jsx,tsx}'],
}
