// // HTML elemanlarına erişim
// const loginFormWrapper = document.getElementById('login-form-wrapper');
// const registerFormWrapper = document.getElementById('register-form-wrapper');
// const showRegisterBtn = document.getElementById('show-register-form');
// const showLoginBtn = document.getElementById('show-login-form');
// const patientLoginForm = document.querySelector('.patient-panel .login-form');
// const staffLoginForm = document.querySelector('.staff-panel .login-form');

// // Formları gizleme/gösterme mantığı
// showRegisterBtn.addEventListener('click', () => {
//     loginFormWrapper.classList.add('hidden-form');
//     registerFormWrapper.classList.remove('hidden-form');
// });

// showLoginBtn.addEventListener('click', () => {
//     loginFormWrapper.classList.remove('hidden-form');
//     registerFormWrapper.classList.add('hidden-form');
// });

// // --- Kayıt Formu İşlemi ---
// const registerForm = document.querySelector('.register-form');
// if (registerForm) {
//     registerForm.addEventListener('submit', function(event) {
//         event.preventDefault();

//         // Her bir elemanın DEĞERİNİ al
//         const name = this.querySelector('[name="Name"]').value;
//         const surname = this.querySelector('[name="Surname"]').value;
//         const genderValue = this.querySelector('[name="Gender"]').value;
//         const email = this.querySelector('[name="Email"]').value;
//         const phoneNumber = this.querySelector('[name="PhoneNumber"]').value;

//         const userData = {
//             Name: name,
//             Surname: surname,
//             Gender: genderValue === "" ? null : parseInt(genderValue),
//             Email: email,
//             PhoneNumber: phoneNumber
//         };

//         fetch('/api/KisilerApi/kayitol', {
//             method: 'POST',
//             headers: {
//                 'Content-Type': 'application/json'
//             },
//             body: JSON.stringify(userData)
//         })
//         .then(response => {
//             if (!response.ok) {
//                 return response.json().then(errorData => {
//                     throw new Error(errorData.message || 'Kayıt işlemi başarısız oldu.');
//                 });
//             }
//             return response.json();
//         })
//         .then(data => {
//             alert(data.message);
//             this.reset();
//             loginFormWrapper.classList.remove('hidden-form');
//             registerFormWrapper.classList.add('hidden-form');
//         })
//         .catch(error => {
//             console.error('Hata:', error);
//             alert(error.message);
//         });
//     });
// }

// // --- Hasta Giriş Formu İşlemi ---
// if (patientLoginForm) {
//     patientLoginForm.addEventListener('submit', function(event) {
//         event.preventDefault();

//         const email = this.querySelector('[name="Email"]').value;
//         const phoneNumber = this.querySelector('[name="PhoneNumber"]').value;

//         const loginData = {
//             Email: email,
//             PhoneNumber: phoneNumber
//         };

//         fetch('/api/KisilerApi/girisyap', {
//             method: 'POST',
//             headers: {
//                 'Content-Type': 'application/json'
//             },
//             body: JSON.stringify(loginData)
//         })
//         .then(response => {
//             if (!response.ok) {
//                 return response.json().then(errorData => {
//                     throw new Error(errorData.message || 'Giriş işlemi başarısız oldu.');
//                 });
//             }
//             return response.json();
//         })
//         .then(data => {
//             alert(data.message);
//             if (data.redirectUrl) {
//                 window.location.href = data.redirectUrl;
//             }
//         })
//         .catch(error => {
//             console.error('Hata:', error);
//             alert(error.message);
//         });
//     });
// }

// //İdari Giriş Formu
// // --- Hasta Giriş Formu İşlemi ---
// if (staffLoginForm) {
//     staffLoginForm.addEventListener('submit', function(event) {
//         event.preventDefault();

//         const email = this.querySelector('[name="Email"]').value;
//         const phoneNumber = this.querySelector('[name="PhoneNumber"]').value;

//         const loginData = {
//             Email: email,
//             PhoneNumber: phoneNumber
//         };

//         fetch('/api/KisilerApi/staffgirisyap', {
//             method: 'POST',
//             headers: {
//                 'Content-Type': 'application/json'
//             },
//             body: JSON.stringify(loginData)
//         })
//         .then(response => {
//             if (!response.ok) {
//                 return response.json().then(errorData => {
//                     throw new Error(errorData.message || 'Giriş işlemi başarısız oldu.');
//                 });
//             }
//             return response.json();
//         })
//         .then(data => {
//             alert(data.message);
//             // Giriş başarılıysa başka bir sayfaya yönlendirme yapılabilir
//         })
//         .catch(error => {
//             console.error('Hata:', error);
//             alert(error.message);
//         });
//     });
// }

