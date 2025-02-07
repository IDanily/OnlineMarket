(function ($) {
    //// Создаем элемент <script>
    //var script = document.createElement('script');
    //script.src = 'https://yastatic.net/s3/passport-sdk/autofill/v1/sdk-suggest-with-polyfills-latest.js';
    //script.type = 'text/javascript';

    //// Ожидаем загрузки скрипта перед его использованием
    //script.onload = function () {
    //    YaAuthSuggest.init({
    //        client_id: '818138e68e80463b8cbd7f0defa29be8',  // Ваш client_id
    //        response_type: 'token',
    //        redirect_uri: 'https://magazin.somee.com/YandexTokenRedirect' // Вспомогательная страница
    //    }, 'https://magazin.somee.com')
    //        .then(({ handler }) => {
    //            document.getElementById('yandexAuthButton').addEventListener('click', function () {
    //                handler();
    //            });
    //        })
    //        .catch(error => console.log('Ошибка инициализации Яндекс авторизации', error));
    //};

    document.getElementById('yandexAuthButton').addEventListener('click', function () {
        const authWindow = window.open('https://oauth.yandex.ru/authorize?response_type=token&client_id=818138e68e80463b8cbd7f0defa29be8&redirect_uri=https://magazin.somee.com/YandexTokenRedirect',
            'yandexAuth',
            'width=600,height=600'
        );

        window.addEventListener("message", function (event) {
            if (event.data.access_token) {
                // Отправляем токен на сервер
                fetch(`/Authorization/AuthenticateWithYandex?access_token=${event.data.access_token}`)
                    .then(response => {
                        if (response.ok) {
                            window.location.reload(); // Обновляем страницу после успешной авторизации
                        }
                    })
                    .catch(error => console.error("Ошибка при отправке токена", error));
            }
        });
    });


    // Добавляем скрипт в head
    /*document.head.appendChild(script);*/
})(window.jQuery);
