document.addEventListener('DOMContentLoaded', async () => {
    const appointmentsList = document.getElementById('appointments-list');
    const appointmentsMessage = document.getElementById('appointments-message');
    const cancellationReasonModal = document.getElementById('cancellation-reason-modal');
    const modalReasonText = document.getElementById('modal-reason-text');
    const modalCloseBtn = document.getElementById('modal-close-btn');

    /**
     * Shows a message in the designated message box.
     * @param {HTMLElement} element The message box element.
     * @param {string} message The message text.
     * @param {string} type The type of message ('success', 'error', 'info').
     */
    const showMessage = (element, message, type) => {
        element.textContent = message;
        element.className = `p-4 mb-4 text-sm rounded-lg hidden`;
        if (type === 'success') {
            element.classList.add('bg-green-100', 'text-green-800');
        } else if (type === 'error') {
            element.classList.add('bg-red-100', 'text-red-800');
        } else {
            element.classList.add('bg-blue-100', 'text-blue-800');
        }
        element.style.display = 'block';
        setTimeout(() => {
            element.style.display = 'none';
        }, 5000);
    };

    /**
     * Fetches and displays all appointments for all users.
     */
    const fetchAllAppointments = async () => {
        // Show a loading message
        showMessage(appointmentsMessage, 'Randevular yükleniyor...', 'info');
        appointmentsList.innerHTML = '';

        try {
            // API endpoint to get all appointments
            const response = await fetch('/Admin/getAllAppointments');
            if (response.ok) {
                const appointments = await response.json();
                appointmentsList.innerHTML = ''; // Clear old content

                if (appointments.length === 0) {
                    const noAppointmentRow = document.createElement('tr');
                    noAppointmentRow.innerHTML = `<td colspan="6" class="px-6 py-4 text-center text-gray-500">Henüz hiç randevu bulunmamaktadır.</td>`;
                    appointmentsList.appendChild(noAppointmentRow);
                } else {
                    const now = new Date();
                    appointments.forEach(app => {
                        const row = document.createElement('tr');

                        // Determine appointment status
                        let statusText = 'Yaklaşan';
                        let statusColor = 'text-blue-500';

                        // Check if the appointment date and time have passed
                        const appointmentDateTime = new Date(app.appointmentStartDate + 'T' + app.endHour);

                        if (app.isCancelled) {
                            statusText = 'İptal Edildi';
                            statusColor = 'text-red-500';
                        } else if (app.isAttended) {
                            statusText = 'Gerçekleşti';
                            statusColor = 'text-green-500';
                        } else if (appointmentDateTime < now) {
                            statusText = 'Gerçekleşmedi'; // Geçmiş ama gerçekleşmemiş
                            statusColor = 'text-gray-500';
                        }

                        // Handle cancellation reason
                        let cancellationReasonContent = `<span class="text-gray-500">Yok</span>`;
                        if (app.cancellationReason && app.isCancelled) {
                            cancellationReasonContent = `<button class="text-indigo-600 hover:text-indigo-900 view-reason-btn" data-reason="${app.cancellationReason}">Görüntüle</button>`;
                        }

                        row.innerHTML = `
                                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${app.doctorName || 'Bilinmiyor'}</td>
                                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">${app.patientName || 'Bilinmiyor'}</td>
                                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${new Date(app.appointmentStartDate).toLocaleDateString()}</td>
                                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${app.startHour || 'N/A'} - ${app.endHour || 'N/A'}</td>
                                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium ${statusColor}">
                                        ${statusText}
                                    </td>
                                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                        ${cancellationReasonContent}
                                    </td>
                                `;
                        appointmentsList.appendChild(row);
                    });
                    // Hide message box after successful load
                    appointmentsMessage.style.display = 'none';
                }
            } else {
                const errorText = await response.text();
                console.error('Randevuları alırken API hatası:', errorText);
                showMessage(appointmentsMessage, 'Randevular alınırken bir sorun oluştu.', 'error');
            }
        } catch (error) {
            console.error('Bağlantı hatası:', error);
            showMessage(appointmentsMessage, 'Sunucuya bağlanırken bir sorun oluştu.', 'error');
        }
    };

    // Event listener for the "View Reason" buttons
    appointmentsList.addEventListener('click', (e) => {
        if (e.target.classList.contains('view-reason-btn')) {
            const reason = e.target.dataset.reason;
            modalReasonText.textContent = reason;
            cancellationReasonModal.style.display = 'flex';
        }
    });

    // Event listener to close the modal
    modalCloseBtn.addEventListener('click', () => {
        cancellationReasonModal.style.display = 'none';
    });

    // Close modal when clicking outside of it
    window.addEventListener('click', (e) => {
        if (e.target === cancellationReasonModal) {
            cancellationReasonModal.style.display = 'none';
        }
    });

    // Initial fetch of appointments when the page loads
    fetchAllAppointments();
});