// HTML elemanlarına erişim
const loginFormWrapper = document.getElementById('login-form-wrapper');
const registerFormWrapper = document.getElementById('register-form-wrapper');
const showRegisterBtn = document.getElementById('show-register-form');
const showLoginBtn = document.getElementById('show-login-form');
const patientLoginForm = document.querySelector('.patient-panel .login-form');
const staffLoginForm = document.querySelector('.staff-panel .login-form');

// Formları gizleme/gösterme mantığı
showRegisterBtn.addEventListener('click', () => {
    loginFormWrapper.classList.add('hidden-form');
    registerFormWrapper.classList.remove('hidden-form');
});

showLoginBtn.addEventListener('click', () => {
    loginFormWrapper.classList.remove('hidden-form');
    registerFormWrapper.classList.add('hidden-form');
});

// --- Kayıt Formu İşlemi ---
const registerForm = document.querySelector('.register-form');
if (registerForm) {
    registerForm.addEventListener('submit', function(event) {
        event.preventDefault();

        const name = this.querySelector('[name="Name"]').value;
        const surname = this.querySelector('[name="Surname"]').value;
        const genderValue = this.querySelector('[name="Gender"]').value;
        const email = this.querySelector('[name="Email"]').value;
        const phoneNumber = this.querySelector('[name="PhoneNumber"]').value;

        const userData = {
            Name: name,
            Surname: surname,
            Gender: genderValue === "" ? null : parseInt(genderValue),
            Email: email,
            PhoneNumber: phoneNumber
        };

        fetch('/api/KisilerApi/kayitol', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.message || 'Kayıt işlemi başarısız oldu.');
                });
            }
            return response.json();
        })
        .then(data => {
            alert(data.message);
            this.reset();
            loginFormWrapper.classList.remove('hidden-form');
            registerFormWrapper.classList.add('hidden-form');
        })
        .catch(error => {
            console.error('Hata:', error);
            alert(error.message);
        });
    });
}

// --- Hasta Giriş Formu İşlemi ---
if (patientLoginForm) {
    patientLoginForm.addEventListener('submit', function(event) {
        event.preventDefault();

        const email = this.querySelector('[name="Email"]').value;
        const phoneNumber = this.querySelector('[name="PhoneNumber"]').value;

        const loginData = {
            Email: email,
            PhoneNumber: phoneNumber
        };

        fetch('/api/KisilerApi/girisyap', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.message || 'Giriş işlemi başarısız oldu.');
                });
            }
            return response.json();
        })
        .then(data => {
            alert(data.message);
            if (data.success && data.user) {
                // Sadece kullanıcı ID'sini ve diğer gerekli verileri sessionStorage'a kaydet
                sessionStorage.setItem('loggedInUser', JSON.stringify({
                    id: data.user.id,
                    name: data.user.name,
                    surname: data.user.surname,
                    email: data.user.email,
                    phoneNumber: data.user.phoneNumber
                }));
            }
            if (data.redirectUrl) {
                window.location.href = data.redirectUrl;
            }
        })
        .catch(error => {
            console.error('Hata:', error);
            alert(error.message);
        });
    });
}

//İdari Giriş Formu
// --- Hasta Giriş Formu İşlemi ---
if (staffLoginForm) {
    staffLoginForm.addEventListener('submit', function(event) {
        event.preventDefault();

        const email = this.querySelector('[name="Email"]').value;
        const phoneNumber = this.querySelector('[name="PhoneNumber"]').value;

        const loginData = {
            Email: email,
            PhoneNumber: phoneNumber
        };

        fetch('/api/KisilerApi/staffgirisyap', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.message || 'Giriş işlemi başarısız oldu.');
                });
            }
            return response.json();
        })
        .then(data => {
            alert(data.message);
            // if (data.success && data.user) {
            //     // Sadece kullanıcı ID'sini ve diğer gerekli verileri sessionStorage'a kaydet
            //     sessionStorage.setItem('loggedInUser', JSON.stringify({
            //         id: data.user.id,
            //         name: data.user.name,
            //         surname: data.user.surname,
            //         email: data.user.email,
            //         phoneNumber: data.user.phoneNumber
            //     }));
            // }
            if (data.redirectUrl) {
                window.location.href = data.redirectUrl;
            }
        })
        .catch(error => {
            console.error('Hata:', error);
            alert(error.message);
        });
    });
}
