#!/usr/bin/env bash
set -euo pipefail

API_URL="${API_URL:-http://localhost:5000}"
EMAIL="verify-$(date +%s)@test.local"
PASSWORD="Password1"

echo "Registering $EMAIL..."
REGISTER=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\",\"displayName\":\"Verify User\"}")

REGISTER_BODY=$(echo "$REGISTER" | head -n -1)
REGISTER_CODE=$(echo "$REGISTER" | tail -n 1)
echo "Register status: $REGISTER_CODE"

if [ "$REGISTER_CODE" != "201" ]; then
  echo "$REGISTER_BODY"
  exit 1
fi

echo "Logging in..."
LOGIN=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}")

LOGIN_BODY=$(echo "$LOGIN" | head -n -1)
LOGIN_CODE=$(echo "$LOGIN" | tail -n 1)
echo "Login status: $LOGIN_CODE"

if [ "$LOGIN_CODE" != "200" ]; then
  echo "$LOGIN_BODY"
  exit 1
fi

echo "$LOGIN_BODY" | grep -q accessToken && echo "Login verified successfully."
