# Smart Meal Planner — Web

React 18 + TypeScript + Vite SPA for the Smart Meal Planner API.

## Quick start

```bash
cd web
npm install
cp .env.example .env
npm run dev
```

- App: http://localhost:5173
- API proxy: `/api` → http://localhost:5000 (see `vite.config.ts`)

## Architecture

See [ARCHITECTURE.md](./ARCHITECTURE.md) for layer boundaries, data flow, and folder conventions.

## Stack

| Concern | Choice |
|---------|--------|
| Build | Vite 6 |
| UI | React 18 |
| Routing | React Router v6 |
| Server state | TanStack Query |
| HTTP | Axios |
