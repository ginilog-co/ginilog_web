# Ginilog Sign-In on Render — What to Do Next

This document summarizes what was wrong, what was changed in the repo, and the steps to finish deployment so **customer** and **company** sign-in work with the Render API.

---

## What was wrong

1. **Frontend API routing** — The SPA called same-origin `/api/*` in production, but static hosts have no Next.js proxy. When `NEXT_PUBLIC_API_URL` was missing at build time, rewrites pointed at `http://localhost:5000`, so login never reached Render.
2. **CORS** — The API allowed `localhost:3000` and `ginilog-web.vercel.app`, but **not** other `*.onrender.com` frontends. Browser sign-in from a Render-hosted UI was blocked.
3. **Email (verification)** — `ResendApiKey` and `MailSettings__Mail` were not listed in `render.yaml`; unverified users may not receive codes on production.

**Production API URL (confirmed healthy):** `https://api-data-connection.ginilog.org`

---

## What was changed in the repo

| File | Change |
|------|--------|
| `ginilog-spa/lib/api.ts` | Production browsers call Render API directly when env is unset; localhost still uses `/api` rewrites. |
| `ginilog-spa/next.config.js` | Rewrites default to `https://api-data-connection.ginilog.org` instead of `localhost:5000`. |
| `ginilog-spa/.env.example` | Documents `NEXT_PUBLIC_API_URL` and `NEXT_PUBLIC_WS_URL`. |
| `Genilog_WebApi/Program.cs` | CORS allows `*.onrender.com`, `*.vercel.app`, `*.ginilog.com`, and `localhost`. |
| `Genilog_WebApi/render.yaml` | Adds `ResendApiKey` and `MailSettings__Mail` as required env keys. |

Debug logging in `ginilog-spa/lib/api.ts` and `Genilog_WebApi/EmailSender/EmailTemplates.cs` is still present. Remove it after you confirm sign-in works in production.

---

## Step 1 — Deploy the API to Render

1. Commit and push the updated `Genilog_WebApi` (especially `Program.cs` and `render.yaml`).
2. In the [Render dashboard](https://dashboard.render.com), open your **web service** for the API (e.g. `ginilog-api` / `ginilog-web`).
3. Trigger a **manual deploy** or wait for auto-deploy from your branch.
4. Confirm health: open `https://api-data-connection.ginilog.org/api/Health` — expect `{"status":"healthy",...}`.

### Required environment variables (Render → API service → Environment)

Set any that are missing (values are secrets — do not commit them):

| Variable | Purpose |
|----------|---------|
| `ConnectionStrings__Genilog_Data_Context` | PostgreSQL connection string |
| `Jwt__Key` | JWT signing key |
| `Jwt__Issuer` | JWT issuer |
| `Jwt__Audience` | JWT audience |
| `ResendApiKey` | Sends verification / password emails |
| `MailSettings__Mail` | From address for Resend (e.g. `noreply@yourdomain.com`) |
| `MailSettings__Password` | If used by your mail setup |
| `Payment__PaystackSK` | Payments (if used) |
| `Payment__Monnify` | Payments (if used) |
| `Twilio__AuthToken` | SMS (if used) |

`ASPNETCORE_ENVIRONMENT` and `ASPNETCORE_URLS` are already set in `render.yaml`.

---

## Step 2 — Deploy the frontend

### Option A — Vercel (recommended for Next.js)

1. In Vercel project **Settings → Environment Variables**, set:
   ```
   NEXT_PUBLIC_API_URL=https://api-data-connection.ginilog.org
   NEXT_PUBLIC_WS_URL=wss://api-data-connection.ginilog.org/ws
   ```
2. Redeploy (production build must pick up these vars).
3. Confirm the site URL is allowed by CORS (any `*.vercel.app` origin is allowed after API redeploy).

### Option B — Render (Static Site)

1. Set the same `NEXT_PUBLIC_*` variables in the static site’s environment **before build**.
2. Rebuild and publish.
3. After API redeploy, CORS allows `*.onrender.com` origins.

### Option C — Local development

1. Copy `ginilog-spa/.env.example` to `ginilog-spa/.env.local`.
2. Adjust URLs if needed.
3. Run:
   ```bash
   cd ginilog-spa
   npm install
   npm run dev
   ```
4. Localhost uses `/api` rewrites to the URL in `NEXT_PUBLIC_API_URL` (defaults to Render if unset in `next.config.js`).

---

## Step 3 — Verify sign-in

### Customer

1. Open `/customer-portal/login`.
2. Sign in with a **verified** customer account.
3. In DevTools → **Network**, confirm:
   - Request URL: `https://api-data-connection.ginilog.org/api/AuthUsers/login` (or your `NEXT_PUBLIC_API_URL`).
   - Status: **200** (not CORS error, not 404 on `/api/...` from the static host alone).

### Company (manager)

1. Open `/brand-owner/login`.
2. Sign in with a manager account.
3. Confirm request to `https://api-data-connection.ginilog.org/api/Admin/login-manager` returns **200**.

### CORS check (optional, after API redeploy)

Replace `YOUR-FRONTEND` with your real host:

```bash
curl.exe -s -X OPTIONS "https://api-data-connection.ginilog.org/api/AuthUsers/login" ^
  -H "Origin: https://YOUR-FRONTEND.onrender.com" ^
  -H "Access-Control-Request-Method: POST" -D -
```

Expect header: `access-control-allow-origin: https://YOUR-FRONTEND.onrender.com`

---

## Step 4 — Email verification (if users cannot log in)

If login returns **“User Email Not Yet Verify”**:

1. Ensure `ResendApiKey` and `MailSettings__Mail` are set on Render.
2. Use **Resend verification** on the login page or call the API token endpoint.
3. Check API logs on Render for `Warning: Email send failed`.

---

## Step 5 — Clean up (after everything works)

1. Remove debug instrumentation:
   - `ginilog-spa/lib/api.ts` — `#region agent log` blocks
   - `Genilog_WebApi/EmailSender/EmailTemplates.cs` — debug file append block
2. Commit and redeploy API + frontend.

---

## Legacy MVC apps (if you still use them)

These still point at `https://api-data.ginilog.com/api/` in `GlobalConstant.cs`:

- `Customer_Web_App`
- `Ginilog_Company_WebDasboard`
- `Ginilog_AdminWeb`

Update `BaseUrl` or move to configuration if those apps should use Render instead.

---

## Troubleshooting

| Symptom | Likely cause | Action |
|---------|----------------|--------|
| CORS error in browser | API not redeployed with new `Program.cs` | Redeploy API |
| 404 on `/api/...` from frontend host only | Static site without Next server | Set `NEXT_PUBLIC_API_URL`; use direct API URL in client |
| First request very slow | Render free tier cold start | Wait ~30s and retry, or upgrade plan |
| 401 / “User Does not Exist” | Wrong credentials or DB not connected | Check DB connection string and user records |
| “Email Not Yet Verify” + no email | Missing Resend env | Set `ResendApiKey` and `MailSettings__Mail` on Render |

---

## Reference

- Deploy guide: `.windsurf/workflows/deploy-to-render.md`
- API docs: `Genilog_WebApi/API_DOCUMENTATION.md`
- Backend overview: `BACKEND_DOCS.md`
