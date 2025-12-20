# Структура проекта

## Принцип
Feature-first + MVVM: всё, что относится к одной фиче, лежит рядом (View + ViewModel).

## Папки
- `DiaryTaskManagerApp/Core`: доменные модели (без UI и без хранения).
- `DiaryTaskManagerApp/Data`: доступ к данным (репозитории). Сейчас `InMemory`, позже SQLite.
- `DiaryTaskManagerApp/Features`: экраны/фичи.
  - `Features/Tasks`: главный экран со списком, фильтром/сортировкой и нижним вводом.
- `DiaryTaskManagerApp/Resources/Styles`: тема/цвета/общие стили.
- `docs`: документация (план, структура, решения).




