#include <ModbusMaster.h>
#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <Preferences.h>
#include <NTPClient.h>
#include <WiFiUdp.h>
#include <SD.h>
#include <SPI.h>
#include <ArduinoJson.h>

ModbusMaster node;
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "time.windows.com", 0, 60000);  // Оновлення кожні 60 секунд

// Параметри WiFi
const char* apSSID = "ESP32-AP";
const char* apPassword = "12345678";
char wifiSSID[32] = "";
char wifiPassword[32] = "";
// Змінна для режиму: 0 — точка доступу, 1 — Wi-Fi
int mode = 0; 

AsyncWebServer server(80);
Preferences preferences;

uint16_t solarGeneration = 0, powerConsumption = 0;
bool isWorking = false, timeSynced = false;
unsigned long previousMillisForModbus = 0;
const long intervalForModbus = 60000;  // Запит кожну хвилину
string SD_Status = "";
string Wifi_status = "";
const int chipSelect = 5;  // Пін CS для SD-карти
String currentFileName = "";

// Змінні для FreeRTOS і індикації діодом
const int LED_PIN = 2;  // Пін для діода
TaskHandle_t ledTaskHandle = NULL;
bool ledStopFlag = false;
const unsigned long blinkDuration = 120000;  // Тривалість миготіння 2 хв



void setup() {
  pinMode(LED_PIN, OUTPUT); // Діод як вихід
  Serial.begin(9600);
  
  // Ініціалізація Modbus
  node.begin(1, Serial);  // ID = 1 (Slave address)
  
  // Завантажуємо налаштування Wi-Fi
  preferences.begin("wifi-settings", false);
  preferences.getString("wifiSSID", wifiSSID, sizeof(wifiSSID));
  preferences.getString("wifiPassword", wifiPassword, sizeof(wifiPassword));
  mode = preferences.getInt("mode", 0);  // За замовчуванням точка доступу
  preferences.end();
  
  if (!SD.begin(chipSelect)) {
    SD_Status = "SD card initialization failed!";
    return;
  }else{
    SD_Status = "SD card initialization fine";
  }



  if (mode == 1) {
    WiFi.mode(WIFI_STA);
    WiFi.begin(wifiSSID, wifiPassword);
    unsigned long startTime = millis();
    while (WiFi.status() != WL_CONNECTED && millis() - startTime < 20000) {
      delay(500);
      Wifi_status = "Connecting to WiFi...";
    }
    
    if (WiFi.status() == WL_CONNECTED) {
      Wifi_status = "Connected to WiFi";
      Serial.println(WiFi.localIP());
    } else {
      Wifi_status = "Failed to connect to WiFi, starting Access Point...";
      startAccessPoint();
    }
  } else {
    startAccessPoint();
  }

  // Запуск веб-сервера
  setupWebServer();

  // Запуск індикації діодом
  ledStopFlag = false;
  xTaskCreate(ledTask, "LED Blink Task", 1024, NULL, 1, &ledTaskHandle);
}



// Функція для зчитування даних Modbus
void readModbusData() {
  uint8_t result;

  result = node.readHoldingRegisters(16, 1);  // Зчитування регістру 16
  if (result == node.ku8MBSuccess) {
    solarGeneration = node.getResponseBuffer(0);
    Serial.print("Solar Generation: ");
    Serial.println(solarGeneration);
  }
  delay(300);
  result = node.readHoldingRegisters(37, 1);  // Зчитування регістру 37
  if (result == node.ku8MBSuccess) {
    powerConsumption = node.getResponseBuffer(0);
    Serial.print("Power Consumption: ");
    Serial.println(powerConsumption);
  }
}
// Синхронізація часу з NTP сервером
void syncTime() {
  Serial.println("Synchronizing time...");
  if (WiFi.status() == WL_CONNECTED) {
    timeClient.update();
    if (timeClient.isTimeSet()) {
      timeSynced = true;
      Serial.println("Time synchronized: " + timeClient.getFormattedTime());
    } else {
      Serial.println("Failed to synchronize time");
    }
  } else {
    Serial.println("No WiFi connection. Cannot synchronize time.");
  }
}

