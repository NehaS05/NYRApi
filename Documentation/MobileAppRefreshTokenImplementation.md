# Mobile App Refresh Token Implementation Guide

## Overview
This guide shows how to implement the refresh token system in mobile apps (React Native, Flutter, Native iOS/Android) for seamless "never logged out" experience.

## Key Mobile Considerations

### 1. Secure Token Storage
- **Never use AsyncStorage/SharedPreferences for tokens in production**
- Use secure storage solutions:
  - **React Native**: `@react-native-keychain/react-native-keychain`
  - **Flutter**: `flutter_secure_storage`
  - **iOS**: Keychain Services
  - **Android**: EncryptedSharedPreferences

### 2. Background Token Refresh
- Refresh tokens before they expire
- Handle app state changes (background/foreground)
- Implement retry mechanisms for network failures

### 3. Biometric Authentication
- Use biometrics instead of PIN for better UX
- Fallback to PIN if biometrics unavailable

## React Native Implementation

### 1. Install Dependencies
```bash
npm install @react-native-keychain/react-native-keychain
npm install @react-native-async-storage/async-storage
npm install react-native-biometrics
```

### 2. Token Manager Service
```javascript
// services/TokenManager.js
import Keychain from '@react-native-keychain/react-native-keychain';
import AsyncStorage from '@react-native-async-storage/async-storage';

class TokenManager {
  constructor() {
    this.accessToken = null;
    this.refreshToken = null;
    this.tokenExpiry = null;
    this.refreshTokenExpiry = null;
  }

  // Secure storage for tokens
  async saveTokens(accessToken, refreshToken, expiration, refreshExpiration) {
    try {
      // Store tokens securely
      await Keychain.setInternetCredentials(
        'app_tokens',
        'access_token',
        accessToken
      );
      
      await Keychain.setInternetCredentials(
        'app_refresh_tokens',
        'refresh_token',
        refreshToken
      );

      // Store expiration times in AsyncStorage (not sensitive)
      await AsyncStorage.setItem('token_expiry', expiration);
      await AsyncStorage.setItem('refresh_token_expiry', refreshExpiration);

      this.accessToken = accessToken;
      this.refreshToken = refreshToken;
      this.tokenExpiry = new Date(expiration);
      this.refreshTokenExpiry = new Date(refreshExpiration);
    } catch (error) {
      console.error('Error saving tokens:', error);
      throw error;
    }
  }

  async loadTokens() {
    try {
      const accessCreds = await Keychain.getInternetCredentials('app_tokens');
      const refreshCreds = await Keychain.getInternetCredentials('app_refresh_tokens');
      
      if (accessCreds && refreshCreds) {
        this.accessToken = accessCreds.password;
        this.refreshToken = refreshCreds.password;
        
        const expiry = await AsyncStorage.getItem('token_expiry');
        const refreshExpiry = await AsyncStorage.getItem('refresh_token_expiry');
        
        this.tokenExpiry = expiry ? new Date(expiry) : null;
        this.refreshTokenExpiry = refreshExpiry ? new Date(refreshExpiry) : null;
        
        return true;
      }
      return false;
    } catch (error) {
      console.error('Error loading tokens:', error);
      return false;
    }
  }

  async clearTokens() {
    try {
      await Keychain.resetInternetCredentials('app_tokens');
      await Keychain.resetInternetCredentials('app_refresh_tokens');
      await AsyncStorage.removeItem('token_expiry');
      await AsyncStorage.removeItem('refresh_token_expiry');
      
      this.accessToken = null;
      this.refreshToken = null;
      this.tokenExpiry = null;
      this.refreshTokenExpiry = null;
    } catch (error) {
      console.error('Error clearing tokens:', error);
    }
  }

  isAccessTokenExpired() {
    if (!this.tokenExpiry) return true;
    return new Date() >= this.tokenExpiry;
  }

  isRefreshTokenExpired() {
    if (!this.refreshTokenExpiry) return true;
    return new Date() >= this.refreshTokenExpiry;
  }

  shouldRefreshToken() {
    if (!this.tokenExpiry) return false;
    // Refresh if token expires in next 5 minutes
    const fiveMinutesFromNow = new Date(Date.now() + 5 * 60 * 1000);
    return fiveMinutesFromNow >= this.tokenExpiry;
  }
}

export default new TokenManager();
```

