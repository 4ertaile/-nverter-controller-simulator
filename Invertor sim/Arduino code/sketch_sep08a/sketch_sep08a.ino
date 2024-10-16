#include <ModbusMaster.h>
#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <Preferences.h>
#include <NTPClient.h>
#include <HTTPClient.h>
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

//weather options

char latitude[32] = "";          // Широта
char longitude[32] = "";         // Довгота
float temperature;
int cloudiness;
char apiKey[64] = "";    // Ваш API-ключ OpenWeatherMap


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

// Структура для збереження даних
struct EnergyData {
  String Time;
  float PowerConsumption;
  float SolarGeneration;
  float Temperature;
  float cloudiness;
};
 

void setup() {
  pinMode(LED_PIN, OUTPUT);  // Діод як вихід
  Serial.begin(9600);

  // Ініціалізація Modbus
  node.begin(1, Serial);  // ID = 1 (Slave address)

  // Завантажуємо налаштування Wi-Fi
  preferences.begin("esp-settings", false);
  preferences.getString("wifiSSID", wifiSSID, sizeof(wifiSSID));
  preferences.getString("wifiPassword", wifiPassword, sizeof(wifiPassword));
  preferences.getString("apiKey", apiKey, sizeof(apiKey));
  mode = preferences.getInt("mode", 0);
  preferences.getString("latitude", latitude,sizeof(latitude));
  preferences.getString("longitude", longitude,sizeof(longitude));
  isWorking = preferences.getBool("isWorking", false);  // Другий параметр — значення за замовчуванням
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
  //delete this
  std::vector<String> filePaths = { "/09_2024/18-09-2024.json", "/09_2024/23-09-2024.json" };
  String searchTime = "12:05:00";

  EnergyData averageData = processFilesAndGetAverage(filePaths, searchTime);

  Serial.println("Time: " + averageData.Time);
  Serial.println("Average Power Consumption: " + String(averageData.PowerConsumption));
  Serial.println("Average Solar Generation: " + String(averageData.SolarGeneration));
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

// Функція для отримання температури та хмарності
void getWeatherData(float lat, float lon, float &temperature, int &cloudiness) {
  String units = "metric";
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;

    // Формуємо API-запит з широтою, довготою та одиницями виміру
    String apiUrl = "http://api.openweathermap.org/data/2.5/weather?lat=" + String(lat, 6) +
                    "&lon=" + String(lon, 6) + "&appid=" + apiKey + "&units=" + units;

    http.begin(apiUrl);  // Встановлюємо URL для запиту
    int httpCode = http.GET();  // Виконуємо GET-запит

    if (httpCode > 0) {
      // Перевіряємо код відповіді
      if (httpCode == HTTP_CODE_OK) {
        String payload = http.getString();  // Отримуємо відповідь у вигляді строки

        // Розбір JSON-даних
        DynamicJsonDocument doc(1024);
        deserializeJson(doc, payload);

        // Отримуємо необхідні дані з JSON
        temperature = doc["main"]["temp"];
        cloudiness = doc["clouds"]["all"];
      } else {
        Serial.println("Error on HTTP request");
      }
    } else {
      Serial.println("Failed to connect to API");
    }

    http.end();  // Закриваємо з'єднання
  } else {
    Serial.println("WiFi Disconnected");
  }
}

// Функція для перетворення часу з формату "HH:MM:SS" у кількість секунд з початку дня
time_t convertToSecondsFromMidnight(String timeString) {
  int hour, minute, second;

  // Парсинг строки часу в формате "HH:MM:SS"
  sscanf(timeString.c_str(), "%d:%d:%d", &hour, &minute, &second);

  // Перетворюємо час у секунди з початку дня
  return hour * 3600 + minute * 60 + second;
}

