# Refresh Token System for "Never Logged Out" Mode

## Overview
This system implements a refresh token mechanism that allows users to stay logged in indefinitely while maintaining security through PIN confirmation when needed.

## How It Works

### 1. Login Process
- User logs in with email/password
- System generates both:
  - **Access Token** (JWT): Short-lived (24 hours)
  - **Refresh Token**: Long-lived (30 days)
- Both tokens are returned to the client

### 2. Token Refresh
- When access token expires, client can use refresh token to get new access token
- Refresh token is automatically renewed with each refresh (rolling refresh)
- No user interaction required

### 3. PIN Confirmation
- For sensitive operations, system can require PIN confirmation
- PIN is generated server-side and expires in 5 minutes
- PIN validation uses the refresh token to ensure user identity

## API Endpoints

### POST /api/auth/login
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiration": "2024-01-04T12:00:00Z",
  "refreshTokenExpiration": "2024-02-03T12:00:00Z",
  "user": { ... }
}
```

### POST /api/auth/refresh-token
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Response:** Same as login response with new tokens

### POST /api/auth/generate-pin
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Response:**
```json
{
  "message": "PIN generated successfully",
  "pin": "123456"
}
```

### POST /api/auth/confirm-pin
```json
{
  "pin": "123456",
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Response:** Same as login response

### POST /api/auth/revoke-token
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

## Client Implementation

### 1. Store Tokens Securely
```javascript
// Store tokens in secure storage (not localStorage for production)
localStorage.setItem('accessToken', response.token);
localStorage.setItem('refreshToken', response.refreshToken);
```

### 2. Automatic Token Refresh
```javascript
// Intercept API calls and refresh token if needed
axios.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const response = await axios.post('/api/auth/refresh-token', {
            refreshToken
          });
          localStorage.setItem('accessToken', response.data.token);
          localStorage.setItem('refreshToken', response.data.refreshToken);
          // Retry original request
          return axios(error.config);
        } catch (refreshError) {
          // Refresh failed, redirect to login
          window.location.href = '/login';
        }
      }
    }
    return Promise.reject(error);
  }
);
```

### 3. PIN Confirmation Flow
```javascript
// When PIN confirmation is needed
async function confirmPin() {
  const refreshToken = localStorage.getItem('refreshToken');
  
  // Generate PIN
  const pinResponse = await axios.post('/api/auth/generate-pin', {
    refreshToken
  });
  
  // Show PIN to user (in production, send via SMS/email)
  const pin = prompt('Enter PIN: ' + pinResponse.data.pin);
  
  // Confirm PIN
  const confirmResponse = await axios.post('/api/auth/confirm-pin', {
    pin,
    refreshToken
  });
  
  // Update tokens
  localStorage.setItem('accessToken', confirmResponse.data.token);
}
```

## Security Features

1. **Refresh Token Rotation**: New refresh token issued with each refresh
2. **Expiration**: Refresh tokens expire after 30 days
3. **Single Use PINs**: PINs expire after 5 minutes and are invalidated after use
4. **Token Revocation**: Ability to revoke refresh tokens
5. **Database Storage**: Refresh tokens stored securely in database

## Database Changes

Added to `Users` table:
- `RefreshToken` (nvarchar(500), nullable)
- `RefreshTokenExpiry` (datetime2, nullable)

## Configuration

No additional configuration required. The system uses:
- JWT settings from appsettings.json
- In-memory cache for PIN storage
- Existing database connection

## Production Considerations

1. **PIN Delivery**: Implement SMS/email delivery instead of returning PIN in response
2. **Token Storage**: Use secure storage mechanisms (not localStorage)
3. **Rate Limiting**: Add rate limiting for PIN generation
4. **Monitoring**: Log refresh token usage for security monitoring
5. **Cleanup**: Implement cleanup job for expired refresh tokens