document.addEventListener('DOMContentLoaded', function() {
    // Получаем имя пользователя из URL параметров
    const urlParams = new URLSearchParams(window.location.search);
    const username = urlParams.get('username');
    
    if (username) {
        document.getElementById('username').textContent = username;
    } else {
        // Если нет параметра username, перенаправляем на страницу регистрации
        window.location.href = '/register.html';
    }
}); 