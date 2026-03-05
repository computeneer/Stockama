# Stockama Monorepo

Bu repo monorepo yapisinda duzenlendi:

- `backend/`: .NET Core backend cozumunun tamami
- `frontend/admin`: React + Vite + Tailwind admin uygulamasi
- `frontend/web`: React + Vite + Tailwind web uygulamasi
- `frontend/packages/shared`: Admin ve web tarafinin ortak kullandigi paket

## Klasor yapisi

```text
.
├── backend
│   ├── Stockama.slnx
│   └── ...
├── frontend
│   ├── admin
│   ├── web
│   ├── tailwind.preset.cjs
│   └── packages
│       └── shared
│           └── src
│               ├── api
│               ├── hooks
│               ├── components
│               └── types
└── package.json
```

## Paket yonetimi (bun)

Bu repoda frontend package manager olarak `bun` hedeflenir.

```bash
bun install
```

## Frontend komutlari

```bash
bun run dev:admin
bun run dev:web
bun run build:frontend
```

## Backend komutlari

```bash
dotnet build ./backend/Stockama.slnx
dotnet test ./backend/Stockama.slnx
```

## Tailwind + Theme

Hem `admin` hem `web` projelerinde:

- TailwindCSS aktif
- `darkMode: 'class'` konfiguru aktif
- ortak tema toggle mekanizmasi `@stockama/shared` icindeki `useTheme` ve `ThemeToggleButton` ile kullaniliyor

## Shared package icerigi

`@stockama/shared` su modulleri disari acar:

- `types`: backend API request/response tipleri
- `api`: `ApiClient` ve endpoint bazli API helper'lari
- `hooks`: `useApiQuery`, `useApiMutation`, `useTheme`
- `components`: ortak UI componentleri

## API type yapisi (request/response ayrimi)

Request ve response tipleri endpoint bazinda ayri klasorlerde tutulur:

```text
frontend/packages/shared/src/types/api/
├── auth/
│   ├── request.ts
│   └── response.ts
├── company/
│   ├── request.ts
│   └── response.ts
└── common/
    ├── request.ts
    └── response.ts
```

Ornek:

- `frontend/packages/shared/src/types/api/company/request.ts`
- `frontend/packages/shared/src/types/api/company/response.ts`