### 3. API Service with Auto-Refresh
```javascript
// services/ApiService.js
import TokenManager from './TokenManager';

const API_BASE_URL = 'https://your-api-domain.com/api';

class ApiService {
  constructor() {
    this.isRefreshing = false;
    this.failedQueue = [];
  }

  async makeRequest(endpoint, options = {}) {
    // Check if we need to refresh token
    if (TokenManager.shouldRefreshToken() && !this.isRefreshing) {
      await this.refreshToken();
    }

    const url = `${API_BASE_URL}${endpoint}`;
    const config = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
    };

    // Add auth header if we have a token
    if (TokenManager.accessToken) {
      config.headers.Authorization = `Bearer ${TokenManager.accessToken}`;
    }

    try {
      const response = await fetch(url, config);
      
      // Handle 401 - token expired
      if (response.status === 401 && TokenManager.refreshToken) {
        return await this.handleTokenExpired(url, config);
      }
      
      return response;
    } catch (error) {
      console.error('API request failed:', error);
      throw error;
    }
  }

  async handleTokenExpired(url, config) {
    if (this.isRefreshing) {
      // If already refreshing, queue this request
      return new Promise((resolve, reject) => {
        this.failedQueue.push({ resolve, reject, url, config });
      });
    }

    this.isRefreshing = true;

    try {
      await this.refreshToken();
      
      // Retry original request
      config.headers.Authorization = `Bearer ${TokenManager.accessToken}`;
      const response = await fetch(url, config);
      
      // Process queued requests
      this.processQueue(null);
      
      return response;
    } catch (error) {
      this.processQueue(error);
      await TokenManager.clearTokens();
      // Navigate to login screen
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
        config.headers.Authorization = `Bearer ${TokenManager.accessToken}`;
        resolve(fetch(url, config));
      }
    });
    
    this.failedQueue = [];
  }

  async refreshToken() {
    if (!TokenManager.refreshToken || TokenManager.isRefreshTokenExpired()) {
      throw new Error('No valid refresh token');
    }

    const response = await fetch(`${API_BASE_URL}/auth/refresh-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: TokenManager.refreshToken }),
    });

    if (!response.ok) {
      throw new Error('Token refresh failed');
    }

    const data = await response.json();
    await TokenManager.saveTokens(
      data.token,
      data.refreshToken,
      data.expiration,
      data.refreshTokenExpiration
    );

    return data;
  }

  // Auth methods
  async login(email, password) {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      throw new Error('Login failed');
    }

    const data = await response.json();
    await TokenManager.saveTokens(
      data.token,
      data.refreshToken,
      data.expiration,
      data.refreshTokenExpiration
    );

    return data;
  }

  async logout() {
    if (TokenManager.refreshToken) {
      try {
        await this.makeRequest('/auth/revoke-token', {
          method: 'POST',
          body: JSON.stringify({ refreshToken: TokenManager.refreshToken }),
        });
      } catch (error) {
        console.error('Logout API call failed:', error);
      }
    }
    
    await TokenManager.clearTokens();
  }

  async generatePin() {
    const response = await this.makeRequest('/auth/generate-pin', {
      method: 'POST',
      body: JSON.stringify({ refreshToken: TokenManager.refreshToken }),
    });

    if (!response.ok) {
      throw new Error('PIN generation failed');
    }

    return await response.json();
  }

  async confirmPin(pin) {
    const response = await this.makeRequest('/auth/confirm-pin', {
      method: 'POST',
      body: JSON.stringify({ 
        pin, 
        refreshToken: TokenManager.refreshToken 
      }),
    });

    if (!response.ok) {
      throw new Error('PIN confirmation failed');
    }

    const data = await response.json();
    await TokenManager.saveTokens(
      data.token,
      data.refreshToken,
      data.expiration,
      data.refreshTokenExpiration
    );

    return data;
  }
}

export default new ApiService();
```

### 4. Biometric Authentication Component
```javascript
// components/BiometricAuth.js
import React, { useState } from 'react';
import { Alert } from 'react-native';
import TouchID from 'react-native-touch-id';
import ApiService from '../services/ApiService';

