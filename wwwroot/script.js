async function findConcerts() {
    const keyword = document.getElementById('keyword')?.value.trim();
    const lista = document.getElementById('lista-koncertow');
    const topSongsElem = document.getElementById('top-songs');
    const photoElem = document.getElementById('photo');

    if (!keyword) {
        alert('Enter an artist/band name please.');
        return;
    }

    topSongsElem.innerHTML = ``;
    photoElem.innerHTML = ``;

    lista.innerHTML = `<p class="loading">Loading...</p>`;

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

        if (!dane || dane.length === 0) {
            alert('No concerts found.');
            return;
        }

        const topSongs = dane[0]?.topSongs || [];
        const photo = dane[0];

        if (photo?.artistImageUrl) {
            photoElem.innerHTML = `
            <div class="artist-div">
                <img src="${photo.artistImageUrl}" alt="Image of artist ${photo.name}" class="artist-photo" />
            </div>
        `;
        } else {
            photoElem.innerHTML = "<p>No image of the artist</p>";
        }

        topSongsElem.innerHTML = `
        <div class="top-songs-div">
          <strong>Top songs:</strong>
          <ul>
            ${topSongs.map(song => `<li>${song}</li>`).join('')}
          </ul>
        </div>
        `;

        lista.innerHTML = `
            <p class="upcoming"><strong>Upcoming concerts:</strong></p>
        `;

        dane.forEach(koncert => {
            const li = document.createElement('li');
            li.innerHTML = `
              <strong>${koncert.name}</strong><br>
              Venue: ${koncert.venue}<br>
              Date: ${koncert.date}<br>
              
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
        alert('Log in succesfull!');
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