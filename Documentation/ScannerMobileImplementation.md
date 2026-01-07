# Scanner Mobile App Implementation Guide

## Overview
This guide shows how to implement the refresh token system specifically for scanner devices in mobile apps. Scanners use their Serial Number and PIN for authentication instead of email/password.

## Scanner Authentication Flow

### 1. Scanner Login (PIN Confirmation)
```javascript
// Scanner login using Serial Number and PIN
const scannerLogin = async (serialNo, pin) => {
  const response = await fetch('/api/scanners/confirm-pin', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      serialNo: serialNo,
      scannerPIN: pin
    })
  });

  if (response.ok) {
    const data = await response.json();
    if (data.isValid) {
      // Save tokens securely
      await saveTokens(data.token, data.refreshToken);
      return data;
    }
  }
  throw new Error('Invalid scanner credentials');
};
```

### 2. Token Refresh for Scanners
```javascript
const refreshScannerToken = async () => {
  const refreshToken = await getRefreshToken();
  
  const response = await fetch('/api/scanners/refresh-token', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken })
  });

  if (response.ok) {
    const data = await response.json();
    await saveTokens(data.token, data.refreshToken);
    return data;
  }
  throw new Error('Token refresh failed');
};
```

## React Native Scanner App Implementation

### 1. Scanner Token Manager
```javascript
// services/ScannerTokenManager.js
import Keychain from '@react-native-keychain/react-native-keychain';
import AsyncStorage from '@react-native-async-storage/async-storage';

class ScannerTokenManager {
  constructor() {
    this.accessToken = null;
    this.refreshToken = null;
    this.scannerInfo = null;
  }

  async saveTokens(accessToken, refreshToken, scanner, tokenExpiry, refreshExpiry) {
    try {
      // Store tokens securely
      await Keychain.setInternetCredentials(
        'scanner_tokens',
        'access_token',
        accessToken
      );
      
      await Keychain.setInternetCredentials(
        'scanner_refresh_tokens',
        'refresh_token',
        refreshToken
      );

      // Store scanner info and expiration times
      await AsyncStorage.setItem('scanner_info', JSON.stringify(scanner));
      await AsyncStorage.setItem('token_expiry', tokenExpiry);
      await AsyncStorage.setItem('refresh_token_expiry', refreshExpiry);

      this.accessToken = accessToken;
      this.refreshToken = refreshToken;
      this.scannerInfo = scanner;
    } catch (error) {
      console.error('Error saving scanner tokens:', error);
      throw error;
    }
  }

  async loadTokens() {
    try {
      const accessCreds = await Keychain.getInternetCredentials('scanner_tokens');
      const refreshCreds = await Keychain.getInternetCredentials('scanner_refresh_tokens');
      
      if (accessCreds && refreshCreds) {
        this.accessToken = accessCreds.password;
        this.refreshToken = refreshCreds.password;
        
        const scannerInfo = await AsyncStorage.getItem('scanner_info');
        this.scannerInfo = scannerInfo ? JSON.parse(scannerInfo) : null;
        
        return true;
      }
      return false;
    } catch (error) {
      console.error('Error loading scanner tokens:', error);
      return false;
    }
  }

  async clearTokens() {
    try {
      await Keychain.resetInternetCredentials('scanner_tokens');
      await Keychain.resetInternetCredentials('scanner_refresh_tokens');
      await AsyncStorage.removeItem('scanner_info');
      await AsyncStorage.removeItem('token_expiry');
      await AsyncStorage.removeItem('refresh_token_expiry');
      
      this.accessToken = null;
      this.refreshToken = null;
      this.scannerInfo = null;
    } catch (error) {
      console.error('Error clearing scanner tokens:', error);
    }
  }

  getScannerInfo() {
    return this.scannerInfo;
  }
}

export default new ScannerTokenManager();
```

