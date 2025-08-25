// Sayfa yüklendiğinde çalışacak olay dinleyicisi
document.addEventListener('DOMContentLoaded', async () => {

    const editProfileLink = document.getElementById('edit-profile-link');
    const createAppointmentLink = document.getElementById('create-appointment-link');
    const appointmentsLink = document.getElementById('appointments-link'); 
    const logoutLink = document.getElementById('logout-link'); // Çıkış yap bağlantısı
    const editProfileSection = document.getElementById('edit-profile-section');
    const createAppointmentSection = document.getElementById('create-appointment-section');
    const appointmentsSection = document.getElementById('appointments-section');

    const showSection = (section) => {
        document.querySelectorAll('.content-section').forEach(s => s.classList.remove('active'));
        section.classList.add('active');
    };

    editProfileLink.addEventListener('click', (e) => {
        e.preventDefault();
        showSection(editProfileSection);
    });

    createAppointmentLink.addEventListener('click', (e) => {
        e.preventDefault();
        showSection(createAppointmentSection);
    });
    
    appointmentsLink.addEventListener('click', (e) => {
        e.preventDefault();
        showSection(appointmentsSection);
        fetchAppointments();
    });

    // Çıkış yapma fonksiyonu
    logoutLink.addEventListener('click', (e) => {
        e.preventDefault();
        sessionStorage.removeItem('loggedInUser'); // Kullanıcı verisini session'dan sil
        window.location.href = '/'; // Kullanıcıyı giriş sayfasına yönlendir
        // Not: Yukarıdaki '/login.html' yolu uygulamanızın giriş sayfasına göre düzenlenmelidir.
    });


    // --- Bilgilerimi Düzenle Formu ---
    const editProfileForm = document.getElementById('edit-profile-form');
    const profileMessage = document.getElementById('profile-message');
    const nameInput = document.getElementById('name');
    const surnameInput = document.getElementById('surname');
    const emailInput = document.getElementById('email');
    const phoneNumberInput = document.getElementById('phoneNumber');

    const loadUserInfo = () => {
        const userJson = sessionStorage.getItem('loggedInUser');
        if (userJson) {
            try {
                const user = JSON.parse(userJson);
                nameInput.value = user.name;
                surnameInput.value = user.surname;
                emailInput.value = user.email;
                phoneNumberInput.value = user.phoneNumber;
            } catch (e) {
                console.error('Kullanıcı verisi okunurken hata:', e);
                showMessage(profileMessage, 'Kullanıcı bilgileri yüklenemedi.', 'error');
            }
        } else {
            showMessage(profileMessage, 'Oturum bilgileri bulunamadı. Lütfen giriş yapın.', 'error');
        }
    };
    
    loadUserInfo();

    editProfileForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const user = JSON.parse(sessionStorage.getItem('loggedInUser'));
        if (!user || !user.id) {
            showMessage(profileMessage, 'Oturum bilgisi eksik. Lütfen tekrar giriş yapın.', 'error');
            return;
        }

        const updatedUserData = {
            Id: user.id,
            Name: nameInput.value,
            Surname: surnameInput.value,
            Email: emailInput.value,
            PhoneNumber: phoneNumberInput.value
        };
        
        try {
            const response = await fetch('/Patient/updateProfile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updatedUserData)
            });

            if (response.ok) {
                const data = await response.json();
                showMessage(profileMessage, data.message, 'success');
                sessionStorage.setItem('loggedInUser', JSON.stringify({ ...user, ...updatedUserData }));
            } else {
                const errorText = await response.text();
                console.error('API Hatası:', errorText);
                showMessage(profileMessage, 'Bilgiler güncellenirken bir sorun oluştu. Detaylar konsolda.', 'error');
            }
        } catch (error) {
            console.error('Hata:', error);
            showMessage(profileMessage, 'İstek gönderilirken bir sorun oluştu.', 'error');
        }
    });

    // --- Randevu Oluşturma Formu ---
    const createAppointmentForm = document.getElementById('create-appointment-form');
    const appointmentMessage = document.getElementById('appointment-message');
    const doctorSelect = document.getElementById('doctor-select');
    const appointmentTypeSelect = document.getElementById('appointment-type-select');
    const appointmentDateInput = document.getElementById('appointment-date');
    const appointmentHourSelect = document.getElementById('appointment-hour-select');

    const fetchDoctors = async () => {
        try {
            const response = await fetch('/Patient/getDoctors');
            if (response.ok) {
                const doctors = await response.json();
                doctorSelect.innerHTML = '<option value="">Doktor Seçiniz</option>';
                doctors.forEach(doctor => {
                    const option = document.createElement('option');
                    option.value = doctor.id;
                    option.textContent = `${doctor.name} ${doctor.surname}`;
                    doctorSelect.appendChild(option);
                });
            } else {
                console.error('Doktor listesi alınırken hata:', response.statusText);
            }
        } catch (error) {
            console.error('Doktor listesi alınırken hata:', error);
        }
    };

    const fetchAppointmentTypes = async () => {
        try {
            const response = await fetch('/Patient/getAppointmentTypes');
            if (response.ok) {
                const appointmentTypes = await response.json();
                appointmentTypeSelect.innerHTML = '<option value="">Randevu Türü Seçiniz</option>';
                appointmentTypes.forEach(type => {
                    const option = document.createElement('option');
                    option.value = type.id;
                    option.textContent = type.name;
                    appointmentTypeSelect.appendChild(option);
                });
            } else {
                console.error('Randevu tipleri alınırken hata:', response.statusText);
            }
        } catch (error) {
            console.error('Randevu tipleri alınırken hata:', error);
        }
    };

    // Yardımcı fonksiyon: Randevu saatlerini 30 dakikalık aralıklarla oluşturmak için
    const populateAppointmentHours = () => {
        const appointmentHourSelect = document.getElementById('appointment-hour-select');
        // Seçenekleri temizle ve başlangıç metni ekle
        appointmentHourSelect.innerHTML = '<option value="">Saat Seçiniz</option>';

        // Başlangıç ve bitiş saatlerini ayarla
        const startTime = new Date();
        startTime.setHours(12, 0, 0);
        const endTime = new Date();
        endTime.setHours(18, 0, 0);

        // Zaman döngüsü
        while (startTime <= endTime) {
            // Saati ve dakikayı alıp, 0 ekleyerek formatla (örn: 12:00, 12:30)
            const hour = startTime.getHours().toString().padStart(2, '0');
            const minute = startTime.getMinutes().toString().padStart(2, '0');
            const timeString = `${hour}:${minute}`;
            
            // Yeni bir option elementi oluştur
            const option = document.createElement('option');
            option.value = timeString;
            option.textContent = timeString;
            appointmentHourSelect.appendChild(option);

            // Saati 30 dakika artır
            startTime.setMinutes(startTime.getMinutes() + 30);
        }
    };
    
    // --- Randevularım Bölümü ve İptal Fonksiyonu ---
    const appointmentsList = document.getElementById('appointments-list');
    const appointmentsMessage = document.getElementById('appointments-message');
    const cancellationModal = document.getElementById('cancellation-modal');
    const modalCloseBtn = document.querySelector('#cancellation-modal .close-btn');
    const modalVazgecBtn = document.getElementById('modal-close-btn');
    const modalConfirmBtn = document.getElementById('modal-confirm-btn');
    const modalDoctorName = document.getElementById('modal-doctor-name');
    const modalAppointmentDate = document.getElementById('modal-appointment-date');
    const cancellationReasonInput = document.getElementById('cancellation-reason');
    
    let currentAppointmentId = null;

    // Randevuları getirip ekrana basan fonksiyon
    const fetchAppointments = async () => {
        const patient = JSON.parse(sessionStorage.getItem('loggedInUser'));
        if (!patient || !patient.id) {
            showMessage(appointmentsMessage, 'Oturum bilgisi eksik. Lütfen tekrar giriş yapın.', 'error');
            return;
        }

        try {
            const response = await fetch(`/Patient/getAppointments?patientId=${patient.id}`);
            if (response.ok) {
                const appointments = await response.json();
                appointmentsList.innerHTML = '';
                if (appointments.length === 0) {
                    const noAppointmentRow = document.createElement('tr');
                    noAppointmentRow.innerHTML = '<td colspan="5">Henüz hiç randevunuz bulunmamaktadır.</td>';
                    appointmentsList.appendChild(noAppointmentRow);
                } else {
                    const now = new Date();
                    appointments.forEach(app => {
                        const row = document.createElement('tr');
                        
                        // Check if the appointment is cancelled
                        const isCancelled = app.isCancelled;
                        const appointmentDate = new Date(app.appointmentStartDate);
                        const isFuture = appointmentDate > now;
                        
                        let actionCellContent = '';
                        if (isCancelled) {
                             actionCellContent = 'İptal Edildi';
                        } else if (isFuture) {
                             actionCellContent = `<button class="btn btn-sm cancel-appointment-btn" data-id="${app.id}" data-doctor="${app.doctorName}" data-date="${appointmentDate.toLocaleDateString()}">İptal Et</button>`;
                        } else {
                             actionCellContent = 'Gerçekleşti';
                        }

                        row.innerHTML = `
                            <td>${app.doctorName}</td>
                            <td>${appointmentDate.toLocaleDateString()}</td>
                            <td>${app.startHour} - ${app.endHour}</td>
                            <td>${app.appointmentTypeName}</td>
                            <td>${actionCellContent}</td>
                        `;
                        appointmentsList.appendChild(row);
                    });
                }
            } else {
                const errorText = await response.text();
                console.error('Randevuları alırken API hatası:', errorText);
                showMessage(appointmentsMessage, 'Randevularınız alınırken bir sorun oluştu.', 'error');
            }
        } catch (error) {
            console.error('Bağlantı hatası:', error);
            showMessage(appointmentsMessage, 'Randevularınız alınırken bir sorun oluştu.', 'error');
        }
    };
    
    // Olay dinleyicisi: Randevu iptal butonlarına tıklama
    appointmentsList.addEventListener('click', (e) => {
        if (e.target.classList.contains('cancel-appointment-btn')) {
            const btn = e.target;
            currentAppointmentId = btn.dataset.id;
            modalDoctorName.textContent = btn.dataset.doctor;
            modalAppointmentDate.textContent = btn.dataset.date;
            cancellationModal.style.display = 'flex';
        }
    });

    // Olay dinleyicisi: Pop-up'ı kapat
    const closeModal = () => {
        cancellationModal.style.display = 'none';
        cancellationReasonInput.value = '';
        currentAppointmentId = null;
    };
    modalCloseBtn.addEventListener('click', closeModal);
    modalVazgecBtn.addEventListener('click', closeModal);
    window.addEventListener('click', (e) => {
        if (e.target === cancellationModal) {
            closeModal();
        }
    });

    // Olay dinleyicisi: İptali onayla
    modalConfirmBtn.addEventListener('click', async () => {
        const reason = cancellationReasonInput.value.trim();
        if (!reason) {
            alert('Lütfen iptal nedeninizi belirtin.');
            return;
        }

        try {
            const response = await fetch('/Patient/cancelAppointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    appointmentId: currentAppointmentId,
                    cancellationReason: reason
                })
            });

            if (response.ok) {
                const result = await response.json();
                showMessage(appointmentsMessage, result.message, 'success');
                closeModal();
                fetchAppointments(); // Listeyi güncelle
            } else {
                const errorText = await response.text();
                console.error('API Hatası:', errorText);
                showMessage(appointmentsMessage, 'Randevu iptal edilirken bir sorun oluştu.', 'error');
                closeModal();
            }
        } catch (error) {
            console.error('Bağlantı hatası:', error);
            showMessage(appointmentsMessage, 'Randevu iptal edilirken bir sorun oluştu.', 'error');
            closeModal();
        }
    });

    createAppointmentForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const doctorId = doctorSelect.value;
        const appointmentTypeId = appointmentTypeSelect.value;
        const appointmentDate = appointmentDateInput.value;
        const startHour = appointmentHourSelect.value;

        if (!doctorId || !appointmentTypeId || !appointmentDate || !startHour) {
            showMessage(appointmentMessage, 'Lütfen tüm alanları doldurunuz.', 'error');
            return;
        }

        const patient = JSON.parse(sessionStorage.getItem('loggedInUser'));
        if (!patient || !patient.id) {
            showMessage(appointmentMessage, 'Oturum bilgisi eksik. Lütfen tekrar giriş yapın.', 'error');
            return;
        }

        const [hour, minute] = startHour.split(':').map(Number);
        const startDateTime = new Date(`${appointmentDate}T${startHour}:00`);
        startDateTime.setMinutes(startDateTime.getMinutes() + 30);
        const endHour = startDateTime.toTimeString().substring(0, 5);
        
        const appointmentData = {
            PatientId: patient.id,
            DoctorId: doctorId,
            AppointmentTypeId: parseInt(appointmentTypeId),
            AppointmentStartDate: `${appointmentDate}T${startHour}:00.0000000`,
            StartHour: startHour,
            AppointmentEndDate: `${appointmentDate}T${endHour}:00.0000000`,
            EndHour: endHour
        };

        try {
            const response = await fetch('/Patient/createAppointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(appointmentData)
            });

            if (response.ok) {
                const result = await response.json();
                showMessage(appointmentMessage, result.message, 'success');
                createAppointmentForm.reset();
                fetchAppointments(); 
            } else {
                const errorText = await response.text();
                console.error('API Hatası:', errorText);
                showMessage(appointmentMessage, 'Randevu oluşturulurken bir sorun oluştu. Detaylar konsolda.', 'error');
            }
        } catch (error) {
            console.error('Hata:', error);
            showMessage(appointmentMessage, 'Bağlantı hatası: Randevu oluşturulamadı.', 'error');
        }
    });

    fetchDoctors();
    fetchAppointmentTypes();
    populateAppointmentHours(); 
    fetchAppointments();
});

const showMessage = (element, message, type) => {
    element.textContent = message;
    element.className = `message-box ${type}`;
    element.style.display = 'block';
    setTimeout(() => {
        element.style.display = 'none';
    }, 5000);
};
