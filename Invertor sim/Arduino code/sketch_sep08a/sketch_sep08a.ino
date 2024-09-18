#include <ModbusMaster.h>

#define RXD2 16
#define TXD2 17

ModbusMaster node;

void setup() {
  Serial.begin(9600);
  Serial2.begin(9600, SERIAL_8N1, RXD2, TXD2);

  node.begin(1, Serial2); // Адреса 1
}

void loop() {
  uint8_t result;
  result = node.readHoldingRegisters(0, 10); // Запитуємо 10 регістрів з адреси 0

  if (result == node.ku8MBSuccess) {
    for (uint8_t i = 0; i < 10; i++) {
      Serial.print("Register ");
      Serial.print(i);
      Serial.print(": ");
      Serial.println(node.getResponseBuffer(i));
    }
  } else {
    Serial.println("Failed to read registers");
  }

  delay(1000);
}
