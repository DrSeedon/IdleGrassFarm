@echo off
chcp 1251 > nul
echo ============================================
echo Скрипт для добавления файлов в Git
echo Будут добавлены все файлы меньше 100 МБ
echo ============================================
echo.

powershell -Command "git ls-files -m | Where-Object { (Get-Item $_).Length -lt 104857600 } | ForEach-Object { git add $_ }"
powershell -Command "git ls-files --others --exclude-standard | Where-Object { (Get-Item $_).Length -lt 104857600 } | ForEach-Object { git add $_ }"

echo.
echo ============================================
echo Готово! Для проверки введите 'git status'
echo ============================================
echo.
pause 