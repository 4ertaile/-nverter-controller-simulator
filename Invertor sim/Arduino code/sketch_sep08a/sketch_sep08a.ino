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
NTPClient timeClient(ntpUDP, "time.windows.com", 7200, 60000);  // Оновлення кожні 60 секунд

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

char latitude[32] = "";   // Широта
char longitude[32] = "";  // Довгота
float temperature;
int cloudiness;
char apiKey[64] = "";  // Ваш API-ключ OpenWeatherMap

//invertor settings
char invertorIp[15] = "";
char invertorPort[4] = "";
char invertorId[10] = "";

float solarGeneration = 0, powerConsumption = 0;
bool isWorking = false, timeSynced = false;
unsigned long previousMillisForModbus = 0;
const long intervalForModbus = 60000;  // Запит кожну хвилину
String SD_Status = "";
String Wifi_status = "";
const int chipSelect = 5;  // Пін CS для SD-карти
String currentFileName = "";
String fileStatus = "";
unsigned long lastUpdateTime = 0;
bool weatherUpdated = false;  // Чи були успішно оновлені дані
String weatherUpdatedStatus = "";


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
  float Cloudiness;
};
// Структура для зберігання даних про прогноз
struct ForecastData {
  String Time;
  float Temperature;
  float Cloudiness;
};
std::vector<String> findFilesByDayOfWeekAndTime(String dayOfWeek, int daysToCheck, String time);
std::vector<String> findFilesForLastDays(int daysToCheck, String time);
std::vector<String> getCacheFilePaths(String inputDate, String fileType);
std::vector<ForecastData> getForecastData(time_t startTime, int minutes);
std::vector<ForecastData> loadForecastData(int forecastMinutes);
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
  preferences.getString("invertorIp", invertorIp, sizeof(invertorIp));
  preferences.getString("invertorPort", invertorPort, sizeof(invertorPort));
  preferences.getString("invertorId", invertorId, sizeof(invertorId));
  mode = preferences.getInt("mode", 0);
  preferences.getString("latitude", latitude, sizeof(latitude));
  preferences.getString("longitude", longitude, sizeof(longitude));
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
  String searchTime = "20:05:00";
  std::vector<String> filePaths = findFilesByDayOfWeekAndTime("Thursday", 25, searchTime);
  for (const String &filePath : filePaths) {
    Serial.println(filePath);
  }
  std::vector<String> filePaths1 = findFilesForLastDays(25, searchTime);
  for (const String &filePath1 : filePaths1) {
    Serial.println(filePath1);
  }
  calculateAndStoreAverages(filePaths1, "20-10-2024", "last-25");
  std::vector<String> filePaths2 = getCacheFilePaths("20-10-2024", "last-25");
  for (const String &filePath2 : filePaths2) {
    Serial.println(filePath2);
  }

  DynamicJsonDocument forecastData = fetchWeatherForecast();
  saveForecastToFiles(forecastData, 335);
   printWeatherForecast(forecastData);

  int forecastMinutes = 335;  // Наприклад, 120 хвилин
  std::vector<ForecastData> forecastData1 = loadForecastData(forecastMinutes);
  if (!forecastData1.empty()) {
    for (const ForecastData &data : forecastData1) {
      Serial.print("Time: ");
      Serial.print(data.Time);
      Serial.print(", Temperature: ");
      Serial.print(data.Temperature);
      Serial.print(", Cloudiness: ");
      Serial.println(data.Cloudiness);
    }
  } else {
    Serial.println("LOL");
  }
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
//////




// Функція для отримання прогнозу з OpenWeather API
DynamicJsonDocument fetchWeatherForecast() {
  DynamicJsonDocument forecastData(2048);
  HTTPClient http;

  String url = "http://api.openweathermap.org/data/2.5/forecast?lat=" + String(latitude, 6) + "&lon=" + String(longitude, 6) + "&appid=" + apiKey + "&units=metric";
  http.begin(url);
  int httpResponseCode = http.GET();

  if (httpResponseCode == 200) {
    String payload = http.getString();
    deserializeJson(forecastData, payload);
  } else {
    Serial.print("Error in fetching weather data: ");
    Serial.println(httpResponseCode);
  }

  http.end();
  return forecastData;
}
void printWeatherForecast(DynamicJsonDocument forecastData) {
  // Перевіряємо, чи масив даних прогнозу є правильним
  if (!forecastData.containsKey("list") || !forecastData["list"].is<JsonArray>()) {
    Serial.println("No forecast data available.");
    return;
  }

  // Отримуємо розмір масиву прогнозів
  int forecastSize = forecastData["list"].size();

  // Проходимо по кожному запису в масиві прогнозів
  for (int i = 0; i < forecastSize; ++i) {
    // Отримуємо час у форматі UNIX-мітки часу
    time_t forecastTime = static_cast<time_t>(forecastData["list"][i]["dt"]);
    // Конвертуємо UNIX-час у формат дати і часу
    struct tm *timeInfo = localtime(&forecastTime);
    char timeBuffer[20];
    strftime(timeBuffer, sizeof(timeBuffer), "%Y-%m-%d %H:%M:%S", timeInfo);

    // Отримуємо температуру та хмарність
    float temperature = forecastData["list"][i]["main"]["temp"];
    int clouds = forecastData["list"][i]["clouds"]["all"];

    // Виводимо значення часу, температури та хмарності
    Serial.print("Time: ");
    Serial.print(timeBuffer);
    Serial.print(" | Temperature: ");
    Serial.print(temperature);
    Serial.print("°C | Cloudiness: ");
    Serial.print(clouds);
    Serial.println("%");
  }
}

