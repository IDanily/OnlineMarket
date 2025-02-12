﻿(function ($) {
    document.addEventListener("DOMContentLoaded", function () {
        let form = document.querySelector("form");
        if (form) {
            form.addEventListener("submit", function () {
                let productsText = document.getElementById("ProductsList").value;
                let hiddenField = document.getElementById("HiddenProductsList");

                if (hiddenField) {
                    hiddenField.value = productsText;
                }

                console.log("ProductsList:", productsText); // Проверяем, копируется ли текст
            });
        } else {
            console.error("Форма не найдена!");
        }
    });

    $(document).ready(function () {
        // Извлекаем URL из data-атрибута
        var categoryUrl = $('#categoryUrl').data('url');
        // Навигация
        var currentPath = window.location.pathname;
        var links = document.querySelectorAll('.nav a');
        links.forEach(link => link.classList.remove('active'));

        var pageMap = {
            "/Home/MainBase": "home-link",
            "/Product/ProductList": "product-link",
            "/Home/News": "news-link",
            "/Home/Contacts": "contact-link"
        };

        if (pageMap[currentPath]) {
            document.getElementById(pageMap[currentPath]).classList.add('active');
        }

        // Owl Carousel
        $(".owl-carousel").owlCarousel({
            items: 1,
            loop: true,
            autoplay: true,
            autoplayTimeout: 5000,
            autoplayHoverPause: true,
            dots: true
        });

        // Добавление в корзину
        $(".add-to-cart").click(function () {
            var product = {
                id: $(this).data("id"),
                name: $(this).data("name"),
                price: $(this).data("price"),
                quantity: $(this).data("quantity")
            };

            $.post("/Order/AddToOrder", product, function (response) {
                $("#success-banner").fadeIn(500).delay(2000).fadeOut(500, function () {
                    location.reload();
                });
            }).fail(function () {
                alert("Не удалось добавить товар в корзину.");
            });
        });

        // Загрузка категорий
        $.ajax({
            url: categoryUrl,  // Используем извлеченный URL
            type: 'GET',
            dataType: 'json',
            success: function (categories) {
                var categorySelect = $('#Category');
                var selectedCategory = categorySelect.data('selected');

                categorySelect.empty();
                categorySelect.append('<option value="">Выберите категорию</option>');

                categories.forEach(function (category) {
                    var isSelected = category.id == selectedCategory ? 'selected' : '';
                    categorySelect.append('<option value="' + category.id + '" ' + isSelected + '>' + category.name + '</option>');
                });
            },
            error: function () {
                alert('Не удалось загрузить категории');
            }
        });

        // Сброс фильтров
        $('#resetFilters').click(function () {
            $('#Category').val('');
            $('#MinPrice').val('');
            $('#MaxPrice').val('');
            $('#query').val('');
            $('form').submit();
        });

        $('[data-toggle="modal"]').on('click', function (e) {
            var url = $(this).attr('href');
            var modalId = $(this).data('target');

            $.ajax({
                url: url,
                method: 'GET',
                success: function (response) {
                    $(modalId).remove();
                    $("body").append(response);

                    var modalElement = document.getElementById(modalId);
                    if (modalElement) {
                        var modal = new bootstrap.Modal(modalElement);
                        modal.show();
                        modalElement.addEventListener('hidden.bs.modal', function () {
                            location.reload();
                        });
                    } else {
                        console.error("Модальное окно не найдено:", modalId);
                    }
                },
                error: function () {
                    console.error("Ошибка при загрузке данных.");
                }
            });

            e.preventDefault();
        });

        $('#orderSuccessModal').modal('show');

        function toggleNotifications() {
            let list = document.getElementById("notificationList");
            list.classList.toggle("show");

            if (list.classList.contains("show")) {
                fetch("/Notification/GetUserNotifications")
                    .then(response => response.json())
                    .then(data => {
                        let listHTML = data.map(n => `
                    <div class="notification-item ${n.isRead ? '' : 'unread'}">
                        ${n.message}
                    </div>
                `).join("");

                        list.innerHTML = listHTML || "<p>Нет новых уведомлений</p>";
                        document.getElementById("notificationCount").innerText = data.length;
                    });
            }
        }

        function sendNotification(userId, message) {
            fetch("/Notification/SendNotification", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ userId, message })
            }).then(response => {
                if (response.ok) alert("Уведомление отправлено!");
            });
        }
    });

    document.addEventListener("DOMContentLoaded", function () {
        var notificationBell = document.getElementById('notificationBell');
        var notificationDropdown = document.getElementById('notificationDropdown');

        // Обработка клика по колокольчику для открытия/закрытия меню
        //notificationBell.addEventListener('click', function () {
        //    if (notificationDropdown.style.display === 'none' || notificationDropdown.style.display === '') {
        //        notificationDropdown.style.display = 'block'; // Открыть меню
        //    } else {
        //        notificationDropdown.style.display = 'none'; // Закрыть меню
        //    }
        //});

        // Закрытие меню при клике вне его
        window.addEventListener('click', function (e) {
            if (!notificationBell.contains(e.target) && !notificationDropdown.contains(e.target)) {
                notificationDropdown.style.display = 'none';
            }
        });

        notificationBell.addEventListener('click', function () {
            if (notificationDropdown.style.display === 'none' || notificationDropdown.style.display === '') {
                fetch('/Notifications/MarkAsRead', { method: 'POST' })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            document.getElementById('notificationCount').style.display = 'none';

                            const notifications = document.querySelectorAll('.notification-item');
                            notifications.forEach(notification => {
                                notification.classList.remove('unread');
                            });

                            console.log("Уведомления помечены как прочитанные");
                        } else {
                            alert('Произошла ошибка при обновлении уведомлений');
                        }
                    })
                    .catch(error => {
                        console.error('Ошибка при запросе:', error);
                        alert('Не удалось обновить уведомления. Попробуйте снова.');
                    });

                notificationDropdown.style.display = 'block';
            } else {
                notificationDropdown.style.display = 'none';
            }
        });
    });

    document.addEventListener("DOMContentLoaded", function () {
        const generatePdfBtn = document.getElementById("generatePdfBtn");
        if (generatePdfBtn) {
            generatePdfBtn.addEventListener("click", function () {
                const { jsPDF } = window.jspdf;
                const doc = new jsPDF();

                doc.addFont('~/Roboto-Regular.ttf', 'Roboto', 'normal');
                doc.setFont('Roboto');

                doc.text("Сводный отчет по заказам", 20, 20);
                let yPosition = 30;

                const table = document.getElementById("ordersTable");
                const rows = table.querySelectorAll("tbody tr");

                rows.forEach((row, index) => {
                    const cells = row.querySelectorAll("td");
                    const productName = cells[0].innerText.trim();
                    const quantity = cells[1].innerText.trim();
                    const totalAmount = cells[2].innerText.trim();
                    const text = `${index + 1}. Товар: ${productName}, Количество: ${quantity}, Сумма: ${totalAmount}`;

                    doc.text(text, 20, yPosition);
                    yPosition += 10;
                });

                doc.save("order_report.pdf");
            });
        } else {
            console.error("Кнопка для генерации отчета не найдена!");
        }
    });
})(window.jQuery);