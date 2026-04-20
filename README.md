# api-unit-test-samples

A practical reference for **unit testing ASP.NET Core Web API controllers** using **MSTest** and **Moq**.

Covers real patterns used in production .NET projects — mocking services, verifying calls, testing all HTTP response types, and checking soft-delete behaviour.

---

## What It Does

Demonstrates how to write clean, maintainable unit tests for a .NET API controller without hitting a real database. Uses Moq to mock the service layer so tests run fast and in isolation.

---

## How It Works

```
Test Method
    │
    ▼
Mock<IAccessCardService>     ← fake service, no DB needed
    │
    ▼
AccessCardsController        ← the real controller under test
    │
    ▼
Assert HTTP response         ← check status code, body, behaviour
```

---

## Test Cases Covered

| Test | What It Verifies |
|------|-----------------|
| `GetAll_ReturnsOk_WithListOfCards` | Returns 200 with correct list |
| `GetAll_ReturnsOk_WithEmptyList` | Handles empty results gracefully |
| `GetById_ReturnsOk_WhenCardExists` | Returns 200 with correct card |
| `GetById_ReturnsNotFound_WhenCardDoesNotExist` | Returns 404 for missing ID |
| `Create_ReturnsCreated_WithNewCard` | Returns 201 with created object |
| `Create_ReturnsBadRequest_WhenModelStateIsInvalid` | Catches validation errors |
| `Update_ReturnsNoContent_WhenCardExists` | Returns 204 on success |
| `Update_ReturnsNotFound_WhenCardDoesNotExist` | Returns 404 for missing ID |
| `Deactivate_ReturnsNoContent_WhenCardExists` | Returns 204 on soft delete |
| `Deactivate_ReturnsNotFound_WhenCardDoesNotExist` | Returns 404 for missing ID |
| `Create_CallsServiceExactlyOnce` | Verifies no duplicate service calls |
| `Deactivate_NeverCallsDelete_UsesSoftDelete` | Confirms soft delete pattern |

---

## Key Patterns Demonstrated

**1. Arrange / Act / Assert structure**
Every test follows the AAA pattern for readability.

**2. Moq setup and verification**
```csharp
// Setup — tell the mock what to return
_mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(card);

// Verify — confirm the method was called exactly once
_mockService.Verify(s => s.CreateAsync(It.IsAny<AccessCard>()), Times.Once);
```

**3. Testing all HTTP response types**
```csharp
var okResult = result as OkObjectResult;          // 200
var createdResult = result as CreatedAtActionResult; // 201
var badRequest = result as BadRequestObjectResult;   // 400
var notFound = result as NotFoundObjectResult;       // 404
Assert.IsInstanceOfType(result, typeof(NoContentResult)); // 204
```

**4. VerifyNoOtherCalls — catch unexpected behaviour**
```csharp
_mockService.Verify(s => s.DeactivateAsync(1), Times.Once);
_mockService.VerifyNoOtherCalls(); // fails if any other method was called
```

---

## Tech Stack

- **MSTest** — Microsoft's built-in .NET test framework
- **Moq** — most widely used .NET mocking library
- **ASP.NET Core** — controller under test

---

## How to Run Tests

```bash
# 1. Clone the repo
git clone https://github.com/rajugupta-dev/api-unit-test-samples

# 2. Run all tests
dotnet test

# 3. Run with coverage (requires Fine Code Coverage or coverlet)
dotnet test --collect:"XPlat Code Coverage"
```

---

## Related Repos

- [dotnet-access-control-api](https://github.com/rajugupta-dev/dotnet-access-control-api) — The API being tested here
- [azure-notification-function](https://github.com/rajugupta-dev/azure-notification-function) — Azure Function companion project
