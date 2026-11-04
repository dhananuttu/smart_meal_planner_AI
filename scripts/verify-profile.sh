#!/usr/bin/env bash
set -euo pipefail

API_URL="${API_URL:-http://localhost:5000}"
EMAIL="profile-$(date +%s)@test.local"
PASSWORD="Password1"

echo "Registering $EMAIL..."
REGISTER=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\",\"displayName\":\"Profile User\"}")

REGISTER_BODY=$(echo "$REGISTER" | head -n -1)
REGISTER_CODE=$(echo "$REGISTER" | tail -n 1)

if [ "$REGISTER_CODE" != "201" ]; then
  echo "Register failed: $REGISTER_CODE"
  echo "$REGISTER_BODY"
  exit 1
fi

ACCESS=$(echo "$REGISTER_BODY" | python3 -c "import sys,json; print(json.load(sys.stdin)['data']['accessToken'])")

echo "Getting profile..."
GET=$(curl -s -w "\n%{http_code}" -X GET "$API_URL/api/users/me" \
  -H "Authorization: Bearer $ACCESS")

GET_BODY=$(echo "$GET" | head -n -1)
GET_CODE=$(echo "$GET" | tail -n 1)
echo "GET /api/users/me status: $GET_CODE"

if [ "$GET_CODE" != "200" ]; then
  echo "$GET_BODY"
  exit 1
fi

echo "$GET_BODY" | grep -q '"displayName":"Profile User"' && echo "Profile display name verified."

echo "Updating profile..."
PUT=$(curl -s -w "\n%{http_code}" -X PUT "$API_URL/api/users/me" \
  -H "Authorization: Bearer $ACCESS" \
  -H "Content-Type: application/json" \
  -d '{"displayName":"Updated Profile","defaultServings":6,"timezone":"UTC","dietaryPreferences":["vegetarian","gluten-free"]}')

PUT_BODY=$(echo "$PUT" | head -n -1)
PUT_CODE=$(echo "$PUT" | tail -n 1)
echo "PUT /api/users/me status: $PUT_CODE"

if [ "$PUT_CODE" != "200" ]; then
  echo "$PUT_BODY"
  exit 1
fi

echo "$PUT_BODY" | grep -q '"displayName":"Updated Profile"' && echo "Profile update verified successfully."
