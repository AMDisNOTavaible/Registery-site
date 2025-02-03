async function handleRegister(event) {
    event.preventDefault();

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();
    const confirmPassword = document.getElementById('confirm-password').value.trim();
    const errorElement = document.getElementById('error-message');

    // Очистка ошибок
    errorElement.textContent = '';

    // Проверка паролей
    if (password !== confirmPassword) {
        errorElement.textContent = 'Пароли не совпадают';
        return;
    }

    try {
        const formData = new FormData();
        formData.append('username', username);
        formData.append('password', password);

        const response = await fetch('/register', {
            method: 'POST',
            body: formData
        });

        if (response.redirected) {
            window.location.href = response.url;
        } else {
            const text = await response.text();
            errorElement.textContent = text;
        }
    } catch (error) {
        console.error('Error:', error);
        errorElement.textContent = 'Произошла ошибка при регистрации';
    }
}

// Добавляем функцию переключения видимости пароля
function togglePassword(inputId) {
    const passwordInput = document.getElementById(inputId);
    const passwordType = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordInput.setAttribute('type', passwordType);
} 