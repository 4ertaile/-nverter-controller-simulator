#include <Arduino.h>
#include <SPI.h>
#include <SD.h>
#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <ArduinoJson.h>
#include <HardwareSerial.h>

// Ініціалізація Wi-Fi налаштувань
const char* ssid = "Your_SSID";
const char* password = "Your_PASSWORD";

// Встановлюємо пін CS для SD-карти
const int chipSelect = 5;
HardwareSerial MySerial(1);  // UART1 на пинах TX (16), RX (17)

// Команди
const byte GET_FULL_DATE = 0x03;  // Команда для отримання всіх даних
const byte GET_BATTERY_VOLTAGE = 0x04;  // Команда для отримання вольтажу батареї

// Ініціалізація сервера
AsyncWebServer server(80);

// Структура для зберігання даних інвертора
struct InverterData {
  float batteryVoltage;
  float gridCurrent;
  float solarVoltage;
  float solarPower;
  float batteryChargeDischargePower;
};

// Глобальні змінні для збереження одиночного значення вольтажу
float batteryVoltage = 0.0;

unsigned long previousMillis = 0;
const long interval = 300000;  // 5 хвилин у мілісекундах (300000 мс)

void setup() {
  Serial.begin(115200);

  // Налаштування серійного порту
  MySerial.begin(9600, SERIAL_8N1, 16, 17);

  // Ініціалізація Wi-Fi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Підключення до Wi-Fi...");
  }
  Serial.println("Wi-Fi підключено.");
  Serial.println(WiFi.localIP());

  // Ініціалізація SD-карти
  if (!SD.begin(chipSelect)) {
    Serial.println("Помилка ініціалізації SD-карти.");
    return;
  }

  // Запуск веб-сервера
  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request){
    request->send(200, "text/plain", "Веб-сервер працює. Викликати команду отримання вольтажу: /get_voltage");
  });

  // Кінцева точка для отримання вольтажу
  server.on("/get_voltage", HTTP_GET, [](AsyncWebServerRequest *request){
    request->send(200, "text/plain", "Отримуємо вольтаж батареї...");
    requestData(GET_BATTERY_VOLTAGE);  // Надсилаємо запит на вольтаж батареї
    delay(500);  // Чекаємо дані від інвертора
    if (MySerial.available() >= sizeof(float)) {
      batteryVoltage = readSingleValue();  // Читаємо вольтаж
      Serial.print("Вольтаж батареї: ");
      Serial.println(batteryVoltage);
    } else {
      Serial.println("Помилка: немає даних від інвертора.");
    }
  });

  server.begin();
}

void loop() {
  // Кожні 5 хвилин надсилаємо команду для отримання всіх даних
  unsigned long currentMillis = millis();
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    requestData(GET_FULL_DATE);  // Надсилаємо запит на отримання всіх даних
    delay(500);  // Чекаємо відповідь

    if (MySerial.available() >= sizeof(InverterData)) {
      InverterData data;
      MySerial.readBytes((char*)&data, sizeof(data));  // Читаємо дані у структуру
      saveDataToJson(data);  // Записуємо дані у JSON
    } else {
      Serial.println("Помилка: немає даних від інвертора.");
    }
  }
}

// Функція для запиту даних у інвертора
void requestData(byte command) {
  MySerial.write(command);  // Надсилаємо команду
  Serial.print("Запит відправлено: ");
  Serial.println(command);
}

// Функція для зчитування одного значення (вольтажу)
float readSingleValue() {
  float value;
  MySerial.readBytes((char*)&value, sizeof(value));  // Читаємо значення
  return value;
}

// Функція для запису даних у форматі JSON на SD-карті
void saveDataToJson(const InverterData& data) {
  File dataFile = SD.open("/inverter_data.json", FILE_WRITE);
  
  if (dataFile) {
    StaticJsonDocument<256> jsonDoc;
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