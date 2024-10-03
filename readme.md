Tool to set map coordinates from .csv file and export bunch of devices on Macroscop and Eocortex servers.
Work with 4.3.70+ versions.

Usage: `.\MacroscopCamMapper input.csv`

`--server, -s <url>` - Server address. Default value is 127.0.0.1.\
`--port, -p <number>` - Server port. Default value is 8080.\
`--ssl` - Connect over HTTPS.\
`--login, -l <string>` - Login. Default value is "root". Must specify `--active-directory` if using a Active Directory user.\
`--active-directory` - Specify that is Active Directory user.\
`--password <string>` - Password. Default value is empty string.\
`--names <string>` - Column header contains names of cameras. Default value is "Имя камеры".\
`--channel-id <string>` - Column header contains ids of channels. Default value is "Channel Id".\
`--latitude <string>` - Column header contains latitude. Default value is "Широта".\
`--longitude <string>` - Column header contains longitude. Default value is "Долгота".\
`--on-map <string>` - Column header sets IsOnMap flag. Default value is "Размещена на карте". Valid values: [true, yes, да]; in any letter case. Any other values either lack of value automatically set IsOnMap flag to false.\
`--delimeter <string>` - Columns delimeter. It depends on selected culture.\
`--encoding <string>` - File encoding. Default value is UTF-8. To see all possible encodings specify `--show-encodings`.\
`--culture <string>` - File culture. Default value is InvariantCulture. To see all possible cultures specify `--show-cultures`.\
`--show-encodings` - Show all possible encodings and exit.\
`--show-cultures` - Show all possible cultures and exit.\
`--verbose, -v` - Show verbose output.\
`--export <path>` - Export to file and exit. Overwrite file if it exists.\
`--help, -h, -?` - Show this message and exit.

---

Утилита для пакетной настройки координат устройств из .csv файла и экспорта устройств на серверах Macroscop и Eocortex. Работает с версиями 4.3.70+.

Использование: `.\MacroscopCamMapper input.csv`

`--server, -s <url>` - Адрес сервера. Значение по умолчанию 127.0.0.1.\
`--port, -p <number>` - Порт сервера. Значение по умолчанию 8080.\
`--ssl` - Подключение по HTTPS.\
`--login, -l <string>` - Логин. Значение по умолчанию "root". Следует указать `--active-directory` если используется учётная запись Active Directory.\
`--active-directory` - Указывает, что используется учётная запись Active Directory.\
`--password <string>` - Пароль. Значение по умолчанию - пустая строка.\
`--names <string>` - Заголовок столбца, который содержит имена камер. Значение по умолчанию "Имя камеры".\
`--channel-id <string>` - Заголовок столбца, который содержит channelId. Значение по умолчанию "Channel Id".\
`--latitude <string>` - Заголовок столбца, который содержит широту. Значение по умолчанию "Широта".\
`--longitude <string>` - Заголовок столбца, который содержит долготу. Значение по умолчанию "Долгота".\
`--on-map <string>` - Заголовок столбца, задающий значение флагу IsOnMap. Значение по умолчанию "Размещена на карте". Валидные значения: [true, yes, да]; в любом регистре. Любые другие значения, равно как и его отсутствие,  автоматически установит флаг IsOnMap в значение false.\
`--delimeter <string>` - Разделитель столбцов. Зависит от выбранной культуры.\
`--encoding <string>` - Кодировка файла. Значение по умолчанию UTF-8. Для просмотра перечня доступных кодировок укажите `--show-encodings`.\
`--culture <string>` - Культура файла. Значение по умолчанию InvariantCulture. Для просмотра перечня доступных культур укажите `--show-cultures`.\
`--show-encodings` - Выводит перечень всех доступных кодировок и завершает работу.\
`--show-cultures` - Выводит перечень всех доступных культур и завершает работу.\
`--verbose, -v` - Выводит подробную информацию о результате исполнения запроса на изменение каналов.\
`--export <path>` - Экспортирует каналы в файл и завершает работу. Перезаписывает существующий файл.\
`--help, -h, -?` - Выводит это сообщение и завершает работу.