const BiometricAuth = ({ onSuccess, onError }) => {
  const [isAuthenticating, setIsAuthenticating] = useState(false);

  const authenticateWithBiometrics = async () => {
    setIsAuthenticating(true);
    
    try {
      // Check if biometrics are available
      const biometryType = await TouchID.isSupported();
      
      if (biometryType) {
        // Authenticate with biometrics
        await TouchID.authenticate('Authenticate to access your account', {
          title: 'Authentication Required',
          subtitle: 'Use your biometric to authenticate',
          description: 'This app uses biometric authentication for security',
          fallbackLabel: 'Use PIN instead',
          cancelLabel: 'Cancel',
        });
        
        // Generate and confirm PIN automatically after biometric success
        const pinData = await ApiService.generatePin();
        await ApiService.confirmPin(pinData.pin);
        
        onSuccess();
      } else {
        // Fallback to PIN entry
        showPinEntry();
      }
    } catch (error) {
      console.error('Biometric authentication failed:', error);
      if (error.name === 'UserCancel' || error.name === 'UserFallback') {
        showPinEntry();
      } else {
        onError(error);
      }
    } finally {
      setIsAuthenticating(false);
    }
  };

  const showPinEntry = () => {
    Alert.prompt(
      'Enter PIN',
      'Please enter your 6-digit PIN',
      async (pin) => {
        try {
          await ApiService.confirmPin(pin);
          onSuccess();
        } catch (error) {
          onError(error);
        }
      },
      'secure-text',
      '',
      'number-pad'
    );
  };

  return { authenticateWithBiometrics, isAuthenticating };
};

export default BiometricAuth;
```

### 5. App State Management
```javascript
// hooks/useAppState.js
import { useEffect, useRef } from 'react';
import { AppState } from 'react-native';
import TokenManager from '../services/TokenManager';
import ApiService from '../services/ApiService';

export const useAppState = () => {
  const appState = useRef(AppState.currentState);

  useEffect(() => {
    const handleAppStateChange = async (nextAppState) => {
      if (
        appState.current.match(/inactive|background/) &&
        nextAppState === 'active'
      ) {
        // App came to foreground
        await handleAppForeground();
      }
      
      appState.current = nextAppState;
    };

    const subscription = AppState.addEventListener('change', handleAppStateChange);
    
    return () => subscription?.remove();
  }, []);

  const handleAppForeground = async () => {
    try {
      // Load tokens from secure storage
      await TokenManager.loadTokens();
      
      // Check if refresh token is expired
      if (TokenManager.isRefreshTokenExpired()) {
        // Navigate to login
        return;
      }
      
      // Refresh access token if needed
      if (TokenManager.shouldRefreshToken()) {
        await ApiService.refreshToken();
      }
    } catch (error) {
      console.error('Error handling app foreground:', error);
      // Navigate to login on error
    }
  };
};
```

### 6. Login Screen Implementation
```javascript
// screens/LoginScreen.js
import React, { useState, useEffect } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert } from 'react-native';
import ApiService from '../services/ApiService';
import TokenManager from '../services/TokenManager';