EnergyData processFilesAndGetAverage(std::vector<String> filePaths, String searchTime) {
  float totalPowerConsumption = 0.0;
  float totalSolarGeneration = 0.0;
  int count = 0;
  const size_t bufferSize = 14000;  // Размер буфера для чтения файла
  const size_t bufferSizeForStruct = 256;

  for (const String &filePath : filePaths) {
    File file = SD.open(filePath, FILE_READ);
    if (!file) {
      Serial.println("Failed to open file: " + filePath);
      continue;
    }

    // Создаем буфер для чтения части файла
    String buffer = "[";
    int braceCount = 0;  // Счетчик для символов '}'
    String lastStructBuffer = "";
    bool found = false;

    // Читаем файл порциями
    while (file.available() && buffer.length() < bufferSize - 1) {  // -1 для добавления ']'
      char ch = (char)file.read();
      if (buffer.length() == 1 && ch == ',') {
        continue;
      }
      if (braceCount == 119) {
        lastStructBuffer += ch;
      }
      // Увеличиваем счетчик, если встречаем '}'
      if (ch == '}') {
        braceCount++;
      }
      // Если символ не '[' и ']', добавляем его в буфер
      if (ch != '[' && ch != ']') {
        buffer += ch;
      }
      // Если буфер не пустой, завершаем массив JSON
      if (buffer.length() > 1 && (braceCount >= 120 || !file.available())) {  // Более 1 для учета '[' и ']'
        buffer += "]";                                                        // Завершаем массив JSON
        lastStructBuffer = lastStructBuffer.substring(1);

        // Парсим маленький JSON-объект
        DynamicJsonDocument smallDoc(bufferSizeForStruct);
        DeserializationError smallError = deserializeJson(smallDoc, lastStructBuffer);
        if (smallError) {
          Serial.println("Error deserializing JSON: " + String(smallError.f_str()));
          buffer = "[";
          lastStructBuffer = "";
          braceCount = 0;  // Счетчик для символов '}'
          smallDoc.clear();
          continue;
        }
        JsonObject data = smallDoc.as<JsonObject>();
        if (convertToSecondsFromMidnight(data["Time"]) < convertToSecondsFromMidnight(searchTime)) {
          buffer = "[";
          lastStructBuffer = "";
          braceCount = 0;  // Счетчик для символов '}'
          smallDoc.clear();
          continue;
        }
        smallDoc.clear();

        // Парсим основной JSON-объект
        DynamicJsonDocument largeDoc(bufferSize);
        DeserializationError largeError = deserializeJson(largeDoc, buffer);
        if (largeError) {
          Serial.println("Error deserializing JSON: " + String(largeError.f_str()));
          buffer = "[";
          lastStructBuffer = "";
          braceCount = 0;  // Счетчик для символов '}'
          smallDoc.clear();
          continue;
        }

        // Шукаємо запис із відповідним часом
        JsonArray dataArray = largeDoc.as<JsonArray>();
        for (JsonObject record : dataArray) {
          if (record["Time"] == searchTime) {
            totalPowerConsumption += record["PowerConsumption"].as<float>();
            totalSolarGeneration += record["SolarGeneration"].as<float>();
            count++;
            found = true;
            break;  // Если нашли соответствующий запись, выходим из цикла
          }
        }
        largeDoc.clear();
        buffer = "[";
        lastStructBuffer = "";
        braceCount = 0;  // Счетчик для символов '}'
      }
      if (found) {
        break;
      }
    }
    file.close();
  }

  // Обчислюємо середнє значення
  EnergyData result;
  result.Time = searchTime;

  if (count > 0) {
    result.PowerConsumption = totalPowerConsumption / count;
    result.SolarGeneration = totalSolarGeneration / count;
  } else {
    result.PowerConsumption = 0.0;
    result.SolarGeneration = 0.0;
    Serial.println("No matching records found for time: " + searchTime);
  }

  return result;
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

std::vector<String> findFilesForLastDays(int daysToCheck) {
  std::vector<String> files;
  if (!sDIsOk()) {
    return files;
  }
  time_t currentTime = timeClient.getEpochTime();  // Отримуємо поточний UNIX-час

  // Отримуємо поточний місяць і рік
  String currentMonth = getFormattedDate().substring(5, 7);
  String currentYear = getFormattedDate().substring(0, 4);

  // Перевіряємо поточний місяць
  String folderName = "/" + currentMonth + "_" + currentYear;
  if (SD.exists(folderName)) {
    File dir = SD.open(folderName);
    while (true) {
      File entry = dir.openNextFile();
      if (!entry) break;

      String fileName = entry.name();
      tmElements_t fileDate = parseDate(fileName.substring(0, 10));  // Парсимо дату з імені файлу
      time_t fileTime = makeTime(fileDate);                          // Перетворюємо у UNIX-час

      int dayDiff = (currentTime - fileTime) / SECS_PER_DAY;  // Обчислюємо різницю у днях
      // Перевіряємо, чи файл знаходиться між поточною датою (не включаючи) та 25 днями назад (включно)
      if (dayDiff > 0 && dayDiff <= 25) {              // Поточний день виключено
        files.push_back(folderName + "/" + fileName);  // Додаємо файл до списку
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
  if (!sDIsOk()) {
    return files;
  }
  int daysToCheck = 25;  // Максимум 25 днів для перевірки

  // Перший пошук у поточному місяці
  String date = getFormattedDate();
  String dayInFormat = date.substring(8, 10) + "-" + date.substring(5, 7) + "-" + date.substring(0, 4) + ".json";
  String currentMonth = getFormattedDate().substring(5, 7);    // Отримуємо поточний місяць (MM)
  String currentYear = getFormattedDate().substring(0, 4);     // Отримуємо поточний рік (YYYY)
  String folderName = "/" + currentMonth + "_" + currentYear;  // Папка формату mm_yyyy
  if (SD.exists(folderName)) {
    File dir = SD.open(folderName);
    while (true) {
      File entry = dir.openNextFile();
      if (!entry) break;

      String fileName = entry.name();
      if (getDayOfWeekFromFileName(fileName) == dayOfWeek && dayInFormat != fileName) {
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
          time_t currentTime = timeClient.getEpochTime();
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

String getTodayDataFilePathIfExist() {
  String FileName = "";
  String currentFileName = "";

  String date = getFormattedDate();
  String folderName = "/" + date.substring(5, 7) + "_" + date.substring(0, 4);                                  // /mm_yyyy
  String fileName = date.substring(8, 10) + "-" + date.substring(5, 7) + "-" + date.substring(0, 4) + ".json";  // dd-mm-yyyy.json
  currentFileName = folderName + "/" + fileName;

  if (SD.exists(folderName) && SD.exists(currentFileName)) {
    return currentFileName;
  }
  return "";
}

String getTodayDataFileNameIfExist() {
  String FileName = "";
  String currentFileName = "";

  String date = getFormattedDate();
  String folderName = "/" + date.substring(5, 7) + "_" + date.substring(0, 4);                                  // /mm_yyyy
  String fileName = date.substring(8, 10) + "-" + date.substring(5, 7) + "-" + date.substring(0, 4) + ".json";  // dd-mm-yyyy.json
  currentFileName = folderName + "/" + fileName;

  if (SD.exists(folderName) && SD.exists(currentFileName)) {
    return fileName;
  }
  return "";
}

bool sDIsOk() {
  if (SD.begin(chipSelect)) {
    SD_Status = "SD card initialization fine";
    return true;
  } else {
    SD_Status = "SD card initialization failed!";
    return false;
  }
}

// Запис даних у JSON на SD-карту
void writeDataToSD(String requestTime) {
  if (!sDIsOk()) {
    return;
  }
  fileParseStatus = "";
  currentFileName = "";
  fileStatus = "";

  // Оновлюємо назву файлу відповідно до дати
  String date = getFormattedDate();
  String folderName = "/" + date.substring(5, 7) + "_" + date.substring(0, 4); 
  String fileName = date.substring(8, 10) + "-" + date.substring(5, 7) + "-" + date.substring(0, 4) + ".json"; 

  if (!SD.exists(folderName)) {
    if (!SD.mkdir(folderName)) {
      fileParseStatus = "Failed to create folder: " + folderName;
      return;
    }
  }

  currentFileName = folderName + "/" + fileName;

  // Якщо файл не існує, створюємо новий
  if (!SD.exists(currentFileName)) {
    File file = SD.open(currentFileName, FILE_WRITE);
    if (file) {
      file.print("[");  // Створюємо початок JSON масиву
      file.close();
    } else {
      fileParseStatus = "Failed to create file";
      return;
    }
  }

  // Відкриваємо файл для читання
  File file = SD.open(currentFileName, FILE_READ);
  if (!file) {
    fileStatus = "Failed to open file for reading";
    return;
  }

  // Читаємо лише кінець файлу, щоб перевірити, чи вже є дані, і чи потрібно додати новий елемент
  file.seek(file.size() - 1);  // Переходимо до кінця файлу
  char lastChar = file.read();
  file.close();

  // Відкриваємо файл для запису в кінець
  file = SD.open(currentFileName, FILE_WRITE);
  if (!file) {
    fileStatus = "Failed to open file for writing";
    return;
  }

  // Якщо файл пустий або закритий, додаємо новий елемент
  if (lastChar == ']') {
    file.seek(file.size() - 1);  // Видаляємо закриваючу дужку
    file.print(",");  // Додаємо кому перед новим елементом
  }

  // Додаємо новий елемент у JSON масив
  DynamicJsonDocument doc(256);
  JsonObject newRecord = doc.to<JsonObject>();
  newRecord["Time"] = requestTime;
  newRecord["PowerConsumption"] = powerConsumption;
  newRecord["SolarGeneration"] = solarGeneration;

  // Записуємо новий елемент
  serializeJson(newRecord, file);
  file.print("]");  // Закриваємо масив

  file.close();

  fileStatus = "Data written to SD card";
}

String makeIndexFile(String chunk) {
  String chunkUrl = "/static/" + chunk;

  if (sDIsOk() && SD.open("/static/index.js")) {
  return "<!DOCTYPE html>"
      "<html lang=\"en\">"
      "<head>"
        "<meta charset=\"utf-8\" />"
        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />"
        "<title>The invertor controller</title>"
        "<script src=\"/static/shared.js\"></script>"
      "</head>"
      "<body style=\"display: block;\">"
        "<script src=" + chunkUrl + "></script>"
        "<div id=\"app\"></div>"
      "</body>"
      "</html>";
  }
  String pCode = "
    const send_patch = (url: string) => async () =>{
        let response = await fetch(url, {
            method: 'PATCH',
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
    }
  ";

  String html = "<!DOCTYPE html>"
      "<html lang=\"en\">"
      "<head>"
        "<meta charset=\"utf-8\" />"
        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />"
        "<title>The invertor controller</title>"
        "<script>"+pCode+"</script>"
      "</head>"
      "<body style=\"display: block;\">"
        "<form action='/saveWifi' method='POST'>"
                      "<label>WiFi SSID:</label><input type='text' name='ssid' value='"+ String(wifiSSID) + "'><br>"
                      "<label>WiFi Password:</label><input type='password' name='password' value='"+ String(wifiPassword) + "'><br>"
                      "<input type='submit' value='Save'></form><br>"
                      "<form action='/saveWeather' method='POST'>"
                      "<label>ApiKey:</label><input type='text' name='apiKey' value='"+ String(apiKey) + "'><br>"
                      "<label>Latitude:</label><input  name='latitude' value='"+ String(latitude) + "'><br>"
                      "<label>Longitude:</label><input  name='longitude' value='"+ String(longitude) + "'><br>"
                      "<input type='submit' value='Save'></form><br>"
                      "<button onclick=\"send_patch('/start_ap')\">Start Access Point</button><br>"
                      "<button onclick=\"send_patch('/connect_wifi')\">Connect to WiFi</button><br>"
                      "<button onclick=\"send_patch('/sync_time')\">Synchronize Time</button><br>"
                      "<button onclick=\"send_patch('/start_work')\">Start Work</button><br>"
                      "<button onclick=\"send_patch('/stop_work')\">Stop Work</button><br>"
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
                       "<button onclick=\"window.location.href='/status'\">View SD Card and Data</button><br>"
      "</body>"
      "</html>";
   return html;

}

void setupWebServer() {
  // Головна сторінка: Завантаження з SD-карти або повернення до стандартного варіанту

  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request) {
    String html = makeIndexFile("index.js");
    request->send(200, "text/html", html);
  });

  server.on("/seeFiles", HTTP_GET, [](AsyncWebServerRequest *request) {
    String html = makeIndexFile("files.js");
    request->send(200, "text/html", html);
  });

  server.on("/getFiles", HTTP_GET, [](AsyncWebServerRequest *request) {
    std::vector<String> files = findFilesForLastDays(25);
    String filesArray = "[";
    for (int i = 0; i < files.size(); i++) {
      filesArray += "{\"fname\": \"" + files[i] + "\", \"date\": \"" + getDayOfWeekFromFileName(files[i]) + "\"}";
      if (i != files.size() - 1) {
        filesArray += ",";
      }
    }
    filesArray += "]";

    String time = "\"" + getFormattedDate() + "\"";

    String JsonData = "{\"files\": " + filesArray + "," + "\"time\": " + time + "}";
    request->send(200, "application/json", JsonData);
  });

  // Сервер для завантаження files з SD-карти

  // right is on sd card
  server.serveStatic("/static/", SD, "/static/");

  // migrate all the data interactions into this way
  server.on("/status", HTTP_GET, [](AsyncWebServerRequest *request) {
    // Формуємо JSON з даними
    DynamicJsonDocument doc(1024);

    doc["temperature"] = String(temperature);
    doc["cloudiness"] = String(cloudiness);

    doc["solarGeneration"] = String(solarGeneration);
    doc["powerConsumption"] = String(powerConsumption);

    String jsonResponse;
    serializeJson(doc, jsonResponse);

    // Відправляємо JSON-відповідь
    request->send(200, "application/json", jsonResponse);
  });

  server.on("/options", HTTP_GET,[](AsyncWebServerRequest *request){
    // Формуємо JSON з даними
    DynamicJsonDocument doc(1024);

    // wifiSSID, wifiPassword, apiKey, latitude, longitude

    doc["temperature"] = String(temperature);
    doc["cloudiness"] = String(cloudiness);

    doc["solarGeneration"] = String(solarGeneration);
    doc["powerConsumption"] = String(powerConsumption);

    String jsonResponse;
    serializeJson(doc, jsonResponse);

    // Відправляємо JSON-відповідь
    request->send(200, "application/json", jsonResponse);
  });

  // Інші наявні маршрути
  server.on("/saveWifi", HTTP_POST, [](AsyncWebServerRequest *request) {
    if (request->hasParam("ssid", true) && request->hasParam("password", true)) {
      String ssid = request->getParam("ssid", true)->value();

      String password = request->getParam("password", true)->value();

      ssid.toCharArray(wifiSSID, sizeof(wifiSSID));
      password.toCharArray(wifiPassword, sizeof(wifiPassword));

      preferences.begin("esp-settings", false);
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

  server.on("/saveWeather", HTTP_POST, [](AsyncWebServerRequest *request) {
    if (request->hasParam("apiKey", true) && request->hasParam("latitude", true) && request->hasParam("longitude", true)) {
      String NapiKey = request->getParam("apiKey", true)->value();
      String Nlatitude = request->getParam("latitude", true)->value();
      String Nlongitude = request->getParam("longitude", true)->value();
      NapiKey.toCharArray(apiKey, sizeof(apiKey));
      Nlatitude.toCharArray(latitude, sizeof(latitude));
      Nlongitude.toCharArray(longitude, sizeof(longitude));

      preferences.begin("esp-settings", false);
      preferences.putString("apiKey", NapiKey);
      preferences.putString("latitude", Nlatitude);
      preferences.putString("longitude", Nlongitude);
      preferences.end();


      request->send(200, "text/html", "Settings saved! Restarting...");
    } else {
      request->send(400, "text/html", "Missing parameters");
    }
  });

  server.on("/start_work", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
    preferences.putBool("isWorking", true);  // Точка доступу
    preferences.end();
    isWorking = true;
    request->send(200, "text/html", "Started Modbus reading and data logging.");
  });

  server.on("/stop_work", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
    preferences.putBool("isWorking", false);  // Точка доступу
    preferences.end();
    isWorking = false;
    request->send(200, "text/html", "Stopped Modbus reading and data logging.");
  });

  server.on("/sync_time", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    syncTime();
    request->send(200, "text/html", "Time synchronization attempted.");
  });

  server.on("/start_ap", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
    preferences.putInt("mode", 0);  // Точка доступу
    preferences.end();
    request->send(200, "text/html", "Access Point mode set. Restarting...");
    delay(1000);
    ESP.restart();
  });

  server.on("/connect_wifi", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
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

void getInfoFromInvertor() {
  if (isWorking && timeSynced && timeClient.getSeconds() == 0) {
    int hours = timeClient.getHours();
    int minutes = timeClient.getMinutes();
    String time = String(hours < 10 ? "0" : "") + String(hours) + ":" + String(minutes < 10 ? "0" : "") + String(minutes) + ":" + "00";
    readModbusData();
    getWeatherData(atof(latitude), atof(longitude), temperature, cloudiness);
    writeDataToSD(time);
  }
}

void loop() {
  getInfoFromInvertor();

  // Перевіряємо час для зчитування Modbus даних
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
