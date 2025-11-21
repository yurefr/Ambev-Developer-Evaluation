🍺 Ambev Developer Evaluation - Sales API

Robust RESTful API developed as part of the technical assessment for the **DeveloperStore** team. This project manages the complete sales lifecycle, implementing high-level software architecture patterns, complex business rules, and an aggressive automated testing strategy.

![Net Version](https://img.shields.io/badge/.NET-8.0-purple) ![Status](https://img.shields.io/badge/Status-Complete-green) ![Coverage](https://img.shields.io/badge/Test%20Coverage-High-blue)

## 🚀 Overview

This application is not just a CRUD; it is a demonstration of **Clean Architecture** and **DDD (Domain-Driven Design)**. It manages sales, items, customers, and branches, applying progressive discount rules, quantity limits, and auditing via domain events.

### 🌟 Implementation Highlights
- **Architecture:** Clean Architecture separating responsibilities into Layers (Domain, Application, Infra, WebApi).
- **Design Patterns:** CQRS (Command Query Responsibility Segregation), Mediator, Repository, Unit of Work, and Event-Driven.
- **Idempotency:** The update method (`PUT`) ensures total consistency by synchronizing the state of the sent items with the database.
- **Observability:** Structured logging with Serilog and implemented Health Checks.

---

## 🛠️ Tech Stack

| Category | Technology |
| :--- | :--- |
| **Language** | .NET 8 (C#) |
| **Database** | PostgreSQL |
| **ORM** | Entity Framework Core (Code First) |
| **Communication** | MediatR (In-Process) |
| **Events** | Rebus (In-Memory Bus) |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **Testing** | xUnit, NSubstitute, Bogus, FluentAssertions |
| **Coverage** | Coverlet & ReportGenerator |
| **Container** | Docker & Docker Compose |

---

## 📂 Project Structure

The folder structure rigorously follows the requested requirements, ensuring separation between source code and tests:

```text
root/
├── src/                                      # Application Source Code
│   ├── Ambev.DeveloperEvaluation.WebApi      # Entry Point (API)
│   ├── Ambev.DeveloperEvaluation.Application # Use Cases (CQRS)
│   ├── Ambev.DeveloperEvaluation.Domain      # Entities and Business Rules
│   ├── Ambev.DeveloperEvaluation.ORM         # Data Infrastructure
│   ├── Ambev.DeveloperEvaluation.IoC         # Dependency Injection
│   └── Ambev.DeveloperEvaluation.Common      # Cross-Cutting Utilities
├── tests/                                    # Automated Tests
│   ├── Ambev.DeveloperEvaluation.Unit        # Unit Tests
│   ├── Ambev.DeveloperEvaluation.Integration # Integration Tests (Repos)
│   └── Ambev.DeveloperEvaluation.Functional  # E2E Tests (Controllers)
├── coverage-report.bat                       # Script to generate coverage report
└── README.md                                 # Documentation
```
# 🚀 How to Run

## Prerequisites
- Docker and Docker Compose installed.  
**OR**
- .NET 8 SDK and PostgreSQL running locally.

---

## Option 1: Via Docker (Recommended)

In the project root, run:

```bash
docker-compose up -d --build
```

The API will be available at:  
http://localhost:8080/swagger

---

## Option 2: Run Locally (.NET CLI)

Configure the `ConnectionString` in:

```
src/Ambev.DeveloperEvaluation.WebApi/appsettings.json
```

Apply migrations and start:

```bash
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM -s src/Ambev.DeveloperEvaluation.WebApi
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

---

# 🧪 Quality Assurance & Testing

The project was developed with a focus on high reliability.  
The test suite covers everything from domain logic to API endpoints.

---

## 1. Running Automated Tests

To run all tests (Unit, Integration, and Functional):

```bash
dotnet test
```

---

## 2. Coverage Report 📊

The project includes an automated script for code coverage analysis, aiming for 100% coverage on business rules and handlers.

To generate the visual report (on Windows), run in root:

```bash
.\coverage-report.bat
```

The script will:

- Run the tests  
- Collect metrics via Coverlet  
- Generate a static website  

The report is accessible at:

```
TestResults/CoverageReport/index.html
```

---

# 📮 Manual Testing Guide (Postman)

> **💡 Pro Tip:** For your convenience, the **Postman Collection** file (`Ambev_Developer_Evaluation.postman_collection.json`) is included in the **root directory** of this repository. Simply import it into Postman to access all pre-configured requests, environment variables, and example payloads immediately.

In addition to automated tests, the API was manually validated to ensure the robustness of business flows.

Before testing API calls:

1. Create a user
2. Log in
3. Use the JWT Token (Bearer Token) for authorization

---

## ✅ Happy Path Scenarios

| Method | Endpoint        | Description                                    | Expectation                           |
|--------|-----------------|------------------------------------------------|---------------------------------------|
| POST   | /api/Sales      | Create sale with < 4 items                    | Success (201), Discount = 0%          |
| POST   | /api/Sales      | Create sale with 5 identical items             | Success (201), Discount = 10%         |
| POST   | /api/Sales      | Create sale with 15 identical items            | Success (201), Discount = 20%         |
| GET    | /api/Sales/{id} | Get sale by ID                                 | Returns complete data with items      |
| GET    | /api/Sales      | List paged sales. Test `_page`, `_size` params | Returns paginated results             |

---

## ⚠️ Edge Cases & Business Rules

These tests ensure that the system handles complex logic correctly.

### Discount Upgrade (Via PUT)
- **Action:** Update sale from 3 to 5 identical items  
- **Result:** Discount should increase to **10%**, and total recalculated

### Substitutive Update (Full Sync)
- **Action:** PUT request sends only one new item  
- **Result:** Old items are removed and replaced by the new one

### Dynamic Sorting
- **Action:**  
```
GET /api/Sales?_order=totalAmount desc
```
- **Result:** Highest value sale should appear first

---

# 🚫 Validation & Error Scenarios

The API prevents invalid operations.

| Scenario | Example Payload | Expected Result |
|---------|-----------------|-----------------|
| Limit Exceeded | Quantity: 21 | 400 Bad Request – “Cannot sell more than 20 identical items” |
| Negative Values | Quantity: -1, Price: -10 | 400 Bad Request – FluentValidation blocks it |
| Empty Sale | `items: []` | 400 Bad Request – Must contain at least one item |
| Non-Existent IDs | `GET /api/Sales/{fake-guid}` | 404 – Resource not found |
| Cancelled Item Edit | Attempt to modify cancelled item | Operation rejected or item kept cancelled |

---

# ⚖️ Business Rules Implemented

## Progressive Discounts
- < 4 identical items → **0%**
- 4 to 9 identical items → **10%**
- 10 to 20 identical items → **20%**

## Restrictions
- Cannot sell **more than 20 identical items**
- Cancelled sales **cannot be modified**

---

Developed for **Ambev Developer Evaluation**.