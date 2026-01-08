# Mobile App Quick Start Guide

## Simple Implementation Steps

### 1. Basic Token Storage (React Native)

```javascript
// Install: npm install @react-native-keychain/react-native-keychain

import Keychain from '@react-native-keychain/react-native-keychain';

class SimpleTokenManager {
  async saveTokens(accessToken, refreshToken) {
    await Keychain.setInternetCredentials('auth', 'tokens', JSON.stringify({
      accessToken,
      refreshToken,
      savedAt: new Date().toISOString()
    }));
  }

  async getTokens() {
    try {
      const credentials = await Keychain.getInternetCredentials('auth');
      return credentials ? JSON.parse(credentials.password) : null;
    } catch {
      return null;
    }
  }

  async clearTokens() {
    await Keychain.resetInternetCredentials('auth');
  }
}
```

### 2. API Service with Auto-Refresh

```javascript
const API_URL = 'https://your-api.com/api';
const tokenManager = new SimpleTokenManager();

class ApiClient {
  async request(endpoint, options = {}) {
    let tokens = await tokenManager.getTokens();
    
    // Try request with current token
    let response = await fetch(`${API_URL}${endpoint}`, {
      ...options,
      headers: {
        'Authorization': `Bearer ${tokens?.accessToken}`,
        'Content-Type': 'application/json',
        ...options.headers,
      },
    });

    // If 401, try to refresh token
    if (response.status === 401 && tokens?.refreshToken) {
      const refreshResponse = await fetch(`${API_URL}/auth/refresh-token`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken: tokens.refreshToken }),
      });

      if (refreshResponse.ok) {
        const newTokens = await refreshResponse.json();
        await tokenManager.saveTokens(newTokens.token, newTokens.refreshToken);
        
        // Retry original request
        response = await fetch(`${API_URL}${endpoint}`, {
          ...options,
          headers: {
            'Authorization': `Bearer ${newTokens.token}`,
            'Content-Type': 'application/json',
            ...options.headers,
          },
        });
      } else {
        // Refresh failed, redirect to login
        await tokenManager.clearTokens();
        throw new Error('Session expired');
      }
    }

    return response;
  }

  async login(email, password) {
    const response = await fetch(`${API_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });

    if (response.ok) {
      const data = await response.json();
      await tokenManager.saveTokens(data.token, data.refreshToken);
      return data;
    }
    throw new Error('Login failed');
  }

  async logout() {
    const tokens = await tokenManager.getTokens();
    if (tokens?.refreshToken) {
      await this.request('/auth/revoke-token', {
        method: 'POST',
        body: JSON.stringify({ refreshToken: tokens.refreshToken }),
      });
    }
    await tokenManager.clearTokens();
  }
}

export default new ApiClient();
```

### 3. Login Screen

```javascript
import React, { useState, useEffect } from 'react';
import { View, TextInput, TouchableOpacity, Text, Alert } from 'react-native';
import ApiClient from './ApiClient';

const LoginScreen = ({ navigation }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  useEffect(() => {
    checkExistingLogin();
  }, []);

  const checkExistingLogin = async () => {
    const tokens = await tokenManager.getTokens();
    if (tokens) {
      // Test if tokens are still valid
      try {
        await ApiClient.request('/auth/user/1'); // Any protected endpoint
        navigation.replace('Home');
      } catch {
        // Tokens invalid, stay on login
      }
    }
  };

  const handleLogin = async () => {
    try {
      await ApiClient.login(email, password);
      navigation.replace('Home');
    } catch (error) {
      Alert.alert('Login Failed', error.message);
    }
  };

  return (
    <View style={{ padding: 20, flex: 1, justifyContent: 'center' }}>
      <TextInput
        placeholder="Email"
        value={email}
        onChangeText={setEmail}
        style={{ borderWidth: 1, padding: 10, marginBottom: 10 }}
      />
      <TextInput
        placeholder="Password"
        value={password}
        onChangeText={setPassword}
        secureTextEntry
        style={{ borderWidth: 1, padding: 10, marginBottom: 20 }}
      />
      <TouchableOpacity
        onPress={handleLogin}
        style={{ backgroundColor: 'blue', padding: 15, alignItems: 'center' }}
      >
        <Text style={{ color: 'white' }}>Login</Text>
      </TouchableOpacity>
    </View>
  );
};
```

### 4. Using API in Components

```javascript
import React, { useEffect, useState } from 'react';
import { View, Text, FlatList } from 'react-native';
import ApiClient from './ApiClient';

