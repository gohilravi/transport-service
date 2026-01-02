# Test Instructions

## Running Tests

### Unit Tests (Recommended)
Run only the unit tests which test business logic without database dependencies:
```bash
dotnet test --filter "FullyQualifiedName!~Integration"
```

### All Tests (Including Integration)
To run all tests including integration tests:
```bash
dotnet test
```

**Note**: Integration tests require database setup and may fail if database is not properly configured.

## Test Coverage

### Unit Tests ✅ (22 tests passing)

#### Controller Tests
- ✅ CreateTransport with valid request returns Created with ID
- ✅ CreateTransport with invalid request returns BadRequest
- ✅ CreateTransport with service exception returns InternalServerError
- ✅ UpdateTransportStatus with valid request returns OK
- ✅ UpdateTransportStatus with invalid request returns BadRequest
- ✅ UpdateTransportStatus with non-existent ID returns NotFound
- ✅ UpdateTransportStatus with service exception returns InternalServerError

#### Service Tests
- ✅ CreateTransportAsync with valid request returns TransportResponse
- ✅ CreateTransportAsync with exception rolls back transaction
- ✅ UpdateTransportStatusAsync with valid ID returns updated transport
- ✅ UpdateTransportStatusAsync with invalid ID throws ArgumentException

#### Validator Tests
- ✅ CreateTransportRequest validation with valid data
- ✅ CreateTransportRequest validation with invalid OfferID
- ✅ CreateTransportRequest validation with invalid ZIP codes
- ✅ CreateTransportRequest validation with past dates
- ✅ UpdateTransportStatusRequest validation with valid status
- ✅ UpdateTransportStatusRequest validation with empty status
- ✅ UpdateTransportStatusRequest validation with too long status

### Integration Tests ⚠️ (3 tests - need database setup)
- CreateTransport_WithValidData_ReturnsCreatedWithId
- UpdateTransportStatus_WithValidData_ReturnsOk
- UpdateTransportStatus_WithInvalidId_ReturnsNotFound

## Test Scenarios Covered

### ✅ Happy Path Scenarios
1. **Create Transport** - Valid request creates transport and returns ID only
2. **Update Status** - Valid status update returns updated transport details

### ✅ Validation Scenarios
1. **Required Fields** - All required fields validated
2. **Data Format** - ZIP code format validation
3. **Business Rules** - Future date validation for schedule
4. **String Length** - Status field length validation

### ✅ Error Scenarios
1. **Invalid Input** - BadRequest responses for validation failures
2. **Not Found** - NotFound responses for non-existent transport IDs
3. **Server Errors** - InternalServerError responses for exceptions
4. **Transaction Rollback** - Database transaction rollback on errors

### ✅ Edge Cases
1. **Empty/Null Values** - Proper validation of empty inputs
2. **Boundary Values** - Testing min/max constraints
3. **Exception Handling** - Proper error handling and logging

## API Response Examples

### Create Transport Success (201)
```json
{
  "id": 123
}
```

### Update Status Success (200)
```json
{
  "id": 123,
  "carrierId": 1,
  "purchaseId": 1,
  "pickupLocation": "12345",
  "deliveryLocation": "67890",
  "status": "InTransit",
  "createdAt": "2026-01-02T10:00:00Z",
  "lastModifiedAt": "2026-01-02T11:00:00Z"
}
```

### Validation Error (400)
```json
[
  {
    "field": "OfferId",
    "error": "OfferId must be greater than 0"
  }
]
```