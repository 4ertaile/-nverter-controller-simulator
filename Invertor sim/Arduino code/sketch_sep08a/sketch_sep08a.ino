#include <ModbusMaster.h>
#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <Preferences.h>
#include <NTPClient.h>
#include <WiFiUdp.h>
#include <SD.h>
#include <SPI.h>
#include <ArduinoJson.h>
#include <vector>     // Для роботи з масивами
#include <TimeLib.h>  // Для роботи з часом


ModbusMaster node;
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "time.windows.com", 10800, 60000);  // Оновлення кожні 60 секунд

// Параметри WiFi
const char *apSSID = "ESP32-AP";
const char *apPassword = "12345678";
char wifiSSID[32] = "";
char wifiPassword[32] = "";
// Змінна для режиму: 0 — точка доступу, 1 — Wi-Fi
int mode = 0;

AsyncWebServer server(80);
Preferences preferences;

float solarGeneration = 0, powerConsumption = 0;
bool isWorking = false, timeSynced = false;
unsigned long previousMillisForModbus = 0;
const long intervalForModbus = 60000;  // Запит кожну хвилину
String SD_Status = "";
String Wifi_status = "";
const int chipSelect = 5;  // Пін CS для SD-карти
String currentFileName = "";
String fileStatus = "";

// Змінні для FreeRTOS і індикації діодом
const int LED_PIN = 2;  // Пін для діода
TaskHandle_t ledTaskHandle = NULL;
bool ledStopFlag = false;
const unsigned long blinkDuration = 120000;  // Тривалість миготіння 2 хв
String fileParseStatus = "";