const LoginScreen = ({ navigation }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    checkExistingTokens();
  }, []);

  const checkExistingTokens = async () => {
    const hasTokens = await TokenManager.loadTokens();
    if (hasTokens && !TokenManager.isRefreshTokenExpired()) {
      // User is already logged in
      navigation.replace('Home');
    }
  };

  const handleLogin = async () => {
    if (!email || !password) {
      Alert.alert('Error', 'Please enter email and password');
      return;
    }

    setLoading(true);
    try {
      await ApiService.login(email, password);
      navigation.replace('Home');
    } catch (error) {
      Alert.alert('Login Failed', error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={{ flex: 1, padding: 20, justifyContent: 'center' }}>
      <Text style={{ fontSize: 24, marginBottom: 30, textAlign: 'center' }}>
        Login
      </Text>
      
      <TextInput
        placeholder="Email"
        value={email}
        onChangeText={setEmail}
        keyboardType="email-address"
        autoCapitalize="none"
        style={{
          borderWidth: 1,
          borderColor: '#ddd',
          padding: 15,
          marginBottom: 15,
          borderRadius: 8,
        }}
      />
      
      <TextInput
        placeholder="Password"
        value={password}
        onChangeText={setPassword}
        secureTextEntry
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

export default LoginScreen;
```

### 7. Main App Component
```javascript
// App.js
import React, { useEffect } from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import LoginScreen from './screens/LoginScreen';
import HomeScreen from './screens/HomeScreen';
import { useAppState } from './hooks/useAppState';

const Stack = createStackNavigator();

const App = () => {
  useAppState(); // Handle app state changes

  return (
    <NavigationContainer>
      <Stack.Navigator initialRouteName="Login">
        <Stack.Screen 
          name="Login" 
          component={LoginScreen} 
          options={{ headerShown: false }}
        />
        <Stack.Screen 
          name="Home" 
          component={HomeScreen}
          options={{ headerLeft: null }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default App;
```

## Flutter Implementation

### 1. Dependencies (pubspec.yaml)
```yaml
dependencies:
  flutter_secure_storage: ^9.0.0
  local_auth: ^2.1.6
  http: ^1.1.0
  shared_preferences: ^2.2.2
```

### 2. Token Manager (Dart)
```dart
// services/token_manager.dart
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';

class TokenManager {
  static const _storage = FlutterSecureStorage();
  static const String _accessTokenKey = 'access_token';
  static const String _refreshTokenKey = 'refresh_token';
  static const String _tokenExpiryKey = 'token_expiry';
  static const String _refreshExpiryKey = 'refresh_expiry';

  String? _accessToken;
  String? _refreshToken;
  DateTime? _tokenExpiry;
  DateTime? _refreshTokenExpiry;

  // Getters
  String? get accessToken => _accessToken;
  String? get refreshToken => _refreshToken;
  DateTime? get tokenExpiry => _tokenExpiry;
  DateTime? get refreshTokenExpiry => _refreshTokenExpiry;

  Future<void> saveTokens({
    required String accessToken,
    required String refreshToken,
    required String expiration,
    required String refreshExpiration,
  }) async {
    await _storage.write(key: _accessTokenKey, value: accessToken);
    await _storage.write(key: _refreshTokenKey, value: refreshToken);
    
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenExpiryKey, expiration);
    await prefs.setString(_refreshExpiryKey, refreshExpiration);

    _accessToken = accessToken;
    _refreshToken = refreshToken;
    _tokenExpiry = DateTime.parse(expiration);
    _refreshTokenExpiry = DateTime.parse(refreshExpiration);
  }

  Future<bool> loadTokens() async {
    try {
      _accessToken = await _storage.read(key: _accessTokenKey);
      _refreshToken = await _storage.read(key: _refreshTokenKey);
      
      final prefs = await SharedPreferences.getInstance();
      final expiryStr = prefs.getString(_tokenExpiryKey);
      final refreshExpiryStr = prefs.getString(_refreshExpiryKey);
      
      if (expiryStr != null) _tokenExpiry = DateTime.parse(expiryStr);
      if (refreshExpiryStr != null) _refreshTokenExpiry = DateTime.parse(refreshExpiryStr);
      
      return _accessToken != null && _refreshToken != null;
    } catch (e) {
      return false;
    }
  }

  Future<void> clearTokens() async {
    await _storage.delete(key: _accessTokenKey);
    await _storage.delete(key: _refreshTokenKey);
    
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenExpiryKey);
    await prefs.remove(_refreshExpiryKey);

    _accessToken = null;
    _refreshToken = null;
    _tokenExpiry = null;
    _refreshTokenExpiry = null;
  }

  bool get isAccessTokenExpired {
    if (_tokenExpiry == null) return true;
    return DateTime.now().isAfter(_tokenExpiry!);
  }

  bool get isRefreshTokenExpired {
    if (_refreshTokenExpiry == null) return true;
    return DateTime.now().isAfter(_refreshTokenExpiry!);
  }

  bool get shouldRefreshToken {
    if (_tokenExpiry == null) return false;
    final fiveMinutesFromNow = DateTime.now().add(Duration(minutes: 5));
    return fiveMinutesFromNow.isAfter(_tokenExpiry!);
  }
}
```

## Key Mobile Best Practices

### 1. Security
- Always use secure storage for tokens
- Implement certificate pinning for API calls
- Use biometric authentication when available
- Clear tokens on app uninstall

### 2. User Experience
- Seamless background token refresh
- Biometric fallback to PIN
- Offline capability with cached data
- Loading states and error handling

### 3. Performance
- Minimize token refresh calls
- Cache API responses when appropriate
- Implement request queuing during token refresh
- Use background tasks for token maintenance

### 4. Error Handling
- Network connectivity issues
- Token refresh failures
- Biometric authentication failures
- Server errors and timeouts

This implementation provides a robust, secure, and user-friendly refresh token system for mobile apps that ensures users stay logged in while maintaining security through biometric authentication and PIN confirmation.