float interpolate(float startValue, float endValue, int startMinute, int targetMinute, int endMinute) {
  if (endMinute == startMinute) {
    return startValue;
  }
  return startValue + (endValue - startValue) * (targetMinute - startMinute) / (endMinute - startMinute);
}

void printTimeInHHMMSS(time_t rawTime) {
  struct tm *timeInfo = localtime(&rawTime);
  char timeString[9];  // hh:mm:ss
  strftime(timeString, sizeof(timeString), "%H:%M:%S", timeInfo);
  Serial.println(timeString);
}

////////
void saveForecastToFiles(DynamicJsonDocument forecastData, int forecastMinutes) {
  if (!SD.begin()) {
    Serial.println("SD Card initialization failed!");
    return;
  }

  // Отримуємо поточний час
  time_t rawTime = timeClient.getEpochTime();
  struct tm *timeInfo = localtime(&rawTime);
  char currentDate[11];
  strftime(currentDate, sizeof(currentDate), "%d-%m-%Y", timeInfo);

  int forecastSize = forecastData["list"].size();
  time_t startForecastTime = rawTime - (rawTime % 60);  // Поточна хвилина
  time_t endForecastTime = startForecastTime + forecastMinutes * 60;

  bool dataStock = false;
  bool newDay = false;
  // Перебір точок прогнозу
  for (int i = 0; i < forecastSize - 1; ++i) {
    if (newDay) {
      // Збільшення дати на 1 день
      timeInfo->tm_mday += 1;
      // Обробка зміни місяця і року
      mktime(timeInfo);  // Оновлення timeInfo, щоб врахувати зміни в місяці та році
      strftime(currentDate, sizeof(currentDate), "%d-%m-%Y", timeInfo);
      newDay = false;
    }

    time_t currentTime = static_cast<time_t>(forecastData["list"][i]["dt"]);
    time_t nextTime = static_cast<time_t>(forecastData["list"][i + 1]["dt"]);
    if (dataStock) break;
    if (currentTime <= endForecastTime && endForecastTime < nextTime) dataStock = true;

    struct tm *currentForecastTime = localtime(&currentTime);
    int startMinute = currentForecastTime->tm_hour * 60 + currentForecastTime->tm_min;

    if (nextTime == 1730678400) {  // 1730678400 = 00:00:00
      nextTime -= 30;
      newDay = true;
    }
    struct tm *nextForecastTime = localtime(&nextTime);
    int endMinute = nextForecastTime->tm_hour * 60 + nextForecastTime->tm_min;

    // Отримуємо початкові та кінцеві температури
    float startTemp = forecastData["list"][i]["main"]["temp"];
    float endTemp = forecastData["list"][i + 1]["main"]["temp"];
    float startClouds = forecastData["list"][i]["clouds"]["all"];
    float endClouds = forecastData["list"][i + 1]["clouds"]["all"];

    if (newDay)
      endMinute++;
    // Підготовка до запису в файл для поточної години
    for (int hour = startMinute / 60; hour < endMinute / 60; ++hour) {
      // Формуємо ім'я файлу
      String folderName = "/cache/forecast/" + String(currentDate);

      createDirectories(folderName);
      char fileName[20];
      sprintf(fileName, "%02d.json", hour);

      // Зчитуємо існуючий файл, якщо він є
      StaticJsonDocument<4096> existingData;
      File file = SD.open(folderName + "/" + fileName, FILE_READ);
      if (file) {
        DeserializationError error = deserializeJson(existingData, file);
        file.close();
        if (error) {
          Serial.println("Error reading JSON from file");
          existingData.clear();
        }
      }

      // Масив для зберігання оновлених хвилинних даних
      StaticJsonDocument<128> updatedMinuteForecasts[60];
      int forecastCount = 0;

      // Заповнення масиву хвилинного прогнозу
      for (int minute = 0; minute < 60; ++minute) {
        int absoluteMinute = hour * 60 + minute;
        bool updated = false;

        if (absoluteMinute >= startMinute && absoluteMinute < endMinute) {

          float temp = interpolate(startTemp, endTemp, startMinute, absoluteMinute, endMinute);
          float clouds = interpolate(startClouds, endClouds, startMinute, absoluteMinute, endMinute);

          if (!isnan(temp) && !isnan(clouds)) {
            char timeBuffer[9];
            sprintf(timeBuffer, "%02d:%02d:00", hour, minute);
            updatedMinuteForecasts[minute]["Time"] = String(timeBuffer);
            updatedMinuteForecasts[minute]["Temperature"] = temp;
            updatedMinuteForecasts[minute]["Cloudiness"] = clouds;
            updated = true;
          }
        }

        // Якщо оновлених даних немає, залишаємо старі, якщо вони існують
        if (!updated && existingData.containsKey(String(minute))) {
          updatedMinuteForecasts[minute] = existingData[String(minute)];
          char timeBuffer[9];
          sprintf(timeBuffer, "%02d:%02d:00", hour, minute);
        }
        forecastCount++;
      }

      // Запис оновлених даних у файл
      file = SD.open(folderName + "/" + fileName, FILE_WRITE);
      if (!file) {
        Serial.println("Failed to open file for writing");
        return;
      }

      // Формуємо JSON-масив і записуємо в файл
      file.print("[");
      for (int j = 0; j < forecastCount; j++) {
        serializeJson(updatedMinuteForecasts[j], file);
        if (j < forecastCount - 1) file.print(",");
      }
      file.print("]");
      file.close();  // Закриваємо файл після запису
    }
  }
}

///////
std::vector<ForecastData> loadForecastData(int forecastMinutes) {
  std::vector<ForecastData> forecastArray;

  // Отримуємо поточний час та розраховуємо кінцевий час
  time_t rawTime = timeClient.getEpochTime();
  struct tm *timeInfo = localtime(&rawTime);
  time_t endForecastTime = rawTime + forecastMinutes * 60;

  // Розраховуємо шляхи до файлів, які необхідно зчитати
  std::vector<String> requiredFilePaths;
  int initialMinuteOffset = timeInfo->tm_min;
  int hoursToCover = (forecastMinutes + initialMinuteOffset + 59) / 60;

  // Формуємо список шляхів до файлів, які потрібно зчитати
  for (int hourOffset = 0; hourOffset < hoursToCover; hourOffset++) {
    char currentDate[11];
    strftime(currentDate, sizeof(currentDate), "%d-%m-%Y", timeInfo);

    char fileName[30];
    sprintf(fileName, "/cache/forecast/%s/%02d.json", currentDate, timeInfo->tm_hour);
    requiredFilePaths.push_back(String(fileName));

    // Переходимо на наступну годину
    timeInfo->tm_hour++;
    if (timeInfo->tm_hour >= 24) {
      timeInfo->tm_hour = 0;
      timeInfo->tm_mday++;
      mktime(timeInfo);  // Оновлення структури `tm` для коректної дати
    }
  }

  // Прапор для початку запису даних, коли знайдений час буде більшим або рівним поточному часу
  bool startRecording = false;

  // Зчитування даних з файлів
  for (int j = 0; j < requiredFilePaths.size(); j++) {
    File file = SD.open(requiredFilePaths[j].c_str(), FILE_READ);
    if (!file) {
      Serial.print("Could not open file: ");
      Serial.println(requiredFilePaths[j]);
      continue;  // Пропускаємо файл, якщо його не вдалося відкрити
    }

    DynamicJsonDocument hourData(6000);
    DeserializationError error = deserializeJson(hourData, file);
    file.close();
    if (error) {
      Serial.print("Failed to parse JSON in ");
      Serial.println(requiredFilePaths[j]);
      continue;  // Пропускаємо файл, якщо є помилка в парсингу
    }

    JsonArray minuteEntries = hourData.as<JsonArray>();

    for (int i = 0; i < minuteEntries.size(); i++) {
      JsonObject minuteEntry = minuteEntries[i];
      // Перевірка наявності полів "Temperature" і "Cloudiness"
      if (!minuteEntry.containsKey("Temperature") || !minuteEntry.containsKey("Cloudiness")) {
        continue;  // Пропускаємо запис, якщо відсутні необхідні дані
      }

      // Отримання часу запису з файлу
      const char *entryTimeStr = minuteEntry["Time"];

      // Виділяємо годину та хвилини з рядка
      int entryHour = (entryTimeStr[0] - '0') * 10 + (entryTimeStr[1] - '0');
      int entryMinute = (entryTimeStr[3] - '0') * 10 + (entryTimeStr[4] - '0');

      // Використовуємо поточну дату, щоб задати тільки години та хвилини
      struct tm entryTime = *timeInfo;  // Копіюємо поточну дату
      entryTime.tm_hour = entryHour;
      entryTime.tm_min = entryMinute;
      entryTime.tm_sec = 0;  // Секунди встановимо в 0

      time_t entryEpochTime = mktime(&entryTime);

      // Починаємо запис, якщо час запису більший або рівний поточному часу
      if (!startRecording && entryEpochTime >= rawTime && j == 0) {
        startRecording = true;
      }

      // Якщо старт запису почався, але час вже вийшов за межі шуканого інтервалу, закінчуємо обробку поточного файлу
      if (startRecording && entryEpochTime > endForecastTime && requiredFilePaths.size() - 1 == j) {
        return forecastArray;
      }

      // Додаємо структуру в масив, якщо почався запис
      if (startRecording) {
        ForecastData data;
        data.Time = String(entryTimeStr);
        data.Temperature = minuteEntry["Temperature"].as<float>();
        data.Cloudiness = minuteEntry["Cloudiness"].as<float>();
        forecastArray.push_back(data);
      }
    }
  }

  return forecastArray;
}






////////
// Функція для отримання температури та хмарності
void getWeatherData(float lat, float lon, float &temperature, int &cloudiness) {
  String units = "metric";
  unsigned long currentTime = timeClient.getEpochTime();  // Отримуємо поточний Unix-час
  unsigned long fiveMinutes = 300;                        // 5 хвилин у секундах (300 секунд)

  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;

    // Формуємо API-запит з широтою, довготою та одиницями виміру
    String apiUrl = "http://api.openweathermap.org/data/2.5/weather?lat=" + String(lat, 6) + "&lon=" + String(lon, 6) + "&appid=" + apiKey + "&units=" + units;

    http.begin(apiUrl);         // Встановлюємо URL для запиту
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

        // Оновлюємо змінні успішного оновлення
        lastUpdateTime = currentTime;  // Оновлюємо Unix-час останнього успішного запиту
        weatherUpdated = true;         // Дані успішно оновлені
        weatherUpdatedStatus = "Weather data updated successfully.";
      } else {
        weatherUpdatedStatus = "Error on HTTP request, using previous data.";
      }
    } else {
      weatherUpdatedStatus = "Failed to connect to API, using previous data.";
    }

    http.end();  // Закриваємо з'єднання
  } else {
    weatherUpdatedStatus = "WiFi Disconnected, using previous data.";
  }

  // Перевірка, чи пройшло більше 5 хвилин з останнього успішного оновлення
  if (!weatherUpdated || (currentTime - lastUpdateTime) > fiveMinutes) {
    weatherUpdated = false;  // Якщо більше 5 хвилин без успішного оновлення, дані вважаються застарілими
    weatherUpdatedStatus = "Data not updated for more than 5 minutes.";
  }
}


struct SetEnergyData {
  String Time;
  float PowerConsumption = 0.0;
  float SolarGeneration = 0.0;
  float Temperature = 0.0;
  float Cloudiness = 0.0;
  int countPowerConsumption = 0;
  int countSolarGeneration = 0;
  int countTemperature = 0;
  int countCloudiness = 0;
};
// Функція для створення всіх директорій на основі повного шляху
void createDirectories(String path) {
  String subPath = "";

  for (int i = 0; i < path.length(); i++) {
    char c = path.charAt(i);
    subPath += c;

    // Коли зустрічаємо '/', перевіряємо чи існує ця папка, і створюємо її за необхідності
    if (c == '/' && !SD.exists(subPath)) {
      SD.mkdir(subPath);
    }
  }

  // Якщо повний шлях — це не файл, перевіряємо наявність останньої директорії (без '/')
  if (subPath.length() > 0 && !SD.exists(subPath)) {
    SD.mkdir(subPath);
  }
}

// Функція для отримання середніх значень та запису у новий файл
void calculateAndStoreAverages(std::vector<String> filePaths, String inputDate, String fileType) {
  if (!sDIsOk())
    return;
  SetEnergyData energyData[60];  // Масив для 60 записів на кожну хвилину
  // Формуємо шлях для нового файлу на основі вхідних параметрів дати, типу файлу і часу
  String yearMonth = inputDate.substring(3, 5) + "-" + inputDate.substring(6, 10);  // Формуємо папку виду mm-yyyy
  String fullDate = inputDate + "-" + fileType;                                     // Формуємо папку виду dd-mm-yyyy-long-everage або інший варіант
  String newDir = "/cache/" + yearMonth + "/" + fullDate;

  createDirectories(newDir);

  // Отримуємо годину з назви файлу (наприклад, hh.json)
  if (filePaths.size() < 1) {
    Serial.println("Пустий перечень вхідних даних");
    return;
  }
  String hour = filePaths[0].substring(filePaths[0].length() - 7, filePaths[0].length() - 5);  // Витягуємо годину з імені файлу

  // Ініціалізація структури з порожніми значеннями та часу для кожної хвилини
  for (int i = 0; i < 60; i++) {
    if (i < 10) {
      energyData[i].Time = hour + ":0" + String(i) + ":00";  // Додаємо провідний нуль для хвилин менше 10
    } else {
      energyData[i].Time = hour + ":" + String(i) + ":00";  // Для хвилин більше або рівних 10
    }
  }

  // Читання файлів
  for (const String &filePath : filePaths) {
    File file = SD.open(filePath);
    if (!file) {
      Serial.println("Не вдалося відкрити файл: " + filePath);
      continue;
    }

    // Читання JSON-даних з кожного файлу
    StaticJsonDocument<1024> doc;
    DeserializationError error = deserializeJson(doc, file);
    if (error) {
      Serial.println("Помилка парсингу JSON у файлі: " + filePath);
      file.close();
      continue;
    }

    // Читання даних для кожної хвилини
    for (int i = 0; i < 60; i++) {
      String timeKey = energyData[i].Time;  // використовуємо нове значення Time
      if (doc.containsKey(timeKey)) {
        JsonObject data = doc[timeKey];

        if (data.containsKey("PowerConsumption")) {
          energyData[i].PowerConsumption += data["PowerConsumption"].as<float>();
          energyData[i].countPowerConsumption++;
        }
        if (data.containsKey("SolarGeneration")) {
          energyData[i].SolarGeneration += data["SolarGeneration"].as<float>();
          energyData[i].countSolarGeneration++;
        }
        if (data.containsKey("Temperature")) {
          energyData[i].Temperature += data["Temperature"].as<float>();
          energyData[i].countTemperature++;
        }
        if (data.containsKey("Cloudiness")) {
          energyData[i].Cloudiness += data["Cloudiness"].as<float>();
          energyData[i].countCloudiness++;
        }
      }
    }

    file.close();  // Закриваємо файл після зчитування
  }

  // Обчислення середніх значень
  for (int i = 0; i < 60; i++) {
    if (energyData[i].countPowerConsumption > 0) {
      energyData[i].PowerConsumption /= energyData[i].countPowerConsumption;
    }
    if (energyData[i].countSolarGeneration > 0) {
      energyData[i].SolarGeneration /= energyData[i].countSolarGeneration;
    }
    if (energyData[i].countTemperature > 0) {
      energyData[i].Temperature /= energyData[i].countTemperature;
    }
    if (energyData[i].countCloudiness > 0) {
      energyData[i].Cloudiness /= energyData[i].countCloudiness;
    }
  }

  // Створення папки якщо не існує
  if (!SD.exists(newDir)) {
    SD.mkdir(newDir);
  }
  Serial.println(newDir);
  // Запис результатів у новий файл
  String outputHour = newDir + "/" + hour + ".json";
  File outputFile = SD.open(outputHour, FILE_WRITE);
  if (!outputFile) {
    Serial.println("Не вдалося відкрити файл для запису: " + outputHour);
    return;
  }
  // Видаляємо існуючі записи у файлі, щоб перезаписати їх
  outputFile.print("[");

  // Записуємо кожен запис (60 структур)
  for (int i = 0; i < 60; i++) {
    DynamicJsonDocument doc(256);
    JsonObject newRecord = doc.to<JsonObject>();
    newRecord["Time"] = energyData[i].Time;
    newRecord["PowerConsumption"] = energyData[i].PowerConsumption;
    newRecord["SolarGeneration"] = energyData[i].SolarGeneration;
    newRecord["Temperature"] = energyData[i].Temperature;
    newRecord["Cloudiness"] = energyData[i].Cloudiness;

    serializeJson(newRecord, outputFile);
    if (i < 59) {
      outputFile.print(",");  // Додаємо кому між записами
    }
  }

  outputFile.print("]");  // Закриваємо масив JSON
  outputFile.close();     // Закриваємо файл після запису
  Serial.println("Дані успішно записані у файл: " + outputFile);
}

#include <SD.h>
#include <vector>

// Функція для пошуку всіх файлів у папці за вказаною датою і типом файлу
std::vector<String> getCacheFilePaths(String inputDate, String fileType) {
  std::vector<String> filePaths;
  if (!sDIsOk()) {
    return {};
  }
  // Формуємо шлях до папки на основі вхідних параметрів дати і типу файлу
  String yearMonth = inputDate.substring(3, 5) + "-" + inputDate.substring(6, 10);  // Формуємо папку виду mm-yyyy
  String fullDate = inputDate + "-" + fileType;                                     // Формуємо папку виду dd-mm-yyyy-long-everage або інший варіант
  String dirPath = "/cache/" + yearMonth + "/" + fullDate;                          // Повний шлях до папки

  if (!SD.exists(dirPath)) {
    Serial.println("Папка не існує: " + dirPath);
    return filePaths;  // Повертаємо порожній масив, якщо папка не існує
  }

  // Відкриваємо папку і зчитуємо всі файли всередині
  File dir = SD.open(dirPath);
  if (!dir) {
    Serial.println("Не вдалося відкрити папку: " + dirPath);
    return filePaths;
  }

  File file = dir.openNextFile();
  while (file) {
    // Перевіряємо, чи є об'єкт файлом, а не папкою
    if (!file.isDirectory()) {
      String fileName = file.name();
      // Додаємо шлях до файлу в список
      filePaths.push_back(dirPath + "/" + fileName);
    }
    file = dir.openNextFile();  // Переходимо до наступного файлу
  }

  dir.close();  // Закриваємо папку після зчитування

  return filePaths;
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
  if (fileName.length() > 14) {
    return "";
  }
  String dateStr = fileName.substring(0, 10);  // Витягуємо "dd-mm-yyyy"
  tmElements_t tm = parseDate(dateStr);
  time_t t = makeTime(tm);
  int dayOfWeek = weekday(t);  // Отримуємо день тижня (1 — неділя, 2 — понеділок, ...)

  return getDayOfWeek(dayOfWeek);
}
std::vector<String> findFilesForLastDays(int daysToCheck, String time) {
  std::vector<String> files;
  if (!sDIsOk()) {
    return files;
  }

  time_t currentTime = timeClient.getEpochTime();  // Поточний UNIX-час
  String hour = time.substring(0, 2);              // Витягуємо годину з часу (HH)

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

      int dayDiff = (currentTime - fileTime) / SECS_PER_DAY;  // Різниця у днях
      // Перевіряємо, чи файл відповідає критеріям за останні 25 днів
      if (dayDiff > 0 && dayDiff <= daysToCheck) {
        // Перевіряємо наявність файлу з відповідною годиною
        String filePath = folderName + "/" + fileName.substring(0, 10) + "/" + hour + ".json";
        if (SD.exists(filePath)) {
          // Перевіряємо, чи файл не порожній
          File hourFile = SD.open(filePath, FILE_READ);
          if (hourFile && hourFile.size() > 2) {
            files.push_back(filePath);
          }
          hourFile.close();
        }
      }
      entry.close();

      if (files.size() >= 4) break;  // Якщо знайшли достатньо файлів
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
        tmElements_t fileDate = parseDate(fileName.substring(0, 10));
        time_t fileTime = makeTime(fileDate);
        int dayDiff = (currentTime - fileTime) / SECS_PER_DAY;

        // Перевіряємо, чи файл містить дані за останні 25 днів і чи не старший він
        if (dayDiff > 0 && dayDiff <= daysToCheck) {
          String filePath = prevFolderName + "/" + fileName.substring(0, 10) + "/" + hour + ".json";
          if (SD.exists(filePath)) {
            File hourFile = SD.open(filePath, FILE_READ);
            if (hourFile && hourFile.size() > 2) {
              files.push_back(filePath);
            }
            hourFile.close();
          }
        }
        entry.close();

        if (files.size() >= 4) break;  // Якщо знайшли достатньо файлів
      }
      dir.close();
    }
  }

  return files;
}




std::vector<String> findFilesByDayOfWeekAndTime(String dayOfWeek, int daysToCheck, String time) {
  std::vector<String> files;
  if (!sDIsOk()) {
    return files;
  }
  // Перший пошук у поточному місяці
  String date = getFormattedDate();
  String dayInFormat = date.substring(8, 10) + "-" + date.substring(5, 7) + "-" + date.substring(0, 4);
  String currentMonth = getFormattedDate().substring(5, 7);    // Отримуємо поточний місяць (MM)
  String currentYear = getFormattedDate().substring(0, 4);     // Отримуємо поточний рік (YYYY)
  String folderName = "/" + currentMonth + "_" + currentYear;  // Папка формату mm_yyyy
  String hour = time.substring(0, 2);                          // Витягуємо годину з часу (HH)
  if (SD.exists(folderName)) {
    File dir = SD.open(folderName);
    while (true) {
      File entry = dir.openNextFile();
      if (!entry) break;
      String fileName = entry.name();
      String dayFolder = fileName.substring(0, 10);  // Отримуємо ім'я папки у форматі DD-MM-YYYY


      if (getDayOfWeekFromFileName(dayFolder) == dayOfWeek && dayInFormat != dayFolder) {
        // Перевіряємо, чи існує файл з відповідною годиною всередині папки
        String filePath = folderName + "/" + dayFolder + "/" + hour + ".json";
        if (SD.exists(filePath)) {
          // Відкриваємо файл для перевірки, чи він не порожній і має записи
          File hourFile = SD.open(filePath, FILE_READ);
          if (hourFile && hourFile.size() > 2) {  // Перевіряємо, чи є в файлі дані
            files.push_back(filePath);
          }
          hourFile.close();
        }
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
        String dayFolder = fileName.substring(0, 10);  // Отримуємо ім'я папки у форматі DD-MM-YYYY

        if (getDayOfWeekFromFileName(dayFolder) == dayOfWeek) {
          // Перевіряємо, чи не старіший цей файл більше ніж на 25 днів
          tmElements_t fileDate = parseDate(dayFolder);
          time_t fileTime = makeTime(fileDate);
          time_t currentTime = timeClient.getEpochTime();
          int dayDiff = (currentTime - fileTime) / SECS_PER_DAY;
          if (dayDiff <= daysToCheck) {
            // Перевіряємо наявність файлу з відповідною годиною
            String filePath = prevFolderName + "/" + dayFolder + "/" + hour + ".json";
            if (SD.exists(filePath)) {
              // Відкриваємо файл для перевірки, чи він не порожній і має записи
              File hourFile = SD.open(filePath, FILE_READ);
              if (hourFile && hourFile.size() > 2) {  // Перевіряємо, чи є в файлі дані
                files.push_back(filePath);
              }
              hourFile.close();
            }
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
  time_t rawTime = timeClient.getEpochTime();
  struct tm *timeInfo = localtime(&rawTime);  // Конвертуємо в структуру часу

  // Формуємо рядок у форматі yyyy-mm-dd
  char buffer[11];
  snprintf(buffer, sizeof(buffer), "%04d-%02d-%02d", timeInfo->tm_year + 1900, timeInfo->tm_mon + 1, timeInfo->tm_mday);
  return String(buffer);
}
String getFormattedDate(time_t rawTime) {
  struct tm *timeInfo = localtime(&rawTime);  // Конвертуємо в структуру часу

  // Формуємо рядок у форматі yyyy-mm-dd
  char buffer[11];
  snprintf(buffer, sizeof(buffer), "%04d-%02d-%02d", timeInfo->tm_year + 1900, timeInfo->tm_mon + 1, timeInfo->tm_mday);
  return String(buffer);
}


// Функция для проверки SD-карты без лишних сообщений
bool sDIsOk() {
  // Инициализация SD-карты
  if (SD.begin(chipSelect) && SD.open("/static/index.js")) {
    SD_Status = "SD card initialization fine";
    return true;
  }
  SD_Status = "SD card initialization failed!";
  return false;
}

void writeDataToSD(String requestTime) {
  if (!sDIsOk()) {
    return;
  }
  fileParseStatus = "";
  currentFileName = "";
  fileStatus = "";

  // Оновлюємо назву файлу відповідно до дати і часу
  String date = getFormattedDate();
  String yearMonthFolder = "/" + date.substring(5, 7) + "_" + date.substring(0, 4);                          // MM_YYYY
  String dayFolder = "/" + date.substring(8, 10) + "-" + date.substring(5, 7) + "-" + date.substring(0, 4);  // DD-MM-YYYY
  String hour = requestTime.substring(0, 2);                                                                 // Витягуємо "HH"
  String fileName = hour + ".json";                                                                          //Формат: HH.json

  // Створюємо папку року і місяця, якщо вона не існує
  if (!SD.exists(yearMonthFolder)) {
    if (!SD.mkdir(yearMonthFolder)) {
      fileParseStatus = "Failed to create year-month folder: " + yearMonthFolder;
      return;
    }
  }

  // Створюємо папку дня, якщо вона не існує
  String fullFolderPath = yearMonthFolder + dayFolder;
  if (!SD.exists(fullFolderPath)) {
    if (!SD.mkdir(fullFolderPath)) {
      fileParseStatus = "Failed to create day folder: " + fullFolderPath;
      return;
    }
  }

  currentFileName = fullFolderPath + "/" + fileName;

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
    file.print(",");             // Додаємо кому перед новим елементом
  }

  // Додаємо новий елемент у JSON масив
  DynamicJsonDocument doc(256);
  JsonObject newRecord = doc.to<JsonObject>();
  newRecord["Time"] = requestTime;
  newRecord["PowerConsumption"] = powerConsumption;
  newRecord["SolarGeneration"] = solarGeneration;
  newRecord["Temperature"] = temperature;
  newRecord["Cloudiness"] = cloudiness;
  // Записуємо новий елемент
  serializeJson(newRecord, file);
  file.print("]");  // Закриваємо масив

  file.close();

  fileStatus = "Data written to SD card";
}


String makeIndexFile(String chunk) {
  String chunkUrl = "/static/" + chunk;

  if (sDIsOk()) {
    if (SD.open("/static/index.js")) {
      return "<!DOCTYPE html>"
             "<html lang=\"en\">"
             "<head>"
             "<meta charset=\"utf-8\" />"
             "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />"
             "<title>The invertor controller</title>"
             "<style>"
             "@font-face {"
             "  font-family: 'TMVinograd-Regular';"
             "  src: url('/static/TMVinograd-Regular.ttf') format('truetype');"
             "}"
             "</style>"
             "<script src=\"/static/shared.js\"></script>"
             "</head>"
             "<body style=\"display: block;\">"
             "<script src="
             + chunkUrl + "></script>"
                          "<div id=\"app\"></div>"
                          "</body>"
                          "</html>";
    }
  }

  String pCode = "const send_patch = (url) => async () =>{"
                 "let response = await fetch(url, {"
                 "method: 'PATCH',});"
                 "if (!response.ok) {"
                 "throw new Error('Network response was not ok');}}";

  String html = "<!DOCTYPE html>"
                "<html lang=\"en\">"
                "<head>"
                "<meta charset=\"utf-8\" />"
                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />"
                "<title>The invertor controller</title>"
                "<script>"
                + pCode + "</script>"
                          "</head>"
                          "<body style=\"display: block;\">"
                          "<form action='/saveWifi' method='POST'>"
                          "<label>WiFi SSID:</label><input type='text' name='ssid' value='"
                + String(wifiSSID) + "'><br>"
                                     "<label>WiFi Password:</label><input type='password' name='password' value='"
                + String(wifiPassword) + "'><br>"
                                         "<input type='submit' value='Save'></form><br>"
                                         "<form action='/saveWeather' method='POST'>"
                                         "<label>ApiKey:</label><input type='text' name='apiKey' value='"
                + String(apiKey) + "'><br>"
                                   "<label>Latitude:</label><input  name='latitude' value='"
                + String(latitude) + "'><br>"
                                     "<label>Longitude:</label><input  name='longitude' value='"
                + String(longitude) + "'><br>"
                                      "<input type='submit' value='Save'></form><br>"
                                      "<button onclick=\"send_patch('/start_ap')\">Start Access Point</button><br>"
                                      "<button onclick=\"send_patch('/connect_wifi')\">Connect to WiFi</button><br>"
                                      "<button onclick=\"send_patch('/start_work')\">Start Work</button><br>"
                                      "<button onclick=\"send_patch('/stop_work')\">Stop Work</button><br>"
                                      "<label>SD Status: "
                + String(SD_Status) + "</label><br>"
                                      "<label>Program isWorking: "
                + String(isWorking) + "</label><br>"
                                      "<label>WiFi status: "
                + String(Wifi_status) + "</label><br>"
                                        "<label>Current FileName: "
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
  server.on("/files", HTTP_GET, [](AsyncWebServerRequest *request) {
    DynamicJsonDocument doc(4096);  // Створюємо більший JSON документ

    if (sDIsOk()) {  // Якщо SD карта готова, отримуємо файли
      std::vector<String> files = getCacheFilePaths("20-10-2024", "last-25");

      // Для кожного файлу з масиву
      for (const String &file : files) {
        // Приклад шляху файлу: "/cache/2024-09/21-09-2024-fileType/15.json"
        int lastSlash = file.lastIndexOf('/');
        int secondLastSlash = file.lastIndexOf('/', lastSlash - 1);

        // Витягуємо назву папки з "21-09-2024-fileType"
        String dayName = file.substring(secondLastSlash + 1, lastSlash);

        // Витягуємо годину з назви файлу, наприклад, "15" з "15.json"
        String hour = file.substring(lastSlash + 1, file.length());

        // Перевіряємо, чи вже існує такий день у JSON
        JsonArray dayArray;
        if (doc.containsKey(dayName)) {
          dayArray = doc[dayName].as<JsonArray>();
        } else {
          dayArray = doc.createNestedArray(dayName);  // Створюємо масив для нового дня
        }
        // Додаємо годину до масиву
        JsonObject jsonObject = dayArray.createNestedObject();
        jsonObject["name"] = hour;  // Додаємо годину до JSON
      }

      // Сериалізуємо JSON-об'єкт у строку
      String jsonData;
      serializeJson(doc, jsonData);
      // Відправляємо JSON-відповідь
      request->send(200, "application/json", jsonData);
    } else {
      // Якщо SD карта недоступна, відправляємо порожній об'єкт
      request->send(200, "application/json", "{}");
    }
  });


  // Сервер для завантаження files з SD-карти
  // right is on sd card
  if (sDIsOk()) {
    if (SD.open("/static/index.js")) {
      server.serveStatic("/static/", SD, "/static/");
    }
  }

  // migrate all the data interactions into this way
  server.on("/status", HTTP_GET, [](AsyncWebServerRequest *request) {
    // Формуємо JSON з даними
    DynamicJsonDocument doc(1024);

    doc["temperature"] = temperature;
    doc["cloudiness"] = cloudiness;
    doc["solarGeneration"] = solarGeneration;
    doc["powerConsumption"] = powerConsumption;
    doc["sdStatus"] = SD_Status;
    doc["isWorking"] = isWorking;
    doc["wifiStatus"] = Wifi_status;
    doc["currentFileName"] = currentFileName;
    doc["fileStatus"] = fileStatus;
    doc["fileParseStatus"] = fileParseStatus;
    doc["time"] = String(getFormattedDate() + " " + timeClient.getFormattedTime());
    doc["lastUpdateTime"] = getFormattedDate(lastUpdateTime);
    doc["weatherUpdated"] = weatherUpdated;
    doc["weatherUpdatedStatus"] = weatherUpdatedStatus;

    String jsonResponse;
    serializeJson(doc, jsonResponse);

    // Відправляємо JSON-відповідь
    request->send(200, "application/json", jsonResponse);
  });

  server.on("/options", HTTP_GET, [](AsyncWebServerRequest *request) {
    // Формуємо JSON з даними
    DynamicJsonDocument doc(1024);

    // wifiSSID, wifiPassword, apiKey, latitude, longitude

    doc["ssid"] = String(wifiSSID);
    doc["password"] = String(wifiPassword);

    doc["apiKey"] = String(apiKey);
    doc["latitude"] = String(latitude);
    doc["longitude"] = String(longitude);

    doc["ip"] = String(invertorIp);
    doc["port"] = String(invertorPort);
    doc["id"] = String(invertorId);


    String jsonResponse;
    serializeJson(doc, jsonResponse);

    // Відправляємо JSON-відповідь
    request->send(200, "application/json", jsonResponse);
  });

  // Інші наявні маршрути
  server.on("/saveWifi", HTTP_POST, [](AsyncWebServerRequest *request) {
    String ssid = request->getParam("ssid")->value();
    String password = request->getParam("password")->value();

    ssid.toCharArray(wifiSSID, sizeof(wifiSSID));
    password.toCharArray(wifiPassword, sizeof(wifiPassword));

    preferences.begin("esp-settings", false);
    preferences.putString("wifiSSID", wifiSSID);
    preferences.putString("wifiPassword", wifiPassword);
    preferences.end();

    request->send(200);
  });
  server.on("/saveInvertor", HTTP_POST, [](AsyncWebServerRequest *request) {
    String NinvertorIp = request->getParam("ip")->value();
    String NinvertorPort = request->getParam("port")->value();
    String NinvertorId = request->getParam("id")->value();

    NinvertorIp.toCharArray(invertorIp, sizeof(invertorIp));
    NinvertorPort.toCharArray(invertorPort, sizeof(invertorPort));
    NinvertorId.toCharArray(invertorId, sizeof(invertorId));

    preferences.begin("esp-settings", false);
    preferences.putString("invertorIp", invertorIp);
    preferences.putString("invertorPort", invertorPort);
    preferences.putString("invertorId", invertorId);
    preferences.end();

    request->send(200);
  });

  server.on("/saveWeather", HTTP_POST, [](AsyncWebServerRequest *request) {
    String NapiKey = request->getParam("apiKey")->value();
    String Nlatitude = request->getParam("latitude")->value();
    String Nlongitude = request->getParam("longitude")->value();

    NapiKey.toCharArray(apiKey, sizeof(apiKey));
    Nlatitude.toCharArray(latitude, sizeof(latitude));
    Nlongitude.toCharArray(longitude, sizeof(longitude));

    preferences.begin("esp-settings", false);
    preferences.putString("apiKey", apiKey);
    preferences.putString("latitude", latitude);
    preferences.putString("longitude", longitude);
    preferences.end();


    request->send(200);
  });

  server.on("/start_work", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
    preferences.putBool("isWorking", true);  // Точка доступу
    preferences.end();
    isWorking = true;
    request->send(200);
  });

  server.on("/stop_work", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
    preferences.putBool("isWorking", false);  // Точка доступу
    preferences.end();
    isWorking = false;
    request->send(200, "/");
  });

  server.on("/sync_time", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    syncTime();
    request->send(200);
  });

  server.on("/start_ap", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
    preferences.putInt("mode", 0);  // Точка доступу
    preferences.end();
    request->send(200);
    delay(1000);
    ESP.restart();
  });

  server.on("/connect_wifi", HTTP_PATCH, [](AsyncWebServerRequest *request) {
    preferences.begin("esp-settings", false);
    preferences.putInt("mode", 1);  // Режим Wi-Fi
    preferences.end();
    request->send(200);
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
    vTaskDelay(1);  // Yield control back to the OS
  }
  digitalWrite(LED_PIN, LOW);
  ledStopFlag = true;
  vTaskDelete(NULL);
}

void getInfoFromInvertor() {
  if (isWorking && timeSynced && timeClient.getSeconds() == 0) {
    int hours = timeClient.getHours();
    int minutes = timeClient.getMinutes();
    String time = GetTimeByHoursAndMinuts(hours, minutes);
    readModbusData();
    getWeatherData(atof(latitude), atof(longitude), temperature, cloudiness);
    writeDataToSD(time);
  }
}
String GetTimeByHoursAndMinuts(int hours, int minutes) {
  return String(hours < 10 ? "0" : "") + String(hours) + ":" + String(minutes < 10 ? "0" : "") + String(minutes) + ":" + "00";
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