### 2. Scanner API Service
```javascript
// services/ScannerApiService.js
import ScannerTokenManager from './ScannerTokenManager';

const API_BASE_URL = 'https://your-api-domain.com/api';

class ScannerApiService {
  constructor() {
    this.isRefreshing = false;
    this.failedQueue = [];
  }

  async makeRequest(endpoint, options = {}) {
    const url = `${API_BASE_URL}${endpoint}`;
    const config = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
    };

    // Add auth header if we have a token
    if (ScannerTokenManager.accessToken) {
      config.headers.Authorization = `Bearer ${ScannerTokenManager.accessToken}`;
    }

    try {
      const response = await fetch(url, config);
      
      // Handle 401 - token expired
      if (response.status === 401 && ScannerTokenManager.refreshToken) {
        return await this.handleTokenExpired(url, config);
      }
      
      return response;
    } catch (error) {
      console.error('Scanner API request failed:', error);
      throw error;
    }
  }

  async handleTokenExpired(url, config) {
    if (this.isRefreshing) {
      return new Promise((resolve, reject) => {
        this.failedQueue.push({ resolve, reject, url, config });
      });
    }

    this.isRefreshing = true;

    try {
      await this.refreshToken();
      
      // Retry original request
      config.headers.Authorization = `Bearer ${ScannerTokenManager.accessToken}`;
      const response = await fetch(url, config);
      
      // Process queued requests
      this.processQueue(null);
      
      return response;
    } catch (error) {
      this.processQueue(error);
      await ScannerTokenManager.clearTokens();
      throw error;
    } finally {
      this.isRefreshing = false;
    }
  }

  processQueue(error) {
    this.failedQueue.forEach(({ resolve, reject, url, config }) => {
      if (error) {
        reject(error);
      } else {
        config.headers.Authorization = `Bearer ${ScannerTokenManager.accessToken}`;
        resolve(fetch(url, config));
      }
    });
    
    this.failedQueue = [];
  }

  async refreshToken() {
    if (!ScannerTokenManager.refreshToken) {
      throw new Error('No refresh token available');
    }

    const response = await fetch(`${API_BASE_URL}/scanners/refresh-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: ScannerTokenManager.refreshToken }),
    });

    if (!response.ok) {
      throw new Error('Scanner token refresh failed');
    }

    const data = await response.json();
    await ScannerTokenManager.saveTokens(
      data.token,
      data.refreshToken,
      data.scanner,
      data.tokenExpiry,
      data.refreshTokenExpiry
    );

    return data;
  }

  // Scanner authentication
  async confirmPin(serialNo, pin) {
    const response = await fetch(`${API_BASE_URL}/scanners/confirm-pin`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        serialNo: serialNo,
        scannerPIN: pin
      }),
    });

    if (!response.ok) {
      throw new Error('PIN confirmation failed');
    }

    const data = await response.json();
    
    if (!data.isValid) {
      throw new Error(data.message || 'Invalid PIN');
    }

    await ScannerTokenManager.saveTokens(
      data.token,
      data.refreshToken,
      data.scanner,
      data.tokenExpiry,
      data.refreshTokenExpiry
    );

    return data;
  }

  async logout() {
    if (ScannerTokenManager.refreshToken) {
      try {
        await this.makeRequest('/scanners/revoke-token', {
          method: 'POST',
          body: JSON.stringify({ refreshToken: ScannerTokenManager.refreshToken }),
        });
      } catch (error) {
        console.error('Scanner logout API call failed:', error);
      }
    }
    
    await ScannerTokenManager.clearTokens();
  }

  // Scanner-specific API calls
  async getScannerInfo() {
    const response = await this.makeRequest('/scanners/my-info');
    if (!response.ok) {
      throw new Error('Failed to get scanner info');
    }
    return await response.json();
  }

  async testToken() {
    const response = await this.makeRequest('/scanners/test-token');
    if (!response.ok) {
      throw new Error('Token test failed');
    }
    return await response.json();
  }
}

export default new ScannerApiService();
```

### 3. Scanner Login Screen
```javascript
// screens/ScannerLoginScreen.js
import React, { useState, useEffect } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert } from 'react-native';
import ScannerApiService from '../services/ScannerApiService';
import ScannerTokenManager from '../services/ScannerTokenManager';

