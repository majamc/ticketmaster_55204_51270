async function findConcerts() {
    const keyword = document.getElementById('keyword')?.value.trim();
    const lista = document.getElementById('lista-koncertow');

    if (!keyword) {
        alert('Enter an artist/band name please.');
        return;
    }

    lista.innerHTML = `<p class="text">Loading...</p>`;

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`http://localhost:5101/api/events/${encodeURIComponent(keyword)}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (response.status === 401) {
            alert("Your session has expired. Log in again.");
            localStorage.removeItem('token');
            window.location.href = 'index.html';
            return;
        }

        const dane = await response.json();

        if (!response.ok) {
            lista.innerHTML = '';
            alert(dane?.error || 'Error while finding concerts.');
            return;
        }

        lista.innerHTML = '';

        if (!dane || dane.length === 0) {
            alert('No concerts found.');
            return;
        }

        dane.forEach(koncert => {
            const li = document.createElement('li');
            li.innerHTML = `
              <strong>${koncert.name}</strong><br>
              Venue: ${koncert.venue}<br>
              Date: ${koncert.date}<br>
              Top Songs:<br> ${koncert.topSongs.map(song => `&nbsp;&nbsp;- ${song}`).join('<br>')}
            `;
            lista.appendChild(li);
        });

    } catch (err) {
        console.error(err);
        alert('Error while finding concerts.');
    }
}
async function logIn() {
    const email = document.getElementById('email')?.value.trim();
    const password = document.getElementById('password')?.value.trim();
    const komunikat = document.getElementById('komunikat');

    try {
        const response = await fetch('http://localhost:5101/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const dane = await response.json();

        if (!response.ok) {
            alert(dane.error || 'Unknown error');
            return;
        }

        localStorage.setItem('token', dane.token);
        alert('Zalogowano pomyślnie!');
        window.location.href = 'concerts.html';
    } catch (err) {
        console.error(err);
        alert('Error while logging in.');
    }
}
async function signUp() {
    try {
        const response = await fetch('http://localhost:5101/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                email: document.getElementById('email').value,
                password: document.getElementById('password').value
            })
        });

        if (!response.ok) {
            const errorData = await response.json();
            alert(errorData.error || 'Unknown error');
            return;
        }

        alert('Registration successful! You can log in now.');
    } catch (err) {
        console.error(err);
        alert('Error while signing up.');
    }
}