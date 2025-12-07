# StripIntegrationAPI

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Postgres-PostgreSQL-green)](https://www.postgresql.org/)
[![Stripe](https://img.shields.io/badge/Stripe-Payments-orange)](https://stripe.com/)

**StripIntegrationAPI** — A compact ASP.NET Core 8 Web API demonstrating **Stripe Checkout integration**, robust order & product management, and **PostgreSQL** persistence. Includes a simple static checkout page for demonstration and testing.

---

## 🌐 Live Demo

[Checkout Page](https://stripintegrationapi.onrender.com/checkout.html)

---

## ✨ Key Features

* **Stripe Checkout** session creation and webhook handling.
* **Order, OrderItem, and Product** persistence using PostgreSQL via EF Core.
* **Seeded demo data** and a simple testing frontend (`checkout.html`).
* **RESTful API** for payments and core order management.

---

## 🛠 Tech Stack

* **Backend:** ASP.NET Core 8 (C#)
* **ORM:** Entity Framework Core (Npgsql/PostgreSQL)
* **Payments:** Stripe (`Stripe.net` library)
* **Frontend Demo:** Static HTML/CSS/JS (`wwwroot`)

---

## 💻 Local Development (Run Locally)

To set up and run the project on your local machine, follow these steps:

> ### **1️⃣ Clone the Repository**
> ```bash
> git clone [https://github.com/chikh97laid/StripIntegrationAPI.git](https://github.com/chikh97laid/StripIntegrationAPI.git)
> cd StripIntegrationAPI
> ```
>
> ### **2️⃣ Configure Stripe & Environment Variables**
> Create a Stripe account to get your API keys. Then, set your connection string and keys as system environment variables or in a `.env` file:
> ```bash
> # Example Environment Variables (Replace values with your details)
> export ConnectionStrings__DefaultConnection="Host=HOST;Database=DB;Username=USER;Password=PASS;SSL Mode=Require;Trust Server Certificate=true"
> export Stripe__SecretKey="sk_test_..."
> export Stripe__WebhookSecret="whsec_..."
> ```
>
> ### **3️⃣ Apply Database Migrations**
> ```bash
> dotnet ef database update
> ```
>
> ### **4️⃣ Run the Project**
> ```bash
> dotnet run
> ```
> **Access the Demo:** Open `http://localhost:5000/checkout.html` in your browser.

---

## 🚀 Deployment (Hosting)

When deploying to a platform like Render or Railway, ensure the following steps are completed:

> ### **Deployment Checklist**
> 1.  Set all necessary environment variables (PostgreSQL connection string and Stripe keys) on the host platform.
> 2.  Run database migrations on the host as part of the initial setup.
> 3.  **Crucial:** Update your Stripe webhook URL in the Stripe dashboard to point to your deployed domain.
>
> ### **Example Production Postgres Connection String**
> ```bash
> export ConnectionStrings__DefaultConnection="Host=your-host.render.com;Database=stripintegrationdb;Username=stripuser;Password=secret;SSL Mode=Require;Trust Server Certificate=true"
> ```

---

## 🔗 Useful Links

* **GitHub:** [https://github.com/chikh97laid](https://github.com/chikh97laid)
* **LinkedIn:** [https://linkedin.com/in/chikhouladlaid](https://linkedin.com/in/chikhouladlaid)

---

## 📝 Notes

* Always use **Stripe Test Keys** and test cards during development.
* Ensure migrations are applied successfully locally before deploying.
* The file `wwwroot/checkout.html` is a demo checkout page—update its internal `fetch` URLs if you host the API on a different domain or port.
