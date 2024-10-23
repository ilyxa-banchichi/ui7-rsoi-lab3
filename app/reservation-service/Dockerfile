# Указываем базовый образ
FROM node:18-alpine

# Устанавливаем рабочую директорию в контейнере
WORKDIR /app

# Копируем файл package.json и package-lock.json для установки зависимостей
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем весь проект в контейнер
COPY . .

# Компилируем TypeScript в JavaScript
RUN npm run build

# Указываем порт, который будет использоваться внутри контейнера
EXPOSE 80

# Команда для запуска сервера
CMD ["npm", "run", "start"]