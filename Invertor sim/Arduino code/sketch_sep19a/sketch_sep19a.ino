#include <Arduino.h>
#include <SPI.h>
#include <SD.h>
#include <WiFi.h>
#include <WebServer.h>  // Стандартна бібліотека для ESP32
#include <ArduinoJson.h>
#include <HardwareSerial.h>

// Налаштування для режиму точки доступу (Access Point)
const char* ap_ssid = "ESP32_Access_Point";
const char* ap_password = "12345678";  // Пароль для підключення до точки доступу

// Встановлюємо пін CS для SD-карти
const int chipSelect = 5;
HardwareSerial MySerial(1);  // UART1 на пинах TX (16), RX (17)

// Команди
const byte GET_FULL_DATE = 0x03;  // Команда для отримання всіх даних
const byte GET_BATTERY_VOLTAGE = 0x04;  // Команда для отримання вольтажу батареї

// Ініціалізація сервера
WebServer server(80);

// Структура для зберігання даних інвертора
struct InverterData {
  float batteryVoltage;
  float gridCurrent;
  float solarVoltage;
  float solarPower;
  float batteryChargeDischargePower;
};

// Глобальні змінні для збереження даних
InverterData lastData;  // Для зберігання останнього отриманого набору даних

unsigned long previousMillis = 0;
const long interval = 25000;  // 25 секунд у мілісекундах (25000 мс)

void setup() {
  Serial.begin(115200);
  Serial.println("Запуск ESP32...");

  // Налаштування серійного порту
  MySerial.begin(9600, SERIAL_8N1, 16, 17);
  Serial.println("Серійний порт ініціалізовано.");

  // Ініціалізація SD-карти
  if (!SD.begin(chipSelect)) {
    Serial.println("Помилка ініціалізації SD-карти.");
    return;
  }
  Serial.println("SD-карта успішно ініціалізована.");

  // Налаштування ESP32 як точки доступу
  WiFi.softAP(ap_ssid, ap_password);  // Запускаємо Wi-Fi у режимі точки доступу
  IPAddress IP = WiFi.softAPIP();
  Serial.print("IP адреса точки доступу: ");
  Serial.println(IP);

  // Запуск веб-сервера
  server.on("/", HTTP_GET, [](){
    String html = "<!DOCTYPE html>";
    html += "<html lang='uk'>";
    html += "<head>";
    html += "<meta charset='UTF-8'>";
    html += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
    html += "<title>ESP32 Веб-сервер</title>";
    html += "<style>body { font-family: Arial, sans-serif; } h1 { color: #333; }</style>";
    html += "</head>";
    html += "<body>";
    html += "<h1>Привіт, світ!</h1>";
    html += "<p>Це простий веб-сервер на ESP32.</p>";
    html += "<p><a href='/get_battery_voltage'>Отримати вольтаж батареї</a></p>";
    html += "<p><a href='/get_full_data'>Отримати повні дані</a></p>";
    html += "</body>";
    html += "</html>";
    server.send(200, "text/html; charset=utf-8", html);
  });

  // Кінцева точка для отримання повного набору даних через веб-сервер
  server.on("/get_full_data", HTTP_GET, [](){
    Serial.println("Обробка запиту на отримання повного набору даних.");

    // Надсилаємо запит на отримання даних з інвертора
    requestData(GET_FULL_DATE);

    // Чекаємо на відповідь від інвертора
    delay(500);  // Чекаємо 500 мс

    // Читаємо дані
    if (MySerial.available() >= sizeof(InverterData)) {
      MySerial.readBytes((char*)&lastData, sizeof(lastData));
      // Формуємо HTML відповідь
      String html = "<!DOCTYPE html>";
      html += "<html lang='uk'>";
      html += "<head>";
      html += "<meta charset='UTF-8'>";
      html += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
      html += "<title>Повні дані</title>";
      html += "<style>body { font-family: Arial, sans-serif; } h1 { color: #333; }</style>";
      html += "</head>";
      html += "<body>";
      html += "<h1>Повний набір даних</h1>";
      html += "<p>Вольтаж батареї: " + String(lastData.batteryVoltage) + " V</p>";
      html += "<p>Сила струму в мережі: " + String(lastData.gridCurrent) + " A</p>";
      html += "<p>Вольтаж сонячної батареї: " + String(lastData.solarVoltage) + " V</p>";
      html += "<p>Сонячна потужність: " + String(lastData.solarPower) + " W</p>";
      html += "<p>Потужність заряду/розряду батареї: " + String(lastData.batteryChargeDischargePower) + " W</p>";
      html += "<a href='/'>Повернутися на головну</a>";
      html += "</body>";
      html += "</html>";
      server.send(200, "text/html; charset=utf-8", html);
    } else {
      // Відповідь у разі, якщо немає даних
      String errorHtml = "<!DOCTYPE html>";
      errorHtml += "<html lang='uk'>";
      errorHtml += "<head>";
      errorHtml += "<meta charset='UTF-8'>";
      errorHtml += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
      errorHtml += "<title>Помилка</title>";
      errorHtml += "<style>body { font-family: Arial, sans-serif; } h1 { color: red; }</style>";
      errorHtml += "</head>";
      errorHtml += "<body>";
      errorHtml += "<h1>Помилка</h1>";
      errorHtml += "<p>Не вдалося отримати дані від інвертора. Перевірте підключення та спробуйте знову.</p>";
      errorHtml += "<a href='/'>Повернутися на головну</a>";
      errorHtml += "</body>";
      errorHtml += "</html>";
      server.send(500, "text/html; charset=utf-8", errorHtml);
    }
  });

  // Кінцева точка для отримання вольтажу батареї через веб-сервер
  server.on("/get_battery_voltage", HTTP_GET, [](){
    Serial.println("Обробка запиту на отримання вольтажу батареї.");

    // Надсилаємо запит на отримання вольтажу батареї
    requestData(GET_BATTERY_VOLTAGE);

    // Чекаємо на відповідь від інвертора
    delay(500);  // Чекаємо 500 мс

    // Читаємо дані
    if (MySerial.available() >= sizeof(float)) {
      float batteryVoltage;
      MySerial.readBytes((char*)&batteryVoltage, sizeof(batteryVoltage));
      
      // Формуємо HTML відповідь
      String html = "<!DOCTYPE html>";
      html += "<html lang='uk'>";
      html += "<head>";
      html += "<meta charset='UTF-8'>";
      html += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
      html += "<title>Вольтаж батареї</title>";
      html += "<style>body { font-family: Arial, sans-serif; } h1 { color: #333; }</style>";
      html += "</head>";
      html += "<body>";
      html += "<h1>Вольтаж батареї</h1>";
      html += "<p>Вольтаж батареї: " + String(batteryVoltage) + " V</p>";
      html += "<a href='/'>Повернутися на головну</a>";
      html += "</body>";
      html += "</html>";
      server.send(200, "text/html; charset=utf-8", html);
    } else {
      // Відповідь у разі, якщо немає даних
      String errorHtml = "<!DOCTYPE html>";
      errorHtml += "<html lang='uk'>";
      errorHtml += "<head>";
      errorHtml += "<meta charset='UTF-8'>";
      errorHtml += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
      errorHtml += "<title>Помилка</title>";
      errorHtml += "<style>body { font-family: Arial, sans-serif; } h1 { color: red; }</style>";
      errorHtml += "</head>";
      errorHtml += "<body>";
      errorHtml += "<h1>Помилка</h1>";
      errorHtml += "<p>Не вдалося отримати вольтаж батареї. Перевірте підключення та спробуйте знову.</p>";
      errorHtml += "<a href='/'>Повернутися на головну</a>";
      errorHtml += "</body>";
      errorHtml += "</html>";
      server.send(500, "text/html; charset=utf-8", errorHtml);
    }
  });

  server.begin();
  Serial.println("Веб-сервер запущено.");
}

