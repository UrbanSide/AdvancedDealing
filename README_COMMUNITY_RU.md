# AdvancedDealing Community 1.5.1

Неофициальный community-maintained fork оригинального AdvancedDealing.

В этой версии объединены:

- совместимость с Schedule I `0.4.5f2` IL2CPP;
- исправление удалённого API `ItemInstance.Category`;
- явная ссылка на `Il2CppScheduleOne.Core.dll`;
- редактируемые переводы `en-US.json` и `ru-RU.json`;
- отдельный тайник для получения товара;
- отдельный тайник для доставки денег;
- автоматическая миграция старого единственного тайника в оба новых назначения;
- штатный ручной сбор денег: если денежный тайник не выбран, выручка остаётся у дилера и забирается обычным способом;
- синхронизация обоих назначений в мультиплеере через существующий `DealerData` payload.

## Совместимость сохранений

Внутреннее имя мода оставлено `AdvancedDealing`, поэтому сохраняются прежние пути:

```text
UserData\AdvancedDealing.cfg
UserData\AdvancedDealing\Localization\
<папка сохранения>\AdvancedDealing.json
```

Старое поле `DeadDrop` остаётся в модели только для чтения старых сохранений. При первой загрузке:

```text
ProductDeadDrop = DeadDrop
CashDeadDrop    = DeadDrop
DeadDrop        = null
```

После следующего обычного сохранения игры файл уже будет содержать два независимых назначения.

## Поведение двух тайников

### Тайник для товара

Дилер идёт к нему, когда количество товара достигает настроенного порога. Значение «Не выбран» отключает автоматический забор товара.

### Тайник для денег

Дилер относит туда выручку после достижения денежного порога. Если тайник не выбран, автоматическая доставка отключается: выручка остаётся у дилера, и игрок забирает её обычным способом.

## Сборка IL2CPP

1. Запусти Schedule I через MelonLoader хотя бы один раз и закрой игру.
2. Открой PowerShell в корне архива.
3. Выполни проверку исходников:

```powershell
powershell -ExecutionPolicy Bypass -File .\verify-hotfix-source.ps1
```

4. Собери и установи DLL:

```powershell
powershell -ExecutionPolicy Bypass -File .\build-il2cpp.ps1 `
  -GamePath "C:\Games\Steam\steamapps\common\Schedule I" `
  -Deploy
```

Результат сборки:

```text
dist\AdvancedDealing.Il2Cpp.dll
```

При `-Deploy` старая DLL резервируется рядом с собой, а новая копируется в:

```text
Schedule I\Mods\AdvancedDealing.Il2Cpp.dll
```

Оригинальную и community DLL одновременно держать нельзя.

## Переводы

Язык задаётся в:

```text
Schedule I\UserData\AdvancedDealing.cfg
```

Пример:

```ini
Language = "ru-RU"
```

Файлы переводов:

```text
Schedule I\UserData\AdvancedDealing\Localization\en-US.json
Schedule I\UserData\AdvancedDealing\Localization\ru-RU.json
```

Пользовательские значения не перезаписываются. При обновлении DLL новые отсутствующие ключи автоматически дописываются из встроенного языка.

## Проверка после установки

1. Назначь «Тайник для товара».
2. Назначь «Тайник для денег». Если оставить «Не выбран», выручка будет храниться у дилера до ручного сбора.
3. Перезапусти игру и убедись, что оба значения сохранились.
4. Дай дилеру закончить товар и проверь забор из продуктового тайника.
5. Дождись денежного порога и проверь выбранный способ доставки денег.
6. Открой `MelonLoader\Latest.log` и убедись, что больше нет:

```text
MissingMethodException: ItemInstance.get_Category()
```

## Происхождение изменений

Оригинальный мод: ManZune / Marcel Hellmund.

Идея и первоначальный вариант разделения тайников: Daniel Brenot (`daniel-brenot`), upstream PR #8.

Совместимость 0.4.5f2, миграция данных, локализация и community-интеграция: UrbanSide / FPZone.

Лицензия MIT сохранена в `LICENSE.txt`, атрибуция — в `NOTICE.md`.