void setup() {
  pinMode(LED_PIN, OUTPUT);  // Діод як вихід
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
  } else {
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
      syncTime();
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

// Допоміжна функція для перетворення рядка "dd-mm-yyyy" на структуру часу tmElements_t
tmElements_t parseDate(String dateStr) {
  tmElements_t tm;
  int day = dateStr.substring(0, 2).toInt();
  int month = dateStr.substring(3, 5).toInt();
  int year = dateStr.substring(6, 10).toInt();
  tm.Day = day;
  tm.Month = month;
  tm.Year = CalendarYrToTm(year);
  return tm;
}

// Функція для отримання дня тижня за датою
String getDayOfWeekFromFileName(String fileName) {
  // Очікується, що формат файлу "dd-mm-yyyy.json"
  if (fileName.length() < 14) {
    return "";
  }
  String dateStr = fileName.substring(0, 10);  // Витягуємо "dd-mm-yyyy"
  tmElements_t tm = parseDate(dateStr);
  time_t t = makeTime(tm);
  int dayOfWeek = weekday(t);  // Отримуємо день тижня (1 — неділя, 2 — понеділок, ...)

  return getDayOfWeek(dayOfWeek);
}
std::vector<String> findFilesForLast25Days() {
    std::vector<String> files;
    int daysToCheck = 25;  // Кількість днів для перевірки
    time_t currentTime = now();  // Отримуємо поточний час

    // Отримуємо поточний місяць і рік
    String currentMonth = getFormattedDate().substring(5, 7);
    String currentYear = getFormattedDate().substring(0, 4);
    int currentDay = getFormattedDate().substring(8, 10).toInt();

    // Перевіряємо поточний місяць
    String folderName = "/" + currentMonth + "_" + currentYear;
    if (SD.exists(folderName)) {
        File dir = SD.open(folderName);
        while (true) {
            File entry = dir.openNextFile();
            if (!entry) break;

            String fileName = entry.name();
            tmElements_t fileDate = parseDate(fileName.substring(0, 10));
            time_t fileTime = makeTime(fileDate);
            int dayDiff = (currentTime - fileTime) / SECS_PER_DAY;

            // Перевіряємо, чи файл містить дані за останні 25 днів
            if (dayDiff > 0 && dayDiff <= daysToCheck) {
                files.push_back(folderName + "/" + fileName);
            }
            entry.close();
        }
        dir.close();
    }

    // Якщо у поточному місяці не вистачає днів, переходимо до попереднього
    int remainingDays = daysToCheck - files.size();
    if (remainingDays > 0) {
        int prevMonth = currentMonth.toInt() - 1;
        int prevYear = currentYear.toInt();

        // Якщо це січень, переходимо на попередній рік
        if (prevMonth == 0) {
            prevMonth = 12;
            prevYear -= 1;
        }

        // Формуємо назву попередньої папки
        String prevFolderName = "/" + (prevMonth < 10 ? "0" + String(prevMonth) : String(prevMonth)) + "_" + String(prevYear);

        if (SD.exists(prevFolderName)) {
            File dir = SD.open(prevFolderName);
            while (true) {
                File entry = dir.openNextFile();
                if (!entry) break;

                String fileName = entry.name();
                tmElements_t fileDate = parseDate(fileName.substring(0, 10));
                time_t fileTime = makeTime(fileDate);
                int dayDiff = (currentTime - fileTime) / SECS_PER_DAY;

                // Перевіряємо, чи файл містить дані за останні 25 днів і чи не старший він
                if (dayDiff > 0 && dayDiff <= daysToCheck) {
                    files.push_back(prevFolderName + "/" + fileName);
                }
                entry.close();
            }
            dir.close();
        }
    }

    return files;
}


// Функція для пошуку перших 4 файлів за днями тижня, в межах 25 днів
std::vector<String> findFilesByDayOfWeek(String dayOfWeek) {
    std::vector<String> files;
    int daysToCheck = 25;  // Максимум 25 днів для перевірки

    // Перший пошук у поточному місяці
    String currentMonth = getFormattedDate().substring(5, 7);    // Отримуємо поточний місяць (MM)
    String currentYear = getFormattedDate().substring(0, 4);     // Отримуємо поточний рік (YYYY)
    String folderName = "/" + currentMonth + "_" + currentYear;  // Папка формату mm_yyyy
    Serial.print(folderName);
    if (SD.exists(folderName)) {
        File dir = SD.open(folderName);
        while (true) {
            File entry = dir.openNextFile();
            if (!entry) break;

            String fileName = entry.name();
            Serial.print(fileName);
            Serial.print(getDayOfWeekFromFileName(fileName));
            if (getDayOfWeekFromFileName(fileName) == dayOfWeek) {
                files.push_back(folderName + "/" + fileName);
            }
            entry.close();

            if (files.size() >= 4) break;
        }
        dir.close();
    }

    // Якщо не вистачає файлів у поточному місяці, перевіряємо попередній місяць
    if (files.size() < 4) {
        int prevMonth = currentMonth.toInt() - 1;
        int prevYear = currentYear.toInt();

        // Якщо це січень, переходимо на попередній рік
        if (prevMonth == 0) {
            prevMonth = 12;
            prevYear -= 1;
        }

        String prevFolderName = "/" + (prevMonth < 10 ? "0" + String(prevMonth) : String(prevMonth)) + "_" + String(prevYear);

        if (SD.exists(prevFolderName)) {
            File dir = SD.open(prevFolderName);
            while (true) {
                File entry = dir.openNextFile();
                if (!entry) break;

                String fileName = entry.name();
                if (getDayOfWeekFromFileName(fileName) == dayOfWeek) {
                    // Перевіряємо, чи не старіший цей файл більше ніж на 25 днів
                    tmElements_t fileDate = parseDate(fileName.substring(0, 10));
                    time_t fileTime = makeTime(fileDate);
                    time_t currentTime = now();
                    int dayDiff = (currentTime - fileTime) / SECS_PER_DAY;
                    if (dayDiff <= 25) {
                        files.push_back(prevFolderName + "/" + fileName);
                    }
                }
                entry.close();

                if (files.size() >= 4) break;
            }
            dir.close();
        }
    }

    return files;
}


// Функція для зчитування даних Modbus
void readModbusData() {
  uint8_t result;

  result = node.readHoldingRegisters(16, 1);  // Зчитування регістру 16
  if (result == node.ku8MBSuccess) {
    solarGeneration = node.getResponseBuffer(0);
  }

  delay(300);


  result = node.readHoldingRegisters(37, 1);  // Зчитування регістру 37
  if (result == node.ku8MBSuccess) {
    powerConsumption = node.getResponseBuffer(0);
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
// Функція для конвертації UNIX-часу в дату (yyyy-mm-dd)
String getFormattedDate() {
  time_t rawTime = timeClient.getEpochTime();  // Отримуємо UNIX-час
  struct tm *timeInfo = localtime(&rawTime);   // Конвертуємо в структуру часу

  // Формуємо рядок у форматі yyyy-mm-dd
  char buffer[11];
  snprintf(buffer, sizeof(buffer), "%04d-%02d-%02d", timeInfo->tm_year + 1900, timeInfo->tm_mon + 1, timeInfo->tm_mday);
  return String(buffer);
}

// Запис даних у JSON на SD-карту
void writeDataToSD(String requestTime) {
  currentFileName = "";
  fileStatus = "";
  if (!SD.begin(chipSelect)) {
    SD_Status = "SD card initialization failed!";
    return;
  } else {
    SD_Status = "SD card initialization fine";
  }
  // Оновлюємо назву файлу відповідно до дати
  String date = getFormattedDate();                                                                             // Отримуємо дату у форматі yyyy-mm-dd
  String folderName = "/" + date.substring(5, 7) + "_" + date.substring(0, 4);                                  // mm_yyyy
  String fileName = date.substring(8, 10) + "-" + date.substring(5, 7) + "-" + date.substring(0, 4) + ".json";  // dd-mm-yyyy.json

  if (!SD.exists(folderName)) {
    if (SD.mkdir(folderName)) {
      fileParseStatus = "Folder created successfully";
    } else {
      fileParseStatus = "Failed to create folder: " + folderName;
    }
  }

  currentFileName = folderName + "/" + fileName;

  // Перевіряємо існування файлу і створюємо його, якщо його нема
  if (!SD.exists(currentFileName)) {
    File file = SD.open(currentFileName, FILE_WRITE);
    if (file) {
      file.print("[");  // Створюємо новий JSON масив
      file.close();
    }
  }

  // Відкриваємо файл для читання
  File file = SD.open(currentFileName, FILE_READ);
  if (!file) {
    fileStatus = "Failed to open file for reading";
    return;
  }

  // Читаємо існуючий вміст файлу
  String fileContent = "";
  while (file.available()) {
    fileContent += char(file.read());
  }
  file.close();

  // Парсимо JSON з файлу
  DynamicJsonDocument doc(1024);
  DeserializationError error = deserializeJson(doc, fileContent);
  if (error) {
    // Якщо не вдалося зчитати дані, вважаємо, що файл пустий
    fileParseStatus = "Failed to parse JSON. Treating file as empty.";
    doc.clear();  // Очищаємо документ для подальшого використання
  }

  JsonArray dataArray;
  if (doc.is<JsonArray>()) {
    dataArray = doc.as<JsonArray>();
  } else {
    dataArray = doc.to<JsonArray>();
  }

  // Перевіряємо наявність запису з таким же значенням часу
  bool recordFound = false;
  for (JsonObject record : dataArray) {
    if (record["Time"] == requestTime) {
      // Оновлюємо існуючий запис
      record["PowerConsumption"] = powerConsumption;
      record["solarGeneration"] = solarGeneration;
      recordFound = true;
      break;
    }
  }

  // Якщо запис не знайдено, додаємо новий
  if (!recordFound) {
    JsonObject newRecord = dataArray.createNestedObject();
    newRecord["Time"] = requestTime;
    newRecord["PowerConsumption"] = powerConsumption;
    newRecord["SolarGeneration"] = solarGeneration;
  }

  // Записуємо оновлений JSON у файл
  file = SD.open(currentFileName, FILE_WRITE);
  if (!file) {
    fileStatus = "Failed to open file for writing";
    return;
  }

  file.seek(0);  // Повертаємось на початок файлу для перезапису
  serializeJson(dataArray, file);
  file.print("]");  // Закриваємо JSON масив
  file.close();

  fileStatus = "Data written to SD card";
}


void setupWebServer() {
  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request) {
    String html = "<form action='/save' method='POST'>"
                  "<label>WiFi SSID:</label><input type='text' name='ssid' value='"
                  + String(wifiSSID) + "'><br>"
                                       "<label>WiFi Password:</label><input type='password' name='password' value='"
                  + String(wifiPassword) + "'><br>"
                                           "<input type='submit' value='Save'></form><br>"
                                           "<button onclick=\"window.location.href='/start_ap'\">Start Access Point</button><br>"
                                           "<button onclick=\"window.location.href='/connect_wifi'\">Connect to WiFi</button><br>"
                                           "<button onclick=\"window.location.href='/sync_time'\">Synchronize Time</button><br>"
                                           "<button onclick=\"window.location.href='/start_work'\">Start Work</button><br>"
                                           "<button onclick=\"window.location.href='/stop_work'\">Stop Work</button><br>"
                                           "<button onclick=\"window.location.href='/getFiles'\">getFiles</button><br>"
                                           "<label>SD Status: "
                  + String(SD_Status) + "</label><br>"
                  "<label>SD isWorking: "
                  + String(isWorking) + "</label><br>"
                                        "<label>WiFi status: "
                  + String(Wifi_status) + "</label><br>"
                                          "<label>Current FileName: "
                  + String(currentFileName) + "</label><br>"
                                              "<label>Current fileStatus: "
                  + String(fileStatus) + "</label><br>"
                                         "<label>fileParseStatus: "
                  + String(fileParseStatus) + "</label><br>"
                                              "<p>Current Time: "
                  + getFormattedDate() + " " + timeClient.getFormattedTime() + "</p>"
                                                                               "<button onclick=\"window.location.href='/status'\">View SD Card and Data</button><br>";
    request->send(200, "text/html", html);
  });

  server.on("/getFiles", HTTP_GET, [](AsyncWebServerRequest *request) {
    String html = "<html><body>";
    html += "<h1>ESP32 SD Card Status</h1>";
    html += "<p>SD Card Status: " + SD_Status + "</p>";

    // Додано: пошук файлів за неділю
    std::vector<String> sundayFiles = findFilesByDayOfWeek("Sunday");
    html += "<h2>Files for Sunday:</h2><ul>";
    for (const auto &file : sundayFiles) {
      html += "<li>" + file + "</li>";
    }
    html += "</ul>";

    // Інші елементи сторінки
    html += "<p>WiFi Status: " + Wifi_status + "</p>";
    html += "<p><a href=\"/sync\">Synchronize Time</a></p>";
    html += "<p><a href=\"/start\">Start Work</a></p>";
    html += "<p><a href=\"/stop\">Stop Work</a></p>";
    html += "</body></html>";

    request->send(200, "text/html", html);
  });
  // Додавання маршруту для перегляду статусу SD-карти та поточних даних
  server.on("/status", HTTP_GET, [](AsyncWebServerRequest *request) {
    String html = "<h2>Current Data</h2>";
    html += "<p>Solar Generation: " + String(solarGeneration) + " W</p>";
    html += "<p>Power Consumption: " + String(powerConsumption) + " W</p>";

    html += "<h2>SD Card Content</h2>";
    if (!SD.begin(chipSelect)) {
      html += "<p>SD card not initialized.</p>";
    } else {
      File root = SD.open("/");
      if (!root) {
        html += "<p>Failed to open SD card root directory.</p>";
      } else {
        html += "<ul>";
        while (true) {
          File entry = root.openNextFile();
          if (!entry) {
            // Більше немає файлів
            break;
          }
          html += "<li>" + String(entry.name()) + " (" + entry.size() + " bytes)</li>";
          entry.close();
        }
        html += "</ul>";
      }
      root.close();
    }

    request->send(200, "text/html", html);
  });

  // Інші наявні маршрути
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

  server.on("/start_work", HTTP_GET, [](AsyncWebServerRequest *request) {
    isWorking = true;
    request->send(200, "text/html", "Started Modbus reading and data logging.");
  });

  server.on("/stop_work", HTTP_GET, [](AsyncWebServerRequest *request) {
    isWorking = false;
    request->send(200, "text/html", "Stopped Modbus reading and data logging.");
  });

  server.on("/sync_time", HTTP_GET, [](AsyncWebServerRequest *request) {
    syncTime();
    request->send(200, "text/html", "Time synchronization attempted.");
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
    case 1: return "Sunday";
    case 2: return "Monday";
    case 3: return "Tuesday";
    case 4: return "Wednesday";
    case 5: return "Thursday";
    case 6: return "Friday";
    case 7: return "Saturday";
    default: return "";
  }
}
void loop() {

  // Перевіряємо час для зчитування Modbus даних
  if (isWorking && timeSynced && timeClient.getSeconds() == 0) {
    int hours = timeClient.getHours();
    int minutes = timeClient.getMinutes();
    String time = String(hours < 10 ? "0" : "") + String(hours) + ":" + String(minutes < 10 ? "0" : "") + String(minutes) + ":" + "00";
    readModbusData();
    writeDataToSD(time);
  }
}





//////////

// #include <ModbusMaster.h>
// #include <WiFi.h>
// #include <ESPAsyncWebServer.h>
// #include <Preferences.h>

// ModbusMaster node;

// // Параметри WiFi
// const char* apSSID = "ESP32-AP";
// const char* apPassword = "12345678";
// char wifiSSID[32] = "";
// char wifiPassword[32] = "";
// // Змінна для режиму: 0 — точка доступу, 1 — Wi-Fi
// int mode = 0;

// AsyncWebServer server(80);
// Preferences preferences;

// // Змінні для Modbus регістрів
// uint16_t reg0, reg1;
// float register35Value = 0.0;
// unsigned long previousMillis = 0;
// unsigned long previousMillisForRegister35 = 0;
// const long interval = 1000;  // Оновлення кожну секунду
// const long intervalForRegister35 = 5000;  // Оновлення кожні 5 секунд

// // Змінні для FreeRTOS і індикації діодом
// const int LED_PIN = 2;  // Пін для діода
// TaskHandle_t ledTaskHandle = NULL;
// bool ledStopFlag = false;
// const unsigned long blinkDuration = 120000;  // Тривалість миготіння 2 хв

// void setup() {
//   pinMode(LED_PIN, OUTPUT); // Діод як вихід
//   Serial.begin(9600);

//   // Ініціалізація Modbus
//   node.begin(1, Serial);  // ID = 1 (Slave address)

//   // Завантажуємо налаштування Wi-Fi
//   preferences.begin("wifi-settings", false);
//   preferences.getString("wifiSSID", wifiSSID, sizeof(wifiSSID));
//   preferences.getString("wifiPassword", wifiPassword, sizeof(wifiPassword));
//   mode = preferences.getInt("mode", 0);  // За замовчуванням точка доступу
//   preferences.end();

//   if (mode == 1) {
//     WiFi.mode(WIFI_STA);
//     WiFi.begin(wifiSSID, wifiPassword);
//     unsigned long startTime = millis();
//     while (WiFi.status() != WL_CONNECTED && millis() - startTime < 20000) {
//       delay(500);
//       Serial.println("Connecting to WiFi...");
//     }

//     if (WiFi.status() == WL_CONNECTED) {
//       Serial.println("Connected to WiFi");
//       Serial.println(WiFi.localIP());
//     } else {
//       Serial.println("Failed to connect to WiFi, starting Access Point...");
//       startAccessPoint();
//     }
//   } else {
//     startAccessPoint();
//   }

//   // Запуск веб-сервера
//   setupWebServer();

//   // Запуск індикації діодом
//   ledStopFlag = false;
//   xTaskCreate(ledTask, "LED Blink Task", 1024, NULL, 1, &ledTaskHandle);
// }

// void setupWebServer() {
//   server.on("/", HTTP_GET, [](AsyncWebServerRequest *request) {
//     String html = "<form action='/save' method='POST'>"
//                   "<label>WiFi SSID:</label><input type='text' name='ssid' value='" + String(wifiSSID) + "'><br>"
//                   "<label>WiFi Password:</label><input type='password' name='password' value='" + String(wifiPassword) + "'><br>"
//                   "<input type='submit' value='Save'></form><br>"
//                   "<button onclick=\"window.location.href='/start_ap'\">Start Access Point</button>"
//                   "<button onclick=\"window.location.href='/connect_wifi'\">Connect to WiFi</button>"
//                   "<h2>Modbus Registers</h2>"
//                   "<p>Register 0: " + String(reg0) + "</p>"
//                   "<p>Register 1: " + String(reg1) + "</p>"
//                   "<p>Register 35: " + String(register35Value) + "</p>";
//     request->send(200, "text/html", html);
//   });

//   server.on("/save", HTTP_POST, [](AsyncWebServerRequest *request) {
//     if (request->hasParam("ssid", true) && request->hasParam("password", true)) {
//       String ssid = request->getParam("ssid", true)->value();
//       String password = request->getParam("password", true)->value();
//       ssid.toCharArray(wifiSSID, sizeof(wifiSSID));
//       password.toCharArray(wifiPassword, sizeof(wifiPassword));

//       preferences.begin("wifi-settings", false);
//       preferences.putString("wifiSSID", wifiSSID);
//       preferences.putString("wifiPassword", wifiPassword);
//       preferences.end();

//       request->send(200, "text/html", "Settings saved! Restarting...");
//       delay(1000);
//       ESP.restart();
//     } else {
//       request->send(400, "text/html", "Missing parameters");
//     }
//   });

//   server.on("/start_ap", HTTP_GET, [](AsyncWebServerRequest *request) {
//     preferences.begin("wifi-settings", false);
//     preferences.putInt("mode", 0);  // Точка доступу
//     preferences.end();
//     request->send(200, "text/html", "Access Point mode set. Restarting...");
//     delay(1000);
//     ESP.restart();
//   });

//   server.on("/connect_wifi", HTTP_GET, [](AsyncWebServerRequest *request) {
//     preferences.begin("wifi-settings", false);
//     preferences.putInt("mode", 1);  // Режим Wi-Fi
//     preferences.end();
//     request->send(200, "text/html", "WiFi mode set. Restarting...");
//     delay(1000);
//     ESP.restart();
//   });

//   server.begin();
// }

// void startAccessPoint() {
//   WiFi.mode(WIFI_AP);
//   WiFi.softAP(apSSID, apPassword);
//   IPAddress IP = WiFi.softAPIP();
//   Serial.print("AP IP address: ");
//   Serial.println(IP);
// }

// void ledTask(void *parameter) {
//   unsigned long startMillis = millis();
//   while (!ledStopFlag && millis() - startMillis < blinkDuration) {
//     if (mode == 0) {
//       digitalWrite(LED_PIN, !digitalRead(LED_PIN));  // Миготіння
//       delay(500);
//     } else if (WiFi.status() == WL_CONNECTED) {
//       digitalWrite(LED_PIN, HIGH);  // Постійне світіння
//       delay(500);
//     }
//   }
//   digitalWrite(LED_PIN, LOW);
//   ledStopFlag = true;
//   vTaskDelete(NULL);
// }

// void loop() {
//   unsigned long currentMillis = millis();

//   // Читання регістрів Modbus кожну секунду
//   if (currentMillis - previousMillis >= interval) {
//     previousMillis = currentMillis;
//     readRegisters();
//   }

//   // Зміна значення регістра 35 кожні 5 секунд
//   if (currentMillis - previousMillisForRegister35 >= intervalForRegister35) {
//     previousMillisForRegister35 = currentMillis;
//     incrementRegister35();
//   }
// }

// void readRegisters() {
//   uint8_t result;

//   result = node.readHoldingRegisters(0, 1);  // Читання регістра 0
//   if (result == node.ku8MBSuccess) {
//     reg0 = node.getResponseBuffer(0);
//     Serial.print("Register 0: ");
//     Serial.println(reg0);
//   } else {
//     Serial.print("Error reading register 0: ");
//     Serial.println(result, HEX);
//   }

//   delay(300);

//   result = node.readHoldingRegisters(1, 1);  // Читання регістра 1
//   if (result == node.ku8MBSuccess) {
//     reg1 = node.getResponseBuffer(0);
//     Serial.print("Register 1: ");
//     Serial.println(reg1);
//   } else {
//     Serial.print("Error reading register 1: ");
//     Serial.println(result, HEX);
//   }
// }

// void incrementRegister35() {
//   register35Value += 0.5;
//   uint8_t result = node.writeSingleRegister(1, (uint16_t)(register35Value * 100));
//   if (result == node.ku8MBSuccess) {
//     Serial.print("Register 35 updated to: ");
//     Serial.println(register35Value);
//   } else {
//     Serial.print("Error updating register 35: ");
//     Serial.println(result, HEX);
//   }
// }

///////////
// #include <Arduino.h>
// #include <SPIFFS.h>
// #include <ArduinoJson.h>

// #define MINUTES_IN_DAY 1440  // 24 * 60
// #define PREDICTION_MINUTES 30  // Кількість хвилин для прогнозу
// #define TREND_WINDOW 5  // Кількість попередніх хвилин для обчислення тренду

// // Структура для зберігання даних генерації
// struct GenerationData {
//   float power;         // Потужність генерації у ватах
//   float temperature;   // Температура у градусах Цельсія
//   float cloudiness;    // Хмарність (0-100%)
// };

// // Функція для обчислення сезонної ефективності
// float calculateSeasonalEfficiency(int dayOfYear) {
//   float seasonalEfficiency = (30 + 70 * pow(cos(PI * (dayOfYear - 200) / 365), 5)) / 100.0;
//   return seasonalEfficiency;
// }

// // Функція для обчислення середнього значення генерації на конкретну хвилину за попередні дні
// float calculateMinuteAverage(GenerationData** historicalData, int numDays, int minuteIndex) {
//   float sum = 0;
//   for (int i = 0; i < numDays; i++) {
//     sum += historicalData[i][minuteIndex].power;
//   }
//   return sum / numDays;
// }

// // Функція для обчислення тренду генерації
// float calculateTrend(GenerationData* currentDayData, int currentMinuteIndex) {
//   if (currentMinuteIndex < 1) return 0;  // Не можемо обчислити тренд без попередніх даних

//   int startIndex = currentMinuteIndex - TREND_WINDOW;
//   if (startIndex < 0) startIndex = 0;

//   float sumTrend = 0;
//   int count = 0;

//   for (int i = startIndex + 1; i <= currentMinuteIndex; i++) {
//     sumTrend += (currentDayData[i].power - currentDayData[i - 1].power);
//     count++;
//   }

//   if (count > 0)
//     return sumTrend / count;
//   else
//     return 0;
// }

// // Функція для прогнозування генерації
// void predictGeneration(GenerationData** historicalData30Days,
//                        int numDays30,
//                        GenerationData** historicalData3Days,
//                        int numDays3,
//                        GenerationData* currentDayData,
//                        int currentMinuteIndex,
//                        int dayOfYear,
//                        int minutesToPredict) {

//   // Обчислення сезонної ефективності
//   float seasonalEfficiency = calculateSeasonalEfficiency(dayOfYear);

//   // Отримання поточної температури та хмарності
//   float currentTemperature = currentDayData[currentMinuteIndex].temperature;
//   float currentCloudiness = currentDayData[currentMinuteIndex].cloudiness;

//   // Обчислення тренду
//   float trend = calculateTrend(currentDayData, currentMinuteIndex);

//   // Ініціалізація вагових коефіцієнтів
//   float weight30Days = 0.25;
//   float weight3Days = 0.25;
//   float weightTrend = 0.15;
//   float weightTemperature = 0.2;
//   float weightCloudiness = 0.15;

//   Serial.println("Прогноз генерації на наступні хвилини:");

//   float lastPrediction = currentDayData[currentMinuteIndex].power;  // Початкове значення генерації

//   for (int i = 1; i <= minutesToPredict; i++) {
//     int futureMinuteIndex = currentMinuteIndex + i;
//     if (futureMinuteIndex >= MINUTES_IN_DAY) {
//       futureMinuteIndex = futureMinuteIndex % MINUTES_IN_DAY;
//     }

//     // Обчислення середніх значень для майбутньої хвилини
//     float avg30DaysFuture = calculateMinuteAverage(historicalData30Days, numDays30, futureMinuteIndex);
//     float avg3DaysFuture = calculateMinuteAverage(historicalData3Days, numDays3, futureMinuteIndex);

//     // Прогноз генерації
//     float predictedGeneration = weight30Days * avg30DaysFuture +
//                                 weight3Days * avg3DaysFuture +
//                                 weightTrend * trend * i +  // Множимо на i для прогнозу в майбутнє
//                                 weightTemperature * currentTemperature +
//                                 (-weightCloudiness) * currentCloudiness;  // Негативний вплив хмарності

//     // Застосовуємо сезонну ефективність
//     predictedGeneration *= seasonalEfficiency;

//     // Оновлюємо останнє прогнозоване значення
//     lastPrediction = predictedGeneration;

//     // Виводимо прогноз на кожну хвилину
//     Serial.print("Хвилина ");
//     Serial.print(futureMinuteIndex);
//     Serial.print(": ");
//     Serial.print(lastPrediction);
//     Serial.println(" Вт");
//   }
// }

// // Функція для завантаження даних з файлів JSON (спрощено для прикладу)
// bool loadHistoricalData(const char* fileName, GenerationData* data) {
//   File file = SPIFFS.open(fileName, "r");
//   if (!file) {
//     Serial.println("Помилка відкриття файлу");
//     return false;
//   }

//   // Припустимо, що файл містить масив об'єктів з полями power, temperature, cloudiness
//   StaticJsonDocument<4096> doc;  // Розмір може бути налаштований

//   DeserializationError error = deserializeJson(doc, file);
//   if (error) {
//     Serial.println("Помилка парсингу JSON");
//     file.close();
//     return false;
//   }

//   JsonArray array = doc.as<JsonArray>();
//   int index = 0;
//   for (JsonObject obj : array) {
//     if (index >= MINUTES_IN_DAY) break;
//     data[index].power = obj["power"];
//     data[index].temperature = obj["temperature"];
//     data[index].cloudiness = obj["cloudiness"];
//     index++;
//   }

//   file.close();
//   return true;
// }

// void setup() {
//   Serial.begin(115200);

//   // Ініціалізація SPIFFS
//   if (!SPIFFS.begin(true)) {
//     Serial.println("Помилка ініціалізації SPIFFS");
//     return;
//   }

//   // Завантаження історичних даних
//   const int numDays30 = 30;
//   GenerationData* historicalData30Days[numDays30];

//   for (int i = 0; i < numDays30; i++) {
//     historicalData30Days[i] = new GenerationData[MINUTES_IN_DAY];
//     char fileName[20];
//     snprintf(fileName, sizeof(fileName), "/day_%d.json", i + 1);  // Імена файлів: /day_1.json, /day_2.json, ...
//     if (!loadHistoricalData(fileName, historicalData30Days[i])) {
//       Serial.println("Помилка завантаження даних за 30 днів");
//       return;
//     }
//   }

//   // Завантаження даних за 3 дні (аналогічно)
//   const int numDays3 = 3;
//   GenerationData* historicalData3Days[numDays3];

//   for (int i = 0; i < numDays3; i++) {
//     historicalData3Days[i] = new GenerationData[MINUTES_IN_DAY];
//     char fileName[20];
//     snprintf(fileName, sizeof(fileName), "/day_%d.json", i + 1);  // Використовуємо ті ж файли для прикладу
//     if (!loadHistoricalData(fileName, historicalData3Days[i])) {
//       Serial.println("Помилка завантаження даних за 3 дні");
//       return;
//     }
//   }

//   // Завантаження даних поточного дня
//   GenerationData currentDayData[MINUTES_IN_DAY];
//   if (!loadHistoricalData("/current_day.json", currentDayData)) {
//     Serial.println("Помилка завантаження даних поточного дня");
//     return;
//   }

//   // Отримання поточного часу та дня року
//   int currentMinuteIndex = 5 * 60 + 40;  // Наприклад, 5:40 ранку
//   int dayOfYear = 200;  // Потрібно отримати актуальний день року

//   // Виклик функції прогнозування
//   predictGeneration(historicalData30Days, numDays30,
//                     historicalData3Days, numDays3,
//                     currentDayData, currentMinuteIndex,
//                     dayOfYear, PREDICTION_MINUTES);

//   // Очищення виділеної пам'яті
//   for (int i = 0; i < numDays30; i++) {
//     delete[] historicalData30Days[i];
//   }
//   for (int i = 0; i < numDays3; i++) {
//     delete[] historicalData3Days[i];
//   }
// }

// void loop() {
//   // Порожній цикл
// }
