# Testing the Refresh Token System

## Test Flow Example

### 1. Login and Get Tokens
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}
```

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "ABC123XYZ789...",
  "expiration": "2024-01-04T12:00:00Z",
  "refreshTokenExpiration": "2024-02-03T12:00:00Z",
  "user": {
    "id": 1,
    "name": "Test User",
    "email": "test@example.com",
    // ... other user properties
  }
}
```

### 2. Use Access Token for API Calls
```bash
GET /api/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. When Access Token Expires (after 24 hours)
```bash
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "ABC123XYZ789..."
}
```

**Expected Response:** New tokens with extended expiration

### 4. Generate PIN for Sensitive Operations
```bash
POST /api/auth/generate-pin
Content-Type: application/json

{
  "refreshToken": "NEW_REFRESH_TOKEN_FROM_STEP_3"
}
```

**Expected Response:**
```json
{
  "message": "PIN generated successfully",
  "pin": "123456"
}
```

### 5. Confirm PIN to Get New Access Token
```bash
POST /api/auth/confirm-pin
Content-Type: application/json

{
  "pin": "123456",
  "refreshToken": "NEW_REFRESH_TOKEN_FROM_STEP_3"
}
```

**Expected Response:** New access token (refresh token remains the same)

### 6. Revoke Refresh Token (Logout)
```bash
POST /api/auth/revoke-token
Authorization: Bearer CURRENT_ACCESS_TOKEN
Content-Type: application/json

{
  "refreshToken": "CURRENT_REFRESH_TOKEN"
}
```

**Expected Response:**
```json
{
  "message": "Token revoked successfully"
}
```

## Error Scenarios

### Invalid Refresh Token
```json
{
  "error": "Invalid or expired refresh token"
}
```

### Invalid PIN
```json
{
  "error": "Invalid PIN or refresh token"
}
```

### Expired PIN
```json
{
  "error": "Invalid PIN or refresh token"
}
```

## Client-Side Implementation Example

```javascript
class AuthManager {
  constructor() {
    this.accessToken = localStorage.getItem('accessToken');
    this.refreshToken = localStorage.getItem('refreshToken');
  }

  async login(email, password) {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });
    
    if (response.ok) {
      const data = await response.json();
      this.setTokens(data.token, data.refreshToken);
      return data;
    }
    throw new Error('Login failed');
  }

  async refreshAccessToken() {
    if (!this.refreshToken) throw new Error('No refresh token');
    
    const response = await fetch('/api/auth/refresh-token', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: this.refreshToken })
    });
    
    if (response.ok) {
      const data = await response.json();
      this.setTokens(data.token, data.refreshToken);
      return data;
    }
    throw new Error('Token refresh failed');
  }

  async generatePin() {
    const response = await fetch('/api/auth/generate-pin', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: this.refreshToken })
    });
    
    if (response.ok) {
      return await response.json();
    }
    throw new Error('PIN generation failed');
  }

  async confirmPin(pin) {
    const response = await fetch('/api/auth/confirm-pin', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ pin, refreshToken: this.refreshToken })
    });
    
    if (response.ok) {
      const data = await response.json();
      this.setTokens(data.token, data.refreshToken);
      return data;
    }
    throw new Error('PIN confirmation failed');
  }

  async logout() {
    if (this.refreshToken) {
      await fetch('/api/auth/revoke-token', {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.accessToken}`
        },
        body: JSON.stringify({ refreshToken: this.refreshToken })
      });
    }
    this.clearTokens();
  }

  setTokens(accessToken, refreshToken) {
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
  }

  clearTokens() {
    this.accessToken = null;
    this.refreshToken = null;
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  // Automatic token refresh interceptor
  async apiCall(url, options = {}) {
    const makeRequest = async (token) => {
      return fetch(url, {
        ...options,
        headers: {
          ...options.headers,
          'Authorization': `Bearer ${token}`
        }
      });
    };

    let response = await makeRequest(this.accessToken);
    
    if (response.status === 401 && this.refreshToken) {
      try {
        await this.refreshAccessToken();
        response = await makeRequest(this.accessToken);
      } catch (error) {
        this.clearTokens();
        throw new Error('Session expired');
      }
    }
    
    return response;
  }
}

// Usage example
const auth = new AuthManager();

// Login
await auth.login('user@example.com', 'password123');

// Make API calls (automatically handles token refresh)
const response = await auth.apiCall('/api/users');

// Generate and confirm PIN for sensitive operations
const pinData = await auth.generatePin();
console.log('PIN:', pinData.pin); // In production, this would be sent via SMS
await auth.confirmPin(pinData.pin);

// Logout
await auth.logout();
```