const ScannerLoginScreen = ({ navigation }) => {
  const [serialNo, setSerialNo] = useState('');
  const [pin, setPin] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    checkExistingTokens();
  }, []);

  const checkExistingTokens = async () => {
    const hasTokens = await ScannerTokenManager.loadTokens();
    if (hasTokens) {
      try {
        // Test if tokens are still valid
        await ScannerApiService.testToken();
        navigation.replace('ScannerHome');
      } catch {
        // Tokens invalid, stay on login
        await ScannerTokenManager.clearTokens();
      }
    }
  };

  const handleLogin = async () => {
    if (!serialNo || !pin) {
      Alert.alert('Error', 'Please enter Serial Number and PIN');
      return;
    }

    setLoading(true);
    try {
      const result = await ScannerApiService.confirmPin(serialNo, pin);
      Alert.alert('Success', `Welcome ${result.scanner.scannerName}!`);
      navigation.replace('ScannerHome');
    } catch (error) {
      Alert.alert('Login Failed', error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={{ flex: 1, padding: 20, justifyContent: 'center' }}>
      <Text style={{ fontSize: 24, marginBottom: 30, textAlign: 'center' }}>
        Scanner Login
      </Text>
      
      <TextInput
        placeholder="Serial Number"
        value={serialNo}
        onChangeText={setSerialNo}
        autoCapitalize="characters"
        style={{
          borderWidth: 1,
          borderColor: '#ddd',
          padding: 15,
          marginBottom: 15,
          borderRadius: 8,
        }}
      />
      
      <TextInput
        placeholder="PIN"
        value={pin}
        onChangeText={setPin}
        secureTextEntry
        keyboardType="numeric"
        maxLength={6}
        style={{
          borderWidth: 1,
          borderColor: '#ddd',
          padding: 15,
          marginBottom: 20,
          borderRadius: 8,
        }}
      />
      
      <TouchableOpacity
        onPress={handleLogin}
        disabled={loading}
        style={{
          backgroundColor: loading ? '#ccc' : '#007AFF',
          padding: 15,
          borderRadius: 8,
          alignItems: 'center',
        }}
      >
        <Text style={{ color: 'white', fontSize: 16 }}>
          {loading ? 'Logging in...' : 'Login'}
        </Text>
      </TouchableOpacity>
    </View>
  );
};

export default ScannerLoginScreen;
```

### 4. Scanner Home Screen
```javascript
// screens/ScannerHomeScreen.js
import React, { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, Alert } from 'react-native';
import ScannerApiService from '../services/ScannerApiService';
import ScannerTokenManager from '../services/ScannerTokenManager';

const ScannerHomeScreen = ({ navigation }) => {
  const [scannerInfo, setScannerInfo] = useState(null);

  useEffect(() => {
    loadScannerInfo();
  }, []);

  const loadScannerInfo = async () => {
    try {
      const info = await ScannerApiService.getScannerInfo();
      setScannerInfo(info);
    } catch (error) {
      Alert.alert('Error', 'Failed to load scanner information');
    }
  };

  const handleLogout = async () => {
    Alert.alert(
      'Logout',
      'Are you sure you want to logout?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Logout',
          style: 'destructive',
          onPress: async () => {
            try {
              await ScannerApiService.logout();
              navigation.replace('ScannerLogin');
            } catch (error) {
              console.error('Logout error:', error);
              // Force logout even if API call fails
              await ScannerTokenManager.clearTokens();
              navigation.replace('ScannerLogin');
            }
          }
        }
      ]
    );
  };

  return (
    <View style={{ flex: 1, padding: 20 }}>
      <Text style={{ fontSize: 24, marginBottom: 20 }}>Scanner Dashboard</Text>
      
      {scannerInfo && (
        <View style={{ marginBottom: 30 }}>
          <Text style={{ fontSize: 18, marginBottom: 10 }}>
            Scanner: {scannerInfo.scannerName}
          </Text>
          <Text style={{ fontSize: 16, marginBottom: 5 }}>
            Serial: {scannerInfo.serialNo}
          </Text>
          <Text style={{ fontSize: 16, marginBottom: 5 }}>
            Location: {scannerInfo.locationName}
          </Text>
          <Text style={{ fontSize: 16, color: scannerInfo.isActive ? 'green' : 'red' }}>
            Status: {scannerInfo.isActive ? 'Active' : 'Inactive'}
          </Text>
        </View>
      )}
      
      <TouchableOpacity
        onPress={() => {/* Navigate to scanning functionality */}}
        style={{
          backgroundColor: '#28a745',
          padding: 15,
          borderRadius: 8,
          alignItems: 'center',
          marginBottom: 10,
        }}
      >
        <Text style={{ color: 'white', fontSize: 16 }}>Start Scanning</Text>
      </TouchableOpacity>
      
      <TouchableOpacity
        onPress={handleLogout}
        style={{
          backgroundColor: '#dc3545',
          padding: 15,
          borderRadius: 8,
          alignItems: 'center',
        }}
      >
        <Text style={{ color: 'white', fontSize: 16 }}>Logout</Text>
      </TouchableOpacity>
    </View>
  );
};

export default ScannerHomeScreen;
```

## API Endpoints for Scanners

### Scanner Authentication
- `POST /api/scanners/confirm-pin` - Login with Serial Number and PIN
- `POST /api/scanners/refresh-token` - Refresh access token
- `POST /api/scanners/revoke-token` - Logout/revoke tokens

### Scanner Information
- `GET /api/scanners/my-info` - Get scanner details
- `GET /api/scanners/test-token` - Test token validity

## Key Differences from User Authentication

1. **No Email/Password**: Scanners use Serial Number + PIN
2. **Device-Specific**: Each scanner has unique credentials
3. **Location-Bound**: Scanners are tied to specific locations
4. **Role-Based**: Scanner tokens have "Scanner" role with limited permissions

## Security Features

1. **Secure Token Storage**: Tokens stored in device keychain
2. **Automatic Refresh**: Seamless token renewal
3. **Device Binding**: Tokens tied to specific scanner serial numbers
4. **Location Restrictions**: API access limited to scanner's location
5. **Token Revocation**: Ability to remotely disable scanner access

This implementation ensures scanners stay "logged in" indefinitely while maintaining security through refresh tokens and location-based restrictions.