void setupWebServer() {
  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request) {
    String html = "<form action='/save' method='POST'>"
                  "<label>WiFi SSID:</label><input type='text' name='ssid' value='" + String(wifiSSID) + "'><br>"
                  "<label>WiFi Password:</label><input type='password' name='password' value='" + String(wifiPassword) + "'><br>"
                  "<input type='submit' value='Save'></form><br>"
                  "<button onclick=\"window.location.href='/start_ap'\">Start Access Point</button>"
                  "<button onclick=\"window.location.href='/connect_wifi'\">Connect to WiFi</button>"
                  "<h2>Modbus Registers</h2>"
                  "<p>Register 0: " + String(reg0) + "</p>"
                  "<p>Register 1: " + String(reg1) + "</p>"
                  "<p>Register 35: " + String(register35Value) + "</p>";
    request->send(200, "text/html", html);
  });

  server.on("/save", HTTP_POST, [](AsyncWebServerRequest *request) {
    if (request->hasParam("ssid", true) && request->hasParam("password", true)) {
      String ssid = request->getParam("ssid", true)->value();
      String password = request->getParam("password", true)->value();
      ssid.toCharArray(wifiSSID, sizeof(wifiSSID));
      password.toCharArray(wifiPassword, sizeof(wifiPassword));
      
      preferences.begin("wifi-settings", false);
      preferences.putString("wifiSSID", wifiSSID);
      preferences.putString("wifiPassword", wifiPassword);
      preferences.end();

      request->send(200, "text/html", "Settings saved! Restarting...");
      delay(1000);
      ESP.restart();
    } else {
      request->send(400, "text/html", "Missing parameters");
    }
  });

  server.on("/start_ap", HTTP_GET, [](AsyncWebServerRequest *request) {
    preferences.begin("wifi-settings", false);
    preferences.putInt("mode", 0);  // Точка доступу
    preferences.end();
    request->send(200, "text/html", "Access Point mode set. Restarting...");
    delay(1000);
    ESP.restart();
  });

  server.on("/connect_wifi", HTTP_GET, [](AsyncWebServerRequest *request) {
    preferences.begin("wifi-settings", false);
    preferences.putInt("mode", 1);  // Режим Wi-Fi
    preferences.end();
    request->send(200, "text/html", "WiFi mode set. Restarting...");
    delay(1000);
    ESP.restart();
  });

  server.begin();
}

void startAccessPoint() {
  WiFi.mode(WIFI_AP);
  WiFi.softAP(apSSID, apPassword);
  IPAddress IP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(IP);
}

void ledTask(void *parameter) {
  unsigned long startMillis = millis();
  while (!ledStopFlag && millis() - startMillis < blinkDuration) {
    if (mode == 0) {
      digitalWrite(LED_PIN, !digitalRead(LED_PIN));  // Миготіння
      delay(500);
    } else if (WiFi.status() == WL_CONNECTED) {
      digitalWrite(LED_PIN, HIGH);  // Постійне світіння
      delay(500);
    }
  }
  digitalWrite(LED_PIN, LOW);
  ledStopFlag = true;
  vTaskDelete(NULL);
}
// Функція для повернення назви дня тижня
String getDayOfWeek(int dayIndex) {
  switch (dayIndex) {
    case 0: return "Sunday";
    case 1: return "Monday";
    case 2: return "Tuesday";
    case 3: return "Wednesday";
    case 4: return "Thursday";
    case 5: return "Friday";
    case 6: return "Saturday";
    default: return "";
  }
}
void loop() {
  unsigned long currentMillis = millis();

  // Читання регістрів Modbus кожну секунду
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    readRegisters();
  }

  // Зміна значення регістра 35 кожні 5 секунд
  if (currentMillis - previousMillisForRegister35 >= intervalForRegister35) {
    previousMillisForRegister35 = currentMillis;
    incrementRegister35();
  }
}



