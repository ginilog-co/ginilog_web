---
description: Deploy Ginilog to Render (API + Frontend)
---

# Deploy to Render (Free Tier)

## Overview
This workflow migrates from SmarterASP.NET to Render's free tier hosting.
Render offers 750 hours/month free for web services (spins down after 15min idle).

## Architecture on Render
- **API Service**: .NET 8 Web API on Render
- **Static Site**: Next.js frontend on Render Static Sites
- **Custom Domain**: Connect your domain to both

---

## Step 1: Prepare API for Render

### 1.1 Create render.yaml in `Genilog_WebApi/`

```yaml
services:
  - type: web
    name: ginilog-api
    runtime: dotnet
    buildCommand: dotnet restore Genilog_WebApi.csproj && dotnet build Genilog_WebApi.csproj -c Release
    startCommand: dotnet Genilog_WebApi/bin/Release/net8.0/Genilog_WebApi.dll
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://0.0.0.0:10000
    healthCheckPath: /api/Health
```

### 1.2 Add CORS for Render domain

Update `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Render", policy =>
    {
        policy.WithOrigins(
                "https://ginilog.onrender.com",  // Your static site
                "https://*.onrender.com"         // Any Render subdomain
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// In app pipeline (before MapControllers):
app.UseCors("Render");
```

### 1.3 Add Health Check Endpoint

Create `Controllers/HealthController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;

namespace Genilog_WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}
```

### 1.4 Commit and Push to GitHub

```bash
cd Genilog_WebApi
git init
git add .
git commit -m "Prepare for Render deployment"
# Push to GitHub repository
```

---

## Step 2: Deploy API to Render

### 2.1 Create New Web Service
1. Go to https://dashboard.render.com
2. Click **New** > **Web Service**
3. Connect your GitHub repo
4. Select `.NET` runtime
5. Configure:
   - **Name**: `ginilog-api`
   - **Build Command**: `dotnet restore && dotnet build -c Release`
   - **Start Command**: `dotnet bin/Release/net8.0/Genilog_WebApi.dll`
   - **Plan**: Free

6. Add Environment Variables:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ASPNETCORE_URLS` = `http://0.0.0.0:10000`

7. Click **Create Web Service**

### 2.2 Note the API URL
After deployment, you'll get a URL like:
`https://ginilog-api.onrender.com`

---

## Step 3: Deploy Frontend to Render Static Sites

### 3.1 Update Frontend API URL

Update `ginilog-spa/.env.local`:
```
NEXT_PUBLIC_API_URL=https://ginilog-api.onrender.com
```

### 3.2 Update next.config.js

```javascript
/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  output: 'export',
  distDir: 'dist',
  images: {
    unoptimized: true,
  },
  trailingSlash: true,
};

module.exports = nextConfig;
```

### 3.3 Create render.yaml in `ginilog-spa/`

```yaml
services:
  - type: static
    name: ginilog-frontend
    buildCommand: npm install && npm run build
    staticPublishPath: ./dist
    routes:
      - type: rewrite
        source: /**
        destination: /index.html
```

### 3.4 Commit and Push

```bash
cd ginilog-spa
git add .
git commit -m "Configure for Render deployment"
git push
```

### 3.5 Create Static Site on Render
1. In Render dashboard, click **New** > **Static Site**
2. Connect your repo
3. Configure:
   - **Name**: `ginilog-frontend`
   - **Build Command**: `npm install && npm run build`
   - **Publish Directory**: `dist`
   - **Plan**: Free

4. Click **Create Static Site**

---

## Step 4: Configure Custom Domain (Optional)

### 4.1 Add Domain to API
1. In Render dashboard, go to `ginilog-api` service
2. Click **Settings** > **Custom Domains**
3. Add your domain: `api.yourdomain.com`
4. Follow DNS instructions (CNAME record)

### 4.2 Add Domain to Frontend
1. Go to `ginilog-frontend` static site
2. Click **Settings** > **Custom Domains**
3. Add your domain: `www.yourdomain.com`
4. Follow DNS instructions

---

## Step 5: Update API References

### 5.1 Frontend Environment
Update `ginilog-spa/.env.local`:
```
NEXT_PUBLIC_API_URL=https://api.yourdomain.com
# or
NEXT_PUBLIC_API_URL=https://ginilog-api.onrender.com
```

### 5.2 Rebuild and Redeploy
```bash
cd ginilog-spa
npm run build
git add .
git commit -m "Update API URL to Render"
git push
# Render will auto-deploy
```

---

## Free Tier Limits

| Service | Free Tier |
|---------|-----------|
| Web Service (API) | 750 hrs/month, sleeps after 15min idle |
| Static Site | Unlimited, always on |
| Bandwidth | 100 GB/month |
| Build minutes | 500 min/month |

**Note**: API "sleeps" after 15 min of no traffic (takes ~30s to wake up)

---

## Troubleshooting

### API returns 502/503
- Check logs in Render dashboard
- Verify `ASPNETCORE_URLS` is set to `http://0.0.0.0:10000`
- Ensure Health endpoint responds at `/api/Health`

### CORS errors
- Verify CORS policy includes your frontend domain
- Check `AllowCredentials()` is set for JWT cookies/tokens
- Test with browser console open

### Slow first request
- Normal for free tier (API sleeps after 15min)
- Consider upgrading to paid ($7/month) to prevent sleeping
- Or use a ping service to keep API warm

### Build fails
- Check build logs in Render dashboard
- Verify `package.json` has correct build script
- Ensure Node.js version is compatible (use `engines` field)

---

## Cost Comparison

| Service | SmarterASP.NET | Render |
|---------|----------------|--------|
| Monthly Cost | ~$10-20 | **$0** |
| Performance | Shared Windows | Container-based |
| SSL | Manual | Auto |
| CDN | No | Yes (Static sites) |
| Sleep/Idle | Never | After 15min (web services only) |

---

## Post-Migration Checklist

- [ ] API deployed and responding
- [ ] Frontend deployed and loading
- [ ] Login works
- [ ] API calls succeed (check browser network tab)
- [ ] JWT tokens working
- [ ] Images loading
- [ ] Custom domain configured (optional)
- [ ] Old SmarterASP.NET hosting cancelled

## Support

Render documentation: https://render.com/docs
.NET on Render: https://render.com/docs/dotnet
