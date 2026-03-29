---
description: Deploy Ginilog SPA to SmarterASP.NET
---

# Deploy Next.js Frontend to SmarterASP.NET

## Overview
Since SmarterASP.NET is Windows/IIS hosting, you need to deploy your Next.js app as **static HTML files** (not a Node.js server).

## Prerequisites
- Access to SmarterASP.NET control panel
- FTP client (FileZilla, WinSCP) or SmarterASP.NET File Manager
- Node.js installed locally

## Step 1: Build Static Files

```bash
cd ginilog-spa
npm run build
```

This creates a `dist/` folder with all static HTML/CSS/JS files.

## Step 2: Upload to SmarterASP.NET

### Option A: FTP Upload (Recommended)
1. Open FileZilla or any FTP client
2. Connect to your SmarterASP.NET FTP server:
   - Host: `ftp.yourdomain.com` (or IP from control panel)
   - Username: Your SmarterASP.NET username
   - Password: Your SmarterASP.NET password
   - Port: 21 (or 22 for SFTP)

3. Navigate to the **web root folder** (usually `wwwroot/` or `public_html/`)

4. **Delete old files** (optional but recommended)

5. **Upload contents of `dist/` folder** to the web root
   - Drag all files from `ginilog-spa/dist/` to FTP `wwwroot/`

### Option B: File Manager Upload
1. Login to SmarterASP.NET control panel
2. Go to **File Manager**
3. Navigate to `wwwroot/`
4. Delete old files
5. Upload `dist/` contents as ZIP, then extract

## Step 3: Configure API URL

Since this is static export, API calls go directly to your backend:

1. Create `.env.local` file in `ginilog-spa/`:
```
NEXT_PUBLIC_API_URL=https://api-data.ginilog.com
```

2. Rebuild and reupload if you change this

## Step 4: Set Default Document (IIS)

In SmarterASP.NET control panel:
1. Go to **IIS** > **Default Documents**
2. Ensure `index.html` is at the top of the list
3. Add `customer-portal.html` if needed for sub-routes

## Step 5: Test Routes

After upload, test these URLs:
- `https://yourdomain.com/` → Landing page
- `https://yourdomain.com/customer-portal/` → Customer portal
- `https://yourdomain.com/customer-portal/login/` → Login page

## Important Notes

⚠️ **API Calls**: The frontend makes direct API calls to `https://api-data.ginilog.com`. Ensure CORS is configured on the backend.

⚠️ **No Server-Side Features**: Static export means no API routes, no server components, no image optimization.

⚠️ **Client-Side Routing**: Navigation works via React Router (client-side). Direct URLs to sub-routes need `.html` extension or IIS rewrite rules.

## Troubleshooting

### 404 on Page Refresh
Add `web.config` to `dist/` before upload:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="Next.js Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

### Images Not Loading
- Ensure images are in `public/img/`
- Use relative paths: `/img/image.jpg` (not `../img/`)
- Rebuild and reupload after adding images

### API Errors
- Check browser console for CORS errors
- Verify `NEXT_PUBLIC_API_URL` is set correctly
- Test API directly: `https://api-data.ginilog.com/api/AuthUsers`

## Quick Re-Deploy

After making changes:
```bash
cd ginilog-spa
npm run build
# Upload dist/ contents via FTP
```

## Migration to Render (When Ready)

See `deploy-to-render.md` workflow for moving from SmarterASP.NET to Render when your hosting expires.