void loop() {
  // Кожні 25 секунд надсилаємо команду для отримання всіх даних
  unsigned long currentMillis = millis();
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    Serial.println("Надсилаємо запит на отримання всіх даних.");
    requestData(GET_FULL_DATE);  // Надсилаємо запит на отримання всіх даних
    delay(500);  // Чекаємо відповідь

    if (MySerial.available() >= sizeof(InverterData)) {
      MySerial.readBytes((char*)&lastData, sizeof(lastData));  // Читаємо дані у структуру
      Serial.println("Дані отримані.");
      saveDataToJson(lastData);  // Записуємо дані у JSON (опціонально, якщо вам потрібно записувати в файл)
    } else {
      Serial.println("Помилка: немає даних від інвертора.");
    }
  }

  server.handleClient();  // Обробка запитів веб-сервера
}

// Функція для запиту даних у інвертора
void requestData(byte command) {
  MySerial.write(command);  // Надсилаємо команду
  Serial.print("Запит відправлено: ");
  Serial.println(command);
}

// Функція для запису даних у форматі JSON на SD-карті (опціонально)
void saveDataToJson(const InverterData& data) {
  File dataFile = SD.open("/inverter_data.json", FILE_WRITE);
  
  if (dataFile) {
    StaticJsonDocument<128> jsonDoc;  // Зменшено розмір документа
    jsonDoc["batteryVoltage"] = data.batteryVoltage;
    jsonDoc["gridCurrent"] = data.gridCurrent;
    jsonDoc["solarVoltage"] = data.solarVoltage;
    jsonDoc["solarPower"] = data.solarPower;
    jsonDoc["batteryChargeDischargePower"] = data.batteryChargeDischargePower;

    if (dataFile.size() > 0) {
      dataFile.print(",");  // Додаємо кому для продовження масиву
    }
    serializeJson(jsonDoc, dataFile);
    
    dataFile.close();
    Serial.println("Дані записано у файл JSON.");
  } else {
    Serial.println("Помилка відкриття файлу для запису.");
  }
}


