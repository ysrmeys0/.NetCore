document.querySelector('#admin-login-form').addEventListener('submit', function(event) {
    event.preventDefault();

    const email = this.querySelector('[name="Email"]').value;
    const phoneNumber = this.querySelector('[name="PhoneNumber"]').value;

    const loginData = {
        Email: email,
        PhoneNumber: phoneNumber
    };

    // fetch URL'ini /Admin/AdminGirisYap olarak güncelliyoruz
    fetch('/Admin/AdminGirisYap', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(loginData)
    })
    .then(response => {
        if (!response.ok) {
            // Hata yanıtlarını doğru şekilde yakala
            return response.json().then(errorData => {
                throw new Error(errorData.message || 'Giriş işlemi başarısız oldu.');
            });
        }
        return response.json();
    })
    .then(data => {
        alert(data.message);
        // Giriş başarılıysa Admin Dashboard'a yönlendir
        window.location.href = '/Admin/Dashboard';
    })
    .catch(error => {
        console.error('Hata:', error);
        alert(error.message);
    });
});
