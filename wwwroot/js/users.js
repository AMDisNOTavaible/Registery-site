async function loadUsers() {
    try {
        const response = await fetch('/users');
        if (response.ok) {
            const userList = document.getElementById('userList');
            const html = await response.text();
            userList.innerHTML = html;
        } else {
            console.error('Failed to load users');
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

async function deleteUser(username) {
    if (!confirm(`Вы уверены, что хотите удалить пользователя ${username}?`)) {
        return;
    }

    try {
        const response = await fetch(`/users/${username}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            loadUsers(); // Перезагрузить список пользователей
        } else {
            alert('Не удалось удалить пользователя');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Произошла ошибка при удалении пользователя');
    }
}

// Загрузить пользователей при загрузке страницы
document.addEventListener('DOMContentLoaded', loadUsers); 