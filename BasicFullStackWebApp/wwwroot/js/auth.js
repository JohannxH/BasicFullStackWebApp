const apiBaseUrl = 'https://localhost:5001/api';
let accessToken = 'initial_jwt_token'; // This should be set after login
let tokenIssuedAt = new Date();
let tokenExpiresAt = new Date();

async function refreshToken() {
    const response = await fetch(`${apiBaseUrl}/auth/refresh-token`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${accessToken}`
        }
    });

    if (response.ok) {
        const data = await response.json();
        accessToken = data.token;
        tokenIssuedAt = new Date();
        tokenExpiresAt = new Date(tokenIssuedAt.getTime() + 5 * 60000); // Token expires in 5 minutes
        console.log('Token refreshed:', accessToken);
        updateTokenInfo();
    } else {
        console.error('Failed to refresh token');
    }
}

async function makeAuthenticatedRequest(endpoint, options = {}) {
    options.headers = options.headers || {};
    options.headers['Authorization'] = `Bearer ${accessToken}`;

    let response = await fetch(`${apiBaseUrl}${endpoint}`, options);

    if (response.status === 401) {
        // Token might be expired, try to refresh it
        await refreshToken();
        options.headers['Authorization'] = `Bearer ${accessToken}`;
        response = await fetch(`${apiBaseUrl}${endpoint}`, options);
    }

    return response;
}

function updateTokenInfo() {
    document.getElementById('token-issued').textContent = `Token Issued At: ${tokenIssuedAt.toLocaleTimeString()}`;
    document.getElementById('token-expires').textContent = `Token Expires At: ${tokenExpiresAt.toLocaleTimeString()}`;
    startCountdown();
}

function startCountdown() {
    const countdownElement = document.getElementById('countdown');
    const interval = setInterval(() => {
        const now = new Date();
        const timeLeft = tokenExpiresAt - now;
        if (timeLeft <= 0) {
            clearInterval(interval);
            countdownElement.textContent = 'Token expired';
        } else {
            const minutes = Math.floor(timeLeft / 60000);
            const seconds = Math.floor((timeLeft % 60000) / 1000);
            countdownElement.textContent = `Token expires in: ${minutes}m ${seconds}s`;
        }
    }, 1000);
}

// Example usage
async function getUserData() {
    const response = await makeAuthenticatedRequest('/user/data');
    if (response.ok) {
        const data = await response.json();
        document.querySelector('.content').innerHTML = `<p>Welcome, ${data.username}! Here you can view your profile, adjust settings, and more.</p>`;
    } else {
        console.error('Failed to fetch user data');
    }
}

// Call getUserData to test
getUserData();
