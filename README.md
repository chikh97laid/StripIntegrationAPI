# StripIntegrationAPI

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Postgres-PostgreSQL-green)](https://www.postgresql.org/)
[![Stripe](https://img.shields.io/badge/Stripe-Payments-orange)](https://stripe.com/)

**StripIntegrationAPI** — a compact ASP.NET Core 8 Web API integrating Stripe Checkout with order & product management using PostgreSQL + EF Core.  
Includes a simple static checkout page for quick payment testing.

---

## Live Demo
https://stripintegrationapi.onrender.com/checkout.html

---

## Key Features
- Stripe Checkout session creation + webhook handling  
- Orders, OrderItems, Product entities with PostgreSQL persistence  
- Demo frontend in `wwwroot/checkout.html`  
- Docker-ready build and run workflow  

---

## Tech Stack
- **Backend:** ASP.NET Core 8 (C#)  
- **Database:** PostgreSQL + EF Core (Npgsql)  
- **Payments:** Stripe (Stripe.net)  
- **Frontend:** HTML/CSS/JS  
- **Containerization:** Docker  

---

## Quick Start (Local)

### 1. Clone the repository
```bash
git clone https://github.com/chikh97laid/StripIntegrationAPI.git
cd StripIntegrationAPI
```

### 2. Add environment variables
Create a `.env` file (or set system env variables):

```env
ConnectionStrings__DefaultConnection="Host=HOST;Database=DB;Username=USER;Password=PASS;SSL Mode=Require;Trust Server Certificate=true"
Stripe__SecretKey="sk_test_..."
Stripe__WebhookSecret="whsec_..."
```

### 3. Apply migrations
```bash
dotnet ef database update
```

### 4. Run the API
```bash
dotnet run
```

Open the demo page:  
```
http://localhost:5000/checkout.html
```

> ⚠️ Make sure the `Migrations/` folder is committed so deployments can apply the schema.

---

## Docker (Build & Run)

### 1. Build the image
```bash
docker build -t stripintegrationapi .
```

### 2. Run the container
```bash
docker run \
  -e ConnectionStrings__DefaultConnection="Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true" \
  -e Stripe__SecretKey="sk_test_..." \
  -p 8080:80 stripintegrationapi
```

Then open:
```
http://localhost:8080/checkout.html
```

---

## Environment Variables Summary

| Variable | Description |
|---------|-------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Stripe__SecretKey` | Stripe secret key |
| `Stripe__WebhookSecret` | Stripe webhook signing secret |

**PostgreSQL example:**
```env
Host=your-host.render.com;Database=stripintegrationdb;Username=stripuser;Password=secret;SSL Mode=Require;Trust Server Certificate=true
```

---

## Links
- **GitHub:** https://github.com/chikh97laid  
- **LinkedIn:** https://linkedin.com/in/chikhouladlaid