const UsersScreen = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      const response = await ApiClient.request('/users');
      const data = await response.json();
      setUsers(data);
    } catch (error) {
      console.error('Failed to load users:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <Text>Loading...</Text>;

  return (
    <FlatList
      data={users}
      keyExtractor={(item) => item.id.toString()}
      renderItem={({ item }) => (
        <View style={{ padding: 10 }}>
          <Text>{item.name}</Text>
          <Text>{item.email}</Text>
        </View>
      )}
    />
  );
};
```

## Flutter Quick Implementation

### 1. Token Storage (Flutter)

```dart
// pubspec.yaml: flutter_secure_storage: ^9.0.0

import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'dart:convert';

class TokenManager {
  static const _storage = FlutterSecureStorage();
  
  Future<void> saveTokens(String accessToken, String refreshToken) async {
    final tokens = {
      'accessToken': accessToken,
      'refreshToken': refreshToken,
      'savedAt': DateTime.now().toIso8601String(),
    };
    await _storage.write(key: 'auth_tokens', value: jsonEncode(tokens));
  }

  Future<Map<String, dynamic>?> getTokens() async {
    try {
      final tokensStr = await _storage.read(key: 'auth_tokens');
      return tokensStr != null ? jsonDecode(tokensStr) : null;
    } catch {
      return null;
    }
  }

  Future<void> clearTokens() async {
    await _storage.delete(key: 'auth_tokens');
  }
}
```

### 2. API Service (Flutter)

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class ApiService {
  static const String baseUrl = 'https://your-api.com/api';
  final TokenManager _tokenManager = TokenManager();

  Future<http.Response> request(String endpoint, {
    String method = 'GET',
    Map<String, dynamic>? body,
  }) async {
    final tokens = await _tokenManager.getTokens();
    
    var response = await _makeRequest(endpoint, method, body, tokens?['accessToken']);
    
    // Handle 401 - try refresh
    if (response.statusCode == 401 && tokens?['refreshToken'] != null) {
      final refreshResponse = await http.post(
        Uri.parse('$baseUrl/auth/refresh-token'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'refreshToken': tokens!['refreshToken']}),
      );

      if (refreshResponse.statusCode == 200) {
        final newTokens = jsonDecode(refreshResponse.body);
        await _tokenManager.saveTokens(newTokens['token'], newTokens['refreshToken']);
        
        // Retry original request
        response = await _makeRequest(endpoint, method, body, newTokens['token']);
      } else {
        await _tokenManager.clearTokens();
        throw Exception('Session expired');
      }
    }

    return response;
  }

  Future<http.Response> _makeRequest(String endpoint, String method, 
      Map<String, dynamic>? body, String? token) async {
    final headers = {'Content-Type': 'application/json'};
    if (token != null) headers['Authorization'] = 'Bearer $token';

    final uri = Uri.parse('$baseUrl$endpoint');
    
    switch (method.toUpperCase()) {
      case 'POST':
        return http.post(uri, headers: headers, body: body != null ? jsonEncode(body) : null);
      case 'PUT':
        return http.put(uri, headers: headers, body: body != null ? jsonEncode(body) : null);
      case 'DELETE':
        return http.delete(uri, headers: headers);
      default:
        return http.get(uri, headers: headers);
    }
  }

  Future<Map<String, dynamic>> login(String email, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/auth/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'email': email, 'password': password}),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      await _tokenManager.saveTokens(data['token'], data['refreshToken']);
      return data;
    }
    throw Exception('Login failed');
  }
}
```

## Key Points for Mobile

1. **Always use secure storage** - Never store tokens in plain text
2. **Handle network errors** - Mobile networks are unreliable
3. **Auto-refresh tokens** - Do it transparently in the background
4. **Check tokens on app start** - Users might already be logged in
5. **Clear tokens on logout** - Always revoke server-side too

## Testing the Implementation

1. **Login** → Should save tokens securely
2. **Make API calls** → Should work with saved tokens
3. **Wait 24 hours** → Tokens should auto-refresh
4. **Kill and restart app** → Should stay logged in
5. **Logout** → Should clear all tokens

This gives you a working "never logged out" experience in your mobile app!