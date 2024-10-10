#include <ModbusMaster.h>
#include <WiFi.h>
#include <WebServer.h>

ModbusMaster node;

// Номер серійного порту для зв'язку з ПК
#define SERIAL_PORT Serial

// Параметри WiFi
const char* ssid = "Tenda";
const char* password = "25071979";

// Створення веб-сервера на порту 80
WebServer server(80);

// Змінні для трекінгу часу
unsigned long previousMillis = 0;
unsigned long previousMillisForRegister35 = 0;
const long interval = 1000;  // Оновлення кожну секунду
const long intervalForRegister35 = 5000;  // Оновлення кожні 5 секунд
  uint16_t reg0, reg1;
// Значення регістра 35
float register35Value = 0.0;

void setup() {
  // Ініціалізація серійного порту
  SERIAL_PORT.begin(9600);
  while (!SERIAL_PORT) {
    ; // Чекаємо підключення порту
  }

  // Налаштування Modbus
  node.begin(1, SERIAL_PORT);  // ID = 1 (Slave address на ПК)
  
  // Підключення до WiFi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    SERIAL_PORT.println("Connecting to WiFi...");
  }
  SERIAL_PORT.println("Connected to WiFi!");
  SERIAL_PORT.print("IP address: ");
  SERIAL_PORT.println(WiFi.localIP());

  // Налаштування веб-сервера
  server.on("/", handleRoot);
  server.begin();
  SERIAL_PORT.println("Web server started!");

  SERIAL_PORT.println("ModbusMaster Test");
}

void loop() {
  // Отримуємо поточний час
  unsigned long currentMillis = millis();

  // Читання регістрів 0 і 1 кожну секунду
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    readRegisters();
  }

  // Зміна значення регістра 35 кожні 5 секунд
  if (currentMillis - previousMillisForRegister35 >= intervalForRegister35) {
    previousMillisForRegister35 = currentMillis;
    incrementRegister35();
  }

  // Обробка запитів до веб-сервера
  server.handleClient();
}

// Функція для читання регістрів 0 та 1
void readRegisters() {
  uint8_t result;


  // Читання регістра 0
  result = node.readHoldingRegisters(0, 1);
  if (result == node.ku8MBSuccess) {
    reg0 = node.getResponseBuffer(0);
    SERIAL_PORT.print("Register 0: ");
    SERIAL_PORT.println(reg0);
  } else {
    SERIAL_PORT.print("Error reading register 0: ");
    SERIAL_PORT.println(result, HEX);
  }
  delay(300);
  // Читання регістра 1
  result = node.readHoldingRegisters(1, 1);
  if (result == node.ku8MBSuccess) {
    reg1 = node.getResponseBuffer(0);
    SERIAL_PORT.print("Register 1: ");
    SERIAL_PORT.println(reg1);
  } else {
    SERIAL_PORT.print("Error reading register 1: ");
    SERIAL_PORT.println(result, HEX);
  }
}

// Функція для збільшення значення регістра 35
void incrementRegister35() {
  register35Value += 0.5;

  // Запис у регістр 35
  uint8_t result = node.writeSingleRegister(1, (uint16_t)(register35Value * 100)); // Приведення до формату Modbus
  if (result == node.ku8MBSuccess) {
    SERIAL_PORT.print("Register 35 updated to: ");
    SERIAL_PORT.println(register35Value);
  } else {
    SERIAL_PORT.print("Error updating register 35: ");
    SERIAL_PORT.println(result, HEX);
  }
}

// Обробка запитів до кореневого маршруту веб-сервера
void handleRoot() {
  String html = "<html><body><h1>Modbus Registers</h1>";
  html += "<p>Register 0: " + String(reg0) + "</p>";
  html += "<p>Register 1: " + String(reg1) + "</p>";
  html += "<p>Register 35: " + String(register35Value) + "</p>";
  html += "</body></html>";

  server.send(200, "text/html", html);
}




