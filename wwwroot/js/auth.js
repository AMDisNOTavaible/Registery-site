// Функция для обработки входа
async function handleLogin(event) {
    event.preventDefault();

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();
    const usernameError = document.getElementById('username-error');
    const passwordError = document.getElementById('password-error');

    // Сброс ошибок
    clearErrors();

    // Проверка полей
    if (!validateFields(username, password)) {
        return;
    }

    await attemptLogin(username, password);
}

// Функция проверки полей
function validateFields(username, password) {
    if (!username) {
        showError('username-error', 'Введите имя пользователя');
        return false;
    }

    if (!password) {
        showError('password-error', 'Введите пароль');
        return false;
    }

    return true;
}

// Функция попытки входа
async function attemptLogin(username, password) {
    try {
        const formData = new FormData();
        formData.append('username', username);
        formData.append('password', password);

        const response = await fetch('/login', {
            method: 'POST',
            body: formData
        });

        // Если есть перенаправление, следуем ему
        if (response.redirected) {
            // Проверяем, куда нас перенаправляют
            if (response.url.includes('success.html')) {
                window.location.href = response.url;
                return true;
            } else {
                // Если перенаправление не на success.html, значит это ошибка
                showError('username-error', 'Неверное имя пользователя или пароль');
                return false;
            }
        }

        if (response.url.includes('error=invalid')) {
            showError('username-error', 'Неверное имя пользователя или пароль');
            return false;
        }

        if (response.url.includes('error=empty')) {
            showError('username-error', 'Введите имя пользователя и пароль');
            return false;
        }

        return false;
    } catch (error) {
        console.error('Ошибка при попытке входа:', error);
        showError('username-error', 'Произошла ошибка при входе в систему');
        return false;
    }
}

// Функция очистки ошибок
function clearErrors() {
    document.getElementById('username-error').textContent = '';
    document.getElementById('password-error').textContent = '';
}

// Функция отображения ошибки
function showError(elementId, message) {
    document.getElementById(elementId).textContent = message;
}

// Функция переключения видимости пароля
function togglePassword() {
    const passwordInput = document.getElementById('password');
    const passwordType = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordInput.setAttribute('type', passwordType